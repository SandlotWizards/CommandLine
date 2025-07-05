using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SandlotWizards.CommandLineParser.BuiltIn;
using SandlotWizards.CommandLineParser.Core;
using SandlotWizards.CommandLineParser.Helper;
using SandlotWizards.CommandLineParser.Models;
using SandlotWizards.CommandLineParser.Services;
using Serilog;
using System.Reflection;

var isPassenger = Environment.GetEnvironmentVariable("IS_PASSENGER") == "1";

var builder = Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration((hostingContext, config) =>
    {
        config.AddUserSecrets<Program>(optional: true);
    })
    .ConfigureServices((hostContext, services) =>
    {
        using var tempProvider = services.BuildServiceProvider();

        var commandTypes = Assembly.GetExecutingAssembly()
                                   .GetTypes()
                                   .Where(t => typeof(IRoutableCommand).IsAssignableFrom(t) &&
                                               !t.IsAbstract &&
                                               t.GetCustomAttribute<ExcludeFromCommandDiscoveryAttribute>() is null);
        foreach (var type in commandTypes)
        {
            var instance = ActivatorUtilities.CreateInstance(tempProvider, type) as IRoutableCommand;
            if (instance?.IsEnabled == true) services.AddSingleton(typeof(IRoutableCommand), type);
        }

        services.AddSingleton<VersionCommand>();
        if (isPassenger) services.AddSingleton<SystemDescribeCommand>();
        if (!isPassenger) services.AddSingleton<PassengerDiscoveryService>();
    })
    .UseSerilog((context, services, configuration) =>
    {
        //configuration.WriteTo.Console();
    });

var app = builder.Build();

var logger = app.Services.GetRequiredService<ILogger<Program>>();
logger.LogInformation("Starting SandlotWizard Lore Copilot...");

List<PassengerManifest> passengers = new();

if (!isPassenger)
{
    // 🚀 Discover passenger plugins
    var discovery = app.Services.GetRequiredService<PassengerDiscoveryService>();
    passengers = await discovery.DiscoverAsync();
    logger.LogInformation("Discovered {Count} passenger(s)", passengers.Count);
}

await CommandLineApp.Run(args, registry =>
{
    if (!isPassenger)
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
    }

    var localCommands = CommandDiscoveryService.GetEnabledLocalCommands(app.Services)
                                        .Select(c => new RoutableCommandDescriptor(c))
                                        .ToList();
    registry.RegisterAll(localCommands);

    registry.Register(new RoutableCommandDescriptor(new VersionCommand()));

    var fullCommandList = registry.GetAll()
        .OfType<IRoutableCommandDescriptor>()
        .Where(c => c.IsEnabled)
        .ToList();

    if (isPassenger)
    {
        var toolName = Path.GetFileNameWithoutExtension(Assembly.GetEntryAssembly()?.Location ?? "unknown");
        var systemDescribe = new SystemDescribeCommand(toolName, fullCommandList);
        registry.Register(new RoutableCommandDescriptor(systemDescribe));
    }

    if (!isPassenger)
    {
        var systemList = new SystemListCommand(fullCommandList);
        fullCommandList.Add(new RoutableCommandDescriptor(systemList));
        registry.Register(new RoutableCommandDescriptor(systemList));
    }
}, app.Services);
