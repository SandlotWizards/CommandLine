﻿using System.Reflection;
using System.Threading.Tasks;
using SandlotWizards.ActionLogger;
using SandlotWizards.CommandLineParser.Execution;

namespace SandlotWizards.CommandLineParser.Commands.BuiltIn;

public class VersionCommand : IRoutableCommand
{
    public string Noun => "system";
    public string Verb => "version";
    public string Description => "Displays the current CLI version.";
    public string Group => "Core Utilities";
    public bool IsEnabled => true;
    public bool ShowInList => true;

    public Task<CommandResult?> ExecuteAsync(CommandContext context)
    {
        var toolName = Assembly.GetEntryAssembly()?.GetName().Name ?? "Unknown CLI";
        var version = Assembly.GetEntryAssembly()?.GetName().Version?.ToString() ?? "unknown";
        var versionText = $"{toolName} version {version}";
        var outputMode = context.Metadata["OutputFormat"]?.ToString();

        var displayText = context.IsDryRun ? $"[dry-run] {versionText}" : versionText;

        if (outputMode == "text")
            ActionLog.Global.Message(displayText);

        return Task.FromResult<CommandResult?>(new CommandResult
        {
            Status = "success",
            Messages = new[] { versionText } // leave raw version for output, not display
        });
    }
}
