using SandlotWizards.ActionLogger;
using SandlotWizards.CommandLineParser.Core;

namespace CommandLine.Harness.Commands;

public class RemovePackageCommand : ICommand
{
    public async Task<CommandResult?> ExecuteAsync(CommandContext context)
    {
        var packageId = context.Arguments.GetValueOrDefault("name");
        if (string.IsNullOrWhiteSpace(packageId))
        {
            ActionLog.Global.Message("--name argument is required.");
            return new CommandResult
            {
                Status = "error",
                Messages = new[] { "--name argument is required." }
            };
        }

        var path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".copilot", "packages", packageId.ToLowerInvariant());
        if (Directory.Exists(path))
        {
            Directory.Delete(path, true);
            ActionLog.Global.Message($"Package '{packageId}' removed.");
            return new CommandResult
            {
                Status = "success",
                Messages = new[] { $"Package '{packageId}' removed." },
                Data = new { package = packageId }
            };
        }
        else
        {
            ActionLog.Global.Message($"Package '{packageId}' not found.");
            return new CommandResult
            {
                Status = "success",
                Messages = new[] { $"Package '{packageId}' not found." },
                Data = new { package = packageId }
            };
        }
    }
}
