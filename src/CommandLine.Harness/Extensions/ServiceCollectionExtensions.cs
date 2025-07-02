using CommandLine.Harness.Commands;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CommandLine.Harness.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<HelloCommand>();
        services.AddScoped<ListPackagesCommand>();
        services.AddScoped<AddPackageCommand>();
        services.AddScoped<RemovePackageCommand>();
        return services;
    }
}
