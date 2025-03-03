using Core.Helper;
using Core.Services.Interfaces;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

//var ipApiconfig = builder.Configuration.GetSection("IpApi").Get<IpApiServiceSettings>();
//builder.Services.AddSingleton(ipApiconfig);

builder.Services.Configure<IpApiServiceSettings>(builder.Configuration.GetSection("IpApi"));
builder.Services.AddHttpClient<IGeolocationService, GeolocationService>(
    (serviceProvider, client) =>
    {
        var settings = serviceProvider
            .GetRequiredService<IOptions<IpApiServiceSettings>>()
            .Value;
        client.BaseAddress = new Uri(settings.BaseUrl);
    });
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
