using Core.Services.Interfaces;
using Microsoft.Extensions.Options;
using Infrastructure.Repository.IRepository;
using Infrastructure.Repository;
using Infrastructure.Services.Implementation;
using Infrastructure.Configuration;
using Microsoft.AspNetCore.HttpOverrides;



var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.Configure<IpApiServiceSettings>(builder.Configuration.GetSection("IpApi"));
builder.Services.Configure<RedisSettings>(builder.Configuration.GetSection("ConnectionStrings"));

var serviceProvider = builder.Services.BuildServiceProvider();
var redisSettings = serviceProvider.GetRequiredService<IOptions<RedisSettings>>().Value;

builder.Services.AddHttpClient<IGeolocationService, GeolocationService>(
    (serviceProvider, client) =>
    {
        var settings = serviceProvider
            .GetRequiredService<IOptions<IpApiServiceSettings>>().Value;
        client.BaseAddress = new Uri(settings.BaseUrl);
    });

builder.Services.AddScoped<IBlockedCountryRepository, BlockedCountryRepository>();
builder.Services.AddScoped<IIpRepository, IpRepository>();
builder.Services.AddScoped<RedisCacheService>();

//Redis - add Redis caching service in IServiceCollection
builder.Services.AddStackExchangeRedisCache( options =>
{
    options.Configuration = redisSettings.RedisConn;
    options.InstanceName = redisSettings.InstanceName;
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
app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
});
app.UseSession();
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
