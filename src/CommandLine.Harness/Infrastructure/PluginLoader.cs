using System.Reflection;
using System.Runtime.Loader;
using Microsoft.Extensions.DependencyInjection;
using SandlotWizards.CommandLineParser.Core;
using System.Text.Json;

namespace CommandLine.Harness.Infrastructure;

public static class PluginLoader
{
    private static readonly string PluginRoot = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".copilot", "packages");
    private static readonly Dictionary<string, Type> RegisteredTypes = new();

    public static void RegisterPluginServices(IServiceCollection services)
    {
        if (!Directory.Exists(PluginRoot)) return;

        foreach (var (type, name) in LoadPluginCommandTypes())
        {
            services.AddScoped(type);
            RegisteredTypes[name] = type;
        }
    }

    public static void RegisterPluginRoutes(IServiceProvider provider, ICommandRegistry registry)
    {
        foreach (var kvp in RegisteredTypes)
        {
            var name = kvp.Key;
            var type = kvp.Value;

            var parts = name.Split(' ', 2);
            if (parts.Length != 2) continue;

            var noun = parts[0];
            var verb = parts[1];
            registry.Register(noun, verb, sp => (ICommand)sp.GetRequiredService(type));
        }
    }

    private static IEnumerable<(Type type, string commandName)> LoadPluginCommandTypes()
    {
        foreach (var manifestPath in Directory.EnumerateFiles(PluginRoot, "manifest.json", SearchOption.AllDirectories))
        {
            var manifestJson = File.ReadAllText(manifestPath);
            var manifest = JsonSerializer.Deserialize<PluginManifest>(manifestJson);
            if (manifest is null || string.IsNullOrEmpty(manifest.EntryAssembly)) continue;

            var pluginDir = Path.GetDirectoryName(manifestPath)!;
            var assemblyPath = Path.Combine(pluginDir, manifest.EntryAssembly);
            var context = new AssemblyLoadContext(manifest.Name, isCollectible: true);
            var assembly = context.LoadFromAssemblyPath(assemblyPath);

            foreach (var cmd in manifest.Commands)
            {
                var type = assembly.GetType(cmd.Type);
                if (type is null || !typeof(ICommand).IsAssignableFrom(type)) continue;

                yield return (type, $"{cmd.Noun} {cmd.Verb}");
            }
        }
    }
}
