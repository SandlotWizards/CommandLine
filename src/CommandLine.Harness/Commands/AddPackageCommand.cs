using NuGet.Configuration;
using NuGet.Protocol.Core.Types;
using NuGet.Versioning;
using SandlotWizards.CommandLineParser.Core;
using System.IO.Compression;

namespace CommandLine.Harness.Commands;

public class AddPackageCommand : ICommand
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

        var versionArg = context.Arguments.GetValueOrDefault("version");
        NuGetVersion? version = null;
        if (!string.IsNullOrWhiteSpace(versionArg))
            version = NuGetVersion.Parse(versionArg);

        var settings = Settings.LoadDefaultSettings(root: null);
        var sourceRepositoryProvider = new SourceRepositoryProvider(new PackageSourceProvider(settings), Repository.Provider.GetCoreV3());
        var sourceRepository = sourceRepositoryProvider.CreateRepository(new PackageSource("https://api.nuget.org/v3/index.json"));

        var resource = await sourceRepository.GetResourceAsync<FindPackageByIdResource>();
        using var cache = new SourceCacheContext { NoCache = true, DirectDownload = true };
        var logger = NuGet.Common.NullLogger.Instance;

        var versions = await resource.GetAllVersionsAsync(packageId, cache, logger, CancellationToken.None);
        var resolvedVersion = version ?? versions.Max();

        var nupkgPath = Path.Combine(Path.GetTempPath(), $"{packageId}.{resolvedVersion}.nupkg");
        using (var packageStream = File.Create(nupkgPath))
        {
            var success = await resource.CopyNupkgToStreamAsync(
                packageId,
                resolvedVersion,
                packageStream,
                cache,
                logger,
                CancellationToken.None);

            if (!success)
            {
                Console.WriteLine("Failed to download package.");
                return new CommandResult
                {
                    Status = "error",
                    Messages = new[] { "Failed to download package." }
                };
            }
        }

        var extractPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".copilot", "packages", packageId.ToLowerInvariant());
        if (Directory.Exists(extractPath)) Directory.Delete(extractPath, true);
        Directory.CreateDirectory(extractPath);

        ZipFile.ExtractToDirectory(nupkgPath, extractPath);

        Console.WriteLine($"Package '{packageId}' installed to {extractPath}");
        return new CommandResult
        {
            Status = "success",
            Messages = new[] { $"Package '{packageId}' installed to {extractPath}" },
            Data = new { package = packageId, path = extractPath }
        };
    }
}
