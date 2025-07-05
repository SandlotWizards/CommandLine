using SandlotWizards.ActionLogger;
using SandlotWizards.CommandLineParser.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace SandlotWizards.CommandLineParser.BuiltIn;

[ExcludeFromCommandDiscovery]
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
            .Where(c => c.IsEnabled && !(c.Noun == "system" && c.Verb == "describe") && !(c.Noun == "system" && c.Verb == "list") && !(c.Noun == "core" && c.Verb == "version"))
            .Select(c => new
            {
                noun = c.Noun,
                verb = c.Verb,
                description = c.Description ?? "No description provided",
                group = c.Group ?? "General",
                isEnabled = c.IsEnabled,
                showInList = c.ShowInList
            });

        var manifest = new
        {
            name = _toolName,
            type = "plugin",
            entryPoint = _toolName, // assumes tool command = tool name
            commands = entries
        };

        var outputMode = context.Metadata["OutputFormat"]?.ToString();

        if (outputMode == "json")
        {
            var json = JsonSerializer.Serialize(manifest, new JsonSerializerOptions
            {
                WriteIndented = true
            });

            Console.WriteLine(json); // ✅ plugin discovery output to stdout
            return Task.FromResult<CommandResult?>(null); // prevents OutputWriter from repeating it
        }
        
        if (outputMode == "test")
            ActionLog.Global.Message(JsonSerializer.Serialize(manifest));

        return Task.FromResult<CommandResult?>(new CommandResult
        {
            Status = "success",
            Messages = new[] { "System description generated." },
            Data = manifest
        });
    }
}
