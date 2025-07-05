using SandlotWizards.CommandLineParser.Execution;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SandlotWizards.CommandLineParser.Helper;

public static class CommandRegistrationHelper
{
    public static List<IRoutableCommandDescriptor> LoadCommands(IServiceProvider services, params IRoutableCommand[] manualCommands)
    {
        return [.. manualCommands
            .Where(c => c.IsEnabled)
            .Select(c => (IRoutableCommandDescriptor)new RoutableCommandDescriptor(c))];
    }

    public static void RegisterAll(this CommandRegistry registry, IEnumerable<IRoutableCommandDescriptor> commands)
    {
        foreach (var cmd in commands)
            registry.Register(cmd);
    }
}
