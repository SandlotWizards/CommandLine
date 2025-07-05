using SandlotWizards.ActionLogger;
using SandlotWizards.CommandLineParser.Execution;
using System.Threading.Tasks;

namespace CommandLine.Harness.Commands;

public class HelloCommand : IRoutableCommand
{
    public string Noun => "system";
    public string Verb => "hello";
    public string Description => "Says hello to the specified name or 'world' by default.";
    public string Group => "Core Utilities";
    public bool IsEnabled => true;
    public bool ShowInList => true;

    public Task<CommandResult?> ExecuteAsync(CommandContext context)
    {
        var name = context.Arguments.TryGetValue("name", out var val) ? val : "world";
        var message = $"Hello, {name}!";

        var displayText = context.IsDryRun ? $"[dry-run] {message}" : message;
        ActionLog.Global.Message(displayText);

        return Task.FromResult<CommandResult?>(new CommandResult
        {
            Status = "success",
            Messages = new[] { message }
        });
    }
}
