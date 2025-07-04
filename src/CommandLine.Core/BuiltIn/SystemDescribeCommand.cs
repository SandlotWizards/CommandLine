using SandlotWizards.CommandLineParser.Core;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SandlotWizards.CommandLineParser.BuiltIn;

public class SystemDescribeCommand : IRoutableCommand
{
    private readonly string _toolName;
    private readonly IEnumerable<IRoutableCommandDescriptor> _commands;

    public SystemDescribeCommand(string toolName, IEnumerable<IRoutableCommandDescriptor> commands)
    {
        _toolName = toolName;
        _commands = commands;
    }

    public string Noun => "system";
    public string Verb => "describe";
    public string? Description => "Describes available commands for plugin discovery.";
    public string? Group => "Core Utilities";
    public bool IsEnabled => true;
    public bool ShowInList => false; // Hidden from system list

    public Task<CommandResult?> ExecuteAsync(CommandContext context)
    {
        var entries = _commands
            .Where(c => c.IsEnabled && !(c.Noun == "system" && c.Verb == "describe"))
            .Select(c => new
            {
                noun = c.Noun,
                verb = c.Verb,
                description = c.Description ?? "No description provided",
                group = c.Group ?? "General"
            });

        var manifest = new
        {
            name = _toolName,
            type = "plugin",
            entryPoint = _toolName, // assumes tool command = tool name
            commands = entries
        };

        return Task.FromResult<CommandResult?>(new CommandResult
        {
            Status = "success",
            Messages = new[] { "System description generated." },
            Data = manifest
        });
    }
}
