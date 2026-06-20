using FluentValidation;
using Microsoft.EntityFrameworkCore;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using RoiForm.Application;
using RoiForm.Application.Features.RoiForms.Abstractions;
using RoiForm.Api.Behaviors;
using RoiForm.Api.Configuration;
using Domain.Common;
using RoiForm.Domain.FormManagement;
using RoiForm.Infrastructure.Data;
using RoiForm.Infrastructure.Data.Queries;
using RoiForm.Infrastructure.Repositories;

namespace RoiForm.Api.Extensions;

public static class ServiceCollectionExtensions
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddApplicationServices()
        {
            services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssemblyContaining<Program>();
                cfg.RegisterServicesFromAssemblyContaining<ApplicationAssemblyMarker>();
            });
            services.AddValidatorsFromAssemblyContaining<Program>();
            services.AddValidatorsFromAssemblyContaining<ApplicationAssemblyMarker>();

            services.AddExceptionHandler<GlobalExceptionHandler>();
            services.AddProblemDetails();

            services.AddHealthChecks()
                .AddDbContextCheck<AppDbContext>("database");

            return services;
        }

        public IServiceCollection AddInfrastructureServices(IConfiguration configuration)
        {
            var features = configuration.GetSection("Features").Get<FeatureFlags>() ?? new FeatureFlags();

            services.AddDbContext<AppDbContext>(options =>
            {
                if (features.UseInMemoryDatabase)
                    options.UseInMemoryDatabase("roi_calculator");
                else
                    options.UseNpgsql(configuration.GetConnectionString("DefaultConnection"));
            });

            if (features.UseInMemoryDatabase)
            {
                services.AddScoped<IFormTemplateReadQueries, EfCoreFormTemplateReadQueries>();
            }
            else
            {
                var connStr = configuration.GetConnectionString("DefaultConnection")!;
                services.AddSingleton<IDbConnectionFactory>(new NpgsqlConnectionFactory(connStr));
                services.AddScoped<IFormTemplateReadQueries, DapperFormTemplateReadQueries>();
            }

            services.AddScoped<IFormTemplateRepository, FormTemplateRepository>();
            services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<AppDbContext>());
            services.AddScoped<DatabaseSeeder>();

            return services;
        }

        public IServiceCollection AddObservability(IConfiguration configuration, IHostEnvironment environment)
        {
            var serviceName = configuration["OpenTelemetry:ServiceName"] ?? "roi-form-api";
            var serviceVersion = configuration["OpenTelemetry:ServiceVersion"] ?? "1.0.0";

            // OTEL_EXPORTER_OTLP_ENDPOINT is injected by Aspire; fall back to our config key
            var otlpEndpoint = configuration["OTEL_EXPORTER_OTLP_ENDPOINT"]
                            ?? configuration["OpenTelemetry:Otlp:Endpoint"];

            services.AddLogging(logging => logging.AddOpenTelemetry(o =>
            {
                o.IncludeFormattedMessage = true;
                o.IncludeScopes = true;
                if (!string.IsNullOrEmpty(otlpEndpoint))
                    o.AddOtlpExporter(e => e.Endpoint = new Uri(otlpEndpoint));
            }));

            services.AddOpenTelemetry()
                .ConfigureResource(r => r.AddService(serviceName, serviceVersion: serviceVersion))
                .WithTracing(tracing =>
                {
                    tracing
                        .AddAspNetCoreInstrumentation()
                        .AddHttpClientInstrumentation()
                        .AddSource("Npgsql")
                        .AddEntityFrameworkCoreInstrumentation();

                    if (environment.IsDevelopment())
                        tracing.AddConsoleExporter();

                    if (!string.IsNullOrEmpty(otlpEndpoint))
                        tracing.AddOtlpExporter(o => o.Endpoint = new Uri(otlpEndpoint));
                })
                .WithMetrics(metrics =>
                {
                    metrics
                        .AddAspNetCoreInstrumentation()
                        .AddHttpClientInstrumentation();

                    if (environment.IsDevelopment())
                        metrics.AddConsoleExporter();

                    if (!string.IsNullOrEmpty(otlpEndpoint))
                        metrics.AddOtlpExporter(o => o.Endpoint = new Uri(otlpEndpoint));
                });

            return services;
        }
    }
}
