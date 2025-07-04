using SandlotWizards.ActionLogger;
using SandlotWizards.CommandLineParser.Core;
using System.Threading.Tasks;

public class GoodbyeWorldCommand : IRoutableCommand
{
    public string Noun => "goodbye";
    public string Verb => "world";
    public string Description => "Says goodbye to the specified name or 'world' by default.";
    public string Group => "Demo Commands";
    public bool IsEnabled => true;
    public bool ShowInList => true;

    public Task<CommandResult?> ExecuteAsync(CommandContext context)
    {
        var name = context.Arguments.TryGetValue("name", out var val) ? val : "world";
        var message = $"Goodbye, {name}!";
        ActionLog.Global.Message(message);

        return Task.FromResult<CommandResult?>(new CommandResult
        {
            Status = "success",
            Messages = new[] { message }
        });
    }
}
