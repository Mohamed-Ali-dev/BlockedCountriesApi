using Core.Helper;
using Core.Services.Interfaces;
using Core.Services.Implementation;
using Microsoft.Extensions.Options;
using Infrastructure.Repository.IRepository;



var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.Configure<IpApiServiceSettings>(builder.Configuration.GetSection("IpApi"));
builder.Services.AddHttpClient<IGeolocationService, GeolocationService>(
    (serviceProvider, client) =>
    {
        var settings = serviceProvider
            .GetRequiredService<IOptions<IpApiServiceSettings>>().Value;
        client.BaseAddress = new Uri(settings.BaseUrl);
    });

builder.Services.AddScoped<IBlockedCountryRepository>

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
