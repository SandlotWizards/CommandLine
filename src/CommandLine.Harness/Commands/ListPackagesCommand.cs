using SandlotWizards.ActionLogger;
using SandlotWizards.CommandLineParser.Core;

namespace CommandLine.Harness.Commands;

public class ListPackagesCommand : ICommand
{
    public async Task<CommandResult?> ExecuteAsync(CommandContext context)
    {
        var root = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".copilot", "packages");
        if (!Directory.Exists(root))
        {
            ActionLog.Global.Message("No packages installed.");
            return new CommandResult
            {
                Status = "success",
                Messages = new[] { "No packages installed." },
                Data = Array.Empty<string>()
            };
        }

        var packages = Directory.GetDirectories(root)
            .Select(Path.GetFileName)
            .ToArray();

        if (packages.Length == 0)
        {
            ActionLog.Global.Message("No packages installed.");
            return new CommandResult
            {
                Status = "success",
                Messages = new[] { "No packages installed." },
                Data = Array.Empty<string>()
            };
        }

        ActionLog.Global.Message("Installed packages:");
        foreach (var dir in packages)
            ActionLog.Global.Message("- " + dir);

        return new CommandResult
        {
            Status = "success",
            Messages = new[] { "Package list retrieved." },
            Data = packages
        };
    }
}
