using Microsoft.Extensions.DependencyInjection;
using SandlotWizards.CommandLineParser.Commands.BuiltIn;
using SandlotWizards.CommandLineParser.Execution;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

public static class CommandDiscoveryService
{
    public static List<IRoutableCommand> GetEnabledLocalCommands(IServiceProvider services)
    {
        var assembly = Assembly.GetExecutingAssembly();

        return assembly
            .GetTypes()
            .Where(t =>
                !t.IsAbstract &&
                typeof(IRoutableCommand).IsAssignableFrom(t) &&
                t.GetCustomAttribute<ExcludeFromCommandDiscoveryAttribute>() is null
            )
            .Select(t => ActivatorUtilities.CreateInstance(services, t) as IRoutableCommand)
            .Where(cmd => cmd is { IsEnabled: true })
            .ToList()!;
    }
}
