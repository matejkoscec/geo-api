using System.Threading.RateLimiting;
using FluentValidation;
using geo_api.Features.Location;
using geo_api.Infrastructure;
using geo_api.Infrastructure.Persistence;
using geo_api.Infrastructure.Service;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSignalR();

builder.Services.AddResiliencePipelines();
builder.Services.AddHttpClients(builder.Configuration);

builder.Services.AddValidatorsFromAssemblyContaining<Program>();

builder.Services.AddMediatR(config => config.RegisterServicesFromAssemblyContaining<Program>());

builder.Services.AddDbContext<GeoApiContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString(nameof(GeoApiContext)));
});

builder.Services.AddSingleton<IGooglePlacesApiService, GooglePlacesApiNew>();

builder.Services.AddRateLimiter(rateLimiterOptions =>
{
    rateLimiterOptions.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

    rateLimiterOptions.AddConcurrencyLimiter("concurrent",
        options =>
        {
            options.PermitLimit = 1;
            options.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseRateLimiter();

app.MapLocationEndpoints();
app.MapLocationHubs();

app.Run();