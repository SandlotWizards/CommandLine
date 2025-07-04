using System.Threading.Tasks;
using SandlotWizards.ActionLogger;
using SandlotWizards.CommandLineParser.Core;

namespace SandlotWizards.CommandLineParser.BuiltIn;

public class VersionCommand : IRoutableCommand
{
    public string Noun => "core";
    public string Verb => "version";
    public string Description => "Displays the current CLI version.";
    public string Group => "Core Utilities";
    public bool IsEnabled => true;
    public bool ShowInList => true;

    public Task<CommandResult?> ExecuteAsync(CommandContext context)
    {
        var versionText = "Lore CLI version 1.0.0";
        var outputMode = context.Metadata["OutputFormat"]?.ToString();

        if (outputMode == "text")
            ActionLog.Global.Message(versionText);

        return Task.FromResult<CommandResult?>(new CommandResult
        {
            Status = "success",
            Messages = new[] { versionText }
        });
    }
}
