using Microsoft.Extensions.Options;
using Infrastructure.Repository.IRepository;
using Infrastructure.Repository;
using Infrastructure.Services.Implementation;
using Infrastructure.Configuration;
using Microsoft.AspNetCore.HttpOverrides;
using Infrastructure.Services.Interfaces;
using Microsoft.AspNetCore.RateLimiting;
using System.Threading.RateLimiting;
using Polly;
using Polly.Extensions.Http;
using System.Net;
using API.Middleware;



var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.Configure<IpApiServiceSettings>(builder.Configuration.GetSection("IpApi"));
builder.Services.Configure<RedisSettings>(builder.Configuration.GetSection("ConnectionStrings"));

var serviceProvider = builder.Services.BuildServiceProvider();
var redisSettings = serviceProvider.GetRequiredService<IOptions<RedisSettings>>().Value;

builder.Services.AddHttpClient<IGeolocationService, GeolocationService>((serviceProvider, client) =>
{
    var settings = serviceProvider.GetRequiredService<IOptions<IpApiServiceSettings>>().Value;
    client.BaseAddress = new Uri(settings.BaseUrl.TrimEnd('/') + "/");
})
.AddPolicyHandler(GetRetryPolicy())
.AddHttpMessageHandler(provider =>
{
    var settings = provider.GetRequiredService<IOptions<IpApiServiceSettings>>().Value;
    return new ApiKeyHandler(settings.ApiKey); // Fix: Resolve settings here
});
builder.Services.AddScoped<IBlockedCountryRepository, BlockedCountryRepository>();
builder.Services.AddScoped<IIpRepository, IpRepository>();
builder.Services.AddScoped<ILogRepository, LogRepository>();
builder.Services.AddScoped<RedisCacheService>();

//Redis - add Redis caching service in IServiceCollection
builder.Services.AddStackExchangeRedisCache( options =>
{
    options.Configuration = redisSettings.RedisConn;
    options.InstanceName = redisSettings.InstanceName;
});
builder.Services.AddRateLimiter(options =>
{
    options.AddFixedWindowLimiter("api", limiter =>
    {
        limiter.PermitLimit = 4;
        limiter.Window = TimeSpan.FromSeconds(12);
        limiter.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
    });
});

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
});
builder.Services.AddDistributedMemoryCache();
builder.Services.AddHttpContextAccessor();

builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseMiddleware<ApiExceptionMiddleware>();
app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
});
app.UseSession();
app.UseRateLimiter();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.Run();
static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
{
return HttpPolicyExtensions
    .HandleTransientHttpError()
    .OrResult(msg => msg.StatusCode == HttpStatusCode.TooManyRequests)
    .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));
}

// API key handler
public class ApiKeyHandler : DelegatingHandler
{
    private readonly string _apiKey;

    public ApiKeyHandler(string apiKey) => _apiKey = apiKey;

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var uriBuilder = new UriBuilder(request.RequestUri!);
        var query = System.Web.HttpUtility.ParseQueryString(uriBuilder.Query);
        query["access_key"] = _apiKey;
        uriBuilder.Query = query.ToString();
        request.RequestUri = uriBuilder.Uri;
        return await base.SendAsync(request, cancellationToken);
    }
}