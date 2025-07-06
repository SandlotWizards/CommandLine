using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SandlotWizards.ActionLogger;
using SandlotWizards.ActionLogger.Services;
using SandlotWizards.CommandLineParser.Commands.BuiltIn;
using SandlotWizards.CommandLineParser.Execution;
using SandlotWizards.CommandLineParser.Helper;
using SandlotWizards.CommandLineParser.IO.Input;
using SandlotWizards.CommandLineParser.Models;
using SandlotWizards.CommandLineParser.Services;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;


Environment.SetEnvironmentVariable("TOOL_NAME", "lore-test");
Environment.SetEnvironmentVariable("IS_PASSENGER", "1");

var isPassenger = Environment.GetEnvironmentVariable("IS_PASSENGER") == "1";

if (!ActionLog.IsInitialized)
{
    var defaultLogger = new ActionLoggerService();
    ActionLog.Initialize(defaultLogger);
}

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
        services.AddScoped<IInteractivePromptService, ConsolePromptService>();
        if (isPassenger) services.AddSingleton<SystemDescribeCommand>();
        if (!isPassenger) services.AddSingleton<PassengerDiscoveryService>();
    })
    .UseSerilog((context, services, configuration) =>
    {
        //configuration.WriteTo.Console();
    });

var app = builder.Build();

List<PassengerManifest> passengers = new();

if (!isPassenger)
{
    // 🚀 Discover passenger plugins
    var discovery = app.Services.GetRequiredService<PassengerDiscoveryService>();
    passengers = await discovery.DiscoverAsync();
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
                    cmd.Verb,
                    cmd.Description,
                    cmd.Group,
                    cmd.IsEnabled,
                    cmd.ShowInList
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
        var toolName = Environment.GetEnvironmentVariable("TOOL_NAME") ?? "unknown";
        var systemDescribe = new SystemDescribeCommand(toolName, fullCommandList);
        registry.Register(new RoutableCommandDescriptor(systemDescribe));
    }

    var systemList = new SystemListCommand(fullCommandList);
    fullCommandList.Add(new RoutableCommandDescriptor(systemList));
    registry.Register(new RoutableCommandDescriptor(systemList));

}, app.Services);
