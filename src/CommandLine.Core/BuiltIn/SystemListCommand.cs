using SandlotWizards.ActionLogger;
using SandlotWizards.CommandLineParser.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace SandlotWizards.CommandLineParser.BuiltIn;

public class SystemListCommand : IRoutableCommand
{
    private readonly IEnumerable<IRoutableCommandDescriptor> _commands;

    public SystemListCommand(IEnumerable<IRoutableCommandDescriptor> commands)
    {
        _commands = commands;
    }

    public string Noun => "system";
    public string Verb => "list";
    public string? Description => "Lists available commands grouped by their logical category.";
    public string? Group => "Core Utilities";
    public bool IsEnabled => true;
    public bool ShowInList => true;

    public Task<CommandResult?> ExecuteAsync(CommandContext context)
    {
        var outputFormat = context.Metadata["OutputFormat"]?.ToString()?.ToLowerInvariant() ?? "text";

        var visibleCommands = _commands
            .Where(c => c.IsEnabled && c.ShowInList)
            .GroupBy(c => c.Group ?? "General")
            .OrderBy(g => g.Key);

        if (outputFormat == "json")
        {
            var jsonOutput = visibleCommands.ToDictionary(
                g => g.Key,
                g => g.Select(c => new
                {
                    noun = c.Noun,
                    verb = c.Verb,
                    description = string.IsNullOrWhiteSpace(c.Description) ? "No description provided." : c.Description
                })
            );

            return Task.FromResult<CommandResult?>(new CommandResult
            {
                Status = "success",
                Data = jsonOutput
            });
        }
        else
        {
            foreach (var group in visibleCommands)
            {
                ActionLog.Global.Message(group.Key);
                foreach (var cmd in group.OrderBy(c => c.Noun).ThenBy(c => c.Verb))
                {
                    var label = $"  {cmd.Noun} {cmd.Verb}".PadRight(30);
                    var description = string.IsNullOrWhiteSpace(cmd.Description) ? "No description provided." : cmd.Description;
                    ActionLog.Global.Message($"{label} {description}");
                }
            }

            return Task.FromResult<CommandResult?>(new CommandResult
            {
                Status = "success",
                Messages = new[] { "Commands listed successfully." }
            });
        }
    }
}
