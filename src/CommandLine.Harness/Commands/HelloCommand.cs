using SandlotWizards.ActionLogger;
using SandlotWizards.CommandLineParser.Core;

namespace CommandLine.Harness.Commands;

public class HelloCommand : IRoutableCommand
{
    public string Noun => "system";
    public string Verb => "hello";
    public string Description => "Checks that the Copilot CLI is running and dispatching commands correctly.";
    public string Group => "System Diagnostics";
    public bool IsEnabled => true;
    public bool ShowInList => true;

    public async Task<CommandResult?> ExecuteAsync(CommandContext context)
    {
        var message = "Copilot CLI is running and command dispatch works!";
        ActionLog.Global.Message(message);

        return await Task.FromResult(new CommandResult
        {
            Status = "success",
            Messages = new[] { message }
        });
    }
}
