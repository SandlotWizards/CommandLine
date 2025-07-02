using SandlotWizards.CommandLineParser.Core;

namespace CommandLine.Harness.Commands;

public class RemovePackageCommand : ICommand
{
    public async Task ExecuteAsync(CommandContext context)
    {
        var packageId = context.Arguments.GetValueOrDefault("name");
        if (string.IsNullOrWhiteSpace(packageId))
        {
            Console.WriteLine("--name argument is required.");
            return;
        }

        var path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".copilot", "packages", packageId.ToLowerInvariant());
        if (Directory.Exists(path))
        {
            Directory.Delete(path, true);
            Console.WriteLine($"Package '{packageId}' removed.");
        }
        else
        {
            Console.WriteLine($"Package '{packageId}' not found.");
        }

        await Task.CompletedTask;
    }
}

