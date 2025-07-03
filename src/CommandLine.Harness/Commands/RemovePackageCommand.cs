using SandlotWizards.CommandLineParser.Core;

namespace CommandLine.Harness.Commands;

public class RemovePackageCommand : ICommand
{
    public async Task<CommandResult?> ExecuteAsync(CommandContext context)
    {
        var packageId = context.Arguments.GetValueOrDefault("name");
        if (string.IsNullOrWhiteSpace(packageId))
        {
            Console.WriteLine("--name argument is required.");
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
            Console.WriteLine($"Package '{packageId}' removed.");
            return new CommandResult
            {
                Status = "success",
                Messages = new[] { $"Package '{packageId}' removed." },
                Data = new { package = packageId }
            };
        }
        else
        {
            Console.WriteLine($"Package '{packageId}' not found.");
            return new CommandResult
            {
                Status = "success",
                Messages = new[] { $"Package '{packageId}' not found." },
                Data = new { package = packageId }
            };
        }
    }
}
