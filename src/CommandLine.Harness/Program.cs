using CommandLine.Harness.Commands;
using CommandLine.Harness.Extensions;
using CommandLine.Harness.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SandlotWizards.CommandLineParser.Core;
using Serilog;

var builder = Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration((hostingContext, config) =>
    {
        config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
        config.AddUserSecrets<Program>(optional: true);
    })
    .ConfigureServices((hostContext, services) =>
    {
        services.AddApplicationServices(hostContext.Configuration);
        PluginLoader.RegisterPluginServices(services);
    })
    .UseSerilog((context, services, configuration) =>
    {
        //configuration.WriteTo.Console();
    });

var app = builder.Build();
var logger = app.Services.GetRequiredService<ILogger<Program>>();
logger.LogInformation("Starting SandlotWizard CLI...");

await CommandLineApp.Run(args, registry =>
{
    registry.Register("system", "hello", app.Services.GetRequiredService<HelloCommand>());
    registry.Register("package", "add", app.Services.GetRequiredService<AddPackageCommand>());
    registry.Register("package", "list", app.Services.GetRequiredService<ListPackagesCommand>());
    registry.Register("package", "remove", app.Services.GetRequiredService<RemovePackageCommand>());
    PluginLoader.RegisterPluginRoutes(app.Services, registry);
}, app.Services);