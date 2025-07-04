using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SandlotWizards.CommandLineParser.Core;
using SandlotWizards.CommandLineParser.BuiltIn;
using SandlotWizards.SoftwareFactory.Commands;
using SandlotWizards.CommandLineParser.Input;
using SandlotWizards.CommandLineParser.Registration;
using System;

var builder = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddScoped<IInteractivePromptService, ConsolePromptService>();
        services.AddSingleton<GoodbyeWorldCommand>();
        services.AddSingleton<AddConnectionProfileCommand>();
    });

var app = builder.Build();

// Load enabled commands via helper
var commands = CommandRegistrationHelper.LoadCommands(app.Services,
    app.Services.GetRequiredService<GoodbyeWorldCommand>(),
    app.Services.GetRequiredService<AddConnectionProfileCommand>()
);

// Inject dynamic system descriptor
commands.Add(new RoutableCommandDescriptor("system", "describe", _ =>
    new SystemDescribeCommand("lore-test", commands)));

commands.Add(new RoutableCommandDescriptor("system", "list", _ =>
    new SystemListCommand(commands)));

foreach (var cmd in commands)
{
    Console.WriteLine($"[debug] {cmd.Noun} {cmd.Verb} - ShowInList: {cmd.ShowInList}");
}

await CommandLineApp.Run(args, registry =>
{
    registry.RegisterAll(commands);
}, app.Services);
