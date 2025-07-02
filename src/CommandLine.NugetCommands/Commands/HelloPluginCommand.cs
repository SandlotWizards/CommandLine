using SandlotWizards.CommandLineParser.Core;

namespace CommandLine.NugetCommands.Commands;

public class HelloPluginCommand : ICommand
{
    public async Task ExecuteAsync(CommandContext context)
    {
        var name = context.Arguments.TryGetValue("name", out var val) ? val : "plugin world";
        Console.WriteLine($"Nuget Hello from plugin, {name}!");
        await Task.CompletedTask;
    }
}
