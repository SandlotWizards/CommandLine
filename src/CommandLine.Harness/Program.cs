using CommandLine.Harness.Commands;
using CommandLine.Harness.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SandlotWizards.CommandLineParser.BuiltIn;
using SandlotWizards.CommandLineParser.Core;
using SandlotWizards.CommandLineParser.Registration;
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

// 🚀 Discover passenger plugins
var discovery = app.Services.GetRequiredService<PassengerDiscoveryService>();
var passengers = await discovery.DiscoverAsync();
logger.LogInformation("Discovered {Count} passenger(s)", passengers.Count);

await CommandLineApp.Run(args, registry =>
{
    // 🔌 Register passenger CLI commands (forwarded)
    foreach (var passenger in passengers)
    {
        foreach (var cmd in passenger.Commands)
        {
            registry.Register(new RoutableCommandDescriptor(new ShellForwardCommand(
                passenger.EntryPoint,
                cmd.Noun,
                cmd.Verb
            )));
        }
    }

    var localCommands = CommandRegistrationHelper.LoadCommands(app.Services,
        new HelloCommand(),
        new VersionCommand()
    );
    registry.RegisterAll(localCommands);

    var fullCommandList = registry.GetAll()
        .OfType<IRoutableCommandDescriptor>()
        .Where(c => c.IsEnabled)
        .ToList();

    var systemList = new SystemListCommand(fullCommandList);
    fullCommandList.Add(new RoutableCommandDescriptor(systemList));
    var systemDescribe = new SystemDescribeCommand("copilot", fullCommandList);
    registry.Register(new RoutableCommandDescriptor(systemList));
    registry.Register(new RoutableCommandDescriptor(systemDescribe));
}, app.Services);
