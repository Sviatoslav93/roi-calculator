using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using RoiCalculator.Api.Behaviors;
using RoiCalculator.Api.Configuration;
using RoiCalculator.Core.Aggregates;
using RoiCalculator.Core.Common;
using RoiCalculator.Infrastructure.Data;
using RoiCalculator.Infrastructure.Repositories;

namespace RoiCalculator.Api.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssemblyContaining<Program>();
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        });

        services.AddValidatorsFromAssemblyContaining<Program>();
        services.AddExceptionHandler<GlobalExceptionHandler>();
        services.AddProblemDetails();

        return services;
    }

    public static IServiceCollection AddInfrastructureServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var features = configuration.GetSection("Features").Get<FeatureFlags>() ?? new FeatureFlags();

        services.AddDbContext<AppDbContext>(options =>
        {
            if (features.UseInMemoryDatabase)
                options.UseInMemoryDatabase("roi_calculator");
            else
                options.UseNpgsql(configuration.GetConnectionString("DefaultConnection"));
        });

        services.AddScoped<IRoiFormRepository, RoiFormRepository>();
        services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<AppDbContext>());

        return services;
    }
}
