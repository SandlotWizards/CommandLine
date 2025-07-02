using SandlotWizards.CommandLineParser.Core;

namespace CommandLine.Harness.Commands;

public class ListPackagesCommand : ICommand
{
    public async Task ExecuteAsync(CommandContext context)
    {
        var root = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".copilot", "packages");
        if (!Directory.Exists(root))
        {
            Console.WriteLine("No packages installed.");
            return;
        }

        var packages = Directory.GetDirectories(root);
        if (packages.Length == 0)
        {
            Console.WriteLine("No packages installed.");
        }
        else
        {
            Console.WriteLine("Installed packages:");
            foreach (var dir in packages)
                Console.WriteLine("- " + Path.GetFileName(dir));
        }

        await Task.CompletedTask;
    }
}

