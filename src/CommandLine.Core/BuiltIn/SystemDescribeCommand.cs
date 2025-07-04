using SandlotWizards.CommandLineParser.Core;
using System.Collections.Generic;
using System;
using System.Text.Json;
using System.Threading.Tasks;
using System.Linq;

namespace SandlotWizards.CommandLineParser.BuiltIn;

public class SystemDescribeCommand : ICommand
{
    private readonly string _toolName;
    private readonly IEnumerable<IRoutableCommandDescriptor> _commands;

    public SystemDescribeCommand(string toolName, IEnumerable<IRoutableCommandDescriptor> commands)
    {
        _toolName = toolName;
        _commands = commands;
    }

    public Task<CommandResult?> ExecuteAsync(CommandContext context)
    {
        var entries = _commands
            .Where(c => !(c.Noun == "system" && c.Verb == "describe")) // exclude self
            .Select(c => new
            {
                noun = c.Noun,
                verb = c.Verb,
                description = "No description provided"
            });

        var manifest = new
        {
            name = _toolName,
            type = "plugin",
            entryPoint = _toolName, // assumes tool command = tool name
            commands = entries
        };

        var json = JsonSerializer.Serialize(manifest, new JsonSerializerOptions
        {
            WriteIndented = true
        });

        return Task.FromResult<CommandResult?>(new CommandResult
        {
            Status = "success",
            Messages = new[] { "System description generated." },
            Data = manifest
        });
    }
}
