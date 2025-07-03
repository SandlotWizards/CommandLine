using SandlotWizards.CommandLineParser.Core;

namespace CommandLine.Harness.Commands;

public class ListPackagesCommand : ICommand
{
    public async Task<CommandResult?> ExecuteAsync(CommandContext context)
    {
        var root = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".copilot", "packages");
        if (!Directory.Exists(root))
        {
            Console.WriteLine("No packages installed.");
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
            Console.WriteLine("No packages installed.");
            return new CommandResult
            {
                Status = "success",
                Messages = new[] { "No packages installed." },
                Data = Array.Empty<string>()
            };
        }

        Console.WriteLine("Installed packages:");
        foreach (var dir in packages)
            Console.WriteLine("- " + dir);

        return new CommandResult
        {
            Status = "success",
            Messages = new[] { "Package list retrieved." },
            Data = packages
        };
    }
}
