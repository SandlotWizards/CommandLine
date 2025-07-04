using CommandLine.Harness.Commands;
using CommandLine.Harness.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SandlotWizards.CommandLineParser.BuiltIn;
using SandlotWizards.CommandLineParser.Core;
using SandlotWizards.CommandLineParser.Services;
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
        services.AddSingleton<PassengerDiscoveryService>();
    })
    .UseSerilog((context, services, configuration) =>
    {
        //configuration.WriteTo.Console();
    });

var app = builder.Build();

var logger = app.Services.GetRequiredService<ILogger<Program>>();
logger.LogInformation("Starting SandlotWizard CLI...");

// 🚀 Trigger passenger discovery at startup
var discovery = app.Services.GetRequiredService<PassengerDiscoveryService>();
var passengers = await discovery.DiscoverAsync();
logger.LogInformation("Discovered {Count} passenger(s)", passengers.Count);

await CommandLineApp.Run(args, registry =>
{
    foreach (var passenger in passengers)
    {
        foreach (var cmd in passenger.Commands)
        {
            registry.Register(cmd.Noun, cmd.Verb, new ShellForwardCommand(
                passenger.EntryPoint,
                cmd.Noun,
                cmd.Verb
            ));
        }
    }
    registry.Register("system", "hello", app.Services.GetRequiredService<HelloCommand>());
    registry.Register("package", "add", app.Services.GetRequiredService<AddPackageCommand>());
    registry.Register("package", "list", app.Services.GetRequiredService<ListPackagesCommand>());
    registry.Register("package", "remove", app.Services.GetRequiredService<RemovePackageCommand>());
}, app.Services);
