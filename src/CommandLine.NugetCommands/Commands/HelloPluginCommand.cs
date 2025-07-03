using SandlotWizards.CommandLineParser.Core;

namespace CommandLine.NugetCommands.Commands;

public class HelloPluginCommand : ICommand
{
    public async Task<CommandResult?> ExecuteAsync(CommandContext context)
    {
        var name = context.Arguments.TryGetValue("name", out var val) ? val : "plugin world";
        var message = $"Nuget Hello from plugin, {name}!";
        Console.WriteLine(message);

        return await Task.FromResult(new CommandResult
        {
            Status = "success",
            Messages = new[] { message }
        });
    }
}
