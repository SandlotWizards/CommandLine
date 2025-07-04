using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SandlotWizards.CommandLineParser.Core;
using SandlotWizards.CommandLineParser.BuiltIn;
using System.Collections.Generic;

var builder = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddSingleton<GoodbyeWorldCommand>();
    });

var app = builder.Build();

// ✅ Build the list of descriptors
var commands = new List<IRoutableCommandDescriptor>
{
    new RoutableCommandDescriptor("goodbye", "world", _ => app.Services.GetRequiredService<GoodbyeWorldCommand>())
};

// ✅ Add system describe as the last entry, passing the same list
commands.Add(new RoutableCommandDescriptor("system", "describe", _ =>
    new SystemDescribeCommand("lore-test", commands)));

await CommandLineApp.Run(args, registry =>
{
    foreach (var cmd in commands)
        registry.Register(cmd.Noun, cmd.Verb, cmd.Resolve);
}, app.Services);
