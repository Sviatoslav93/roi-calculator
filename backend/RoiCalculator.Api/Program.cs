using System.Text.Json.Serialization;
using RoiCalculator.Api.Extensions;
using RoiCalculator.Api.Features.RoiForms;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddCors(options =>
    options.AddPolicy("LocalDev", policy =>
        policy.WithOrigins("http://localhost:5173")
              .AllowAnyHeader()
              .AllowAnyMethod()));
builder.Services.ConfigureHttpJsonOptions(options =>
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter()));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseCors("LocalDev");
app.UseExceptionHandler();
app.UseHttpsRedirection();

app.MapRoiFormEndpoints();

app.Run();
