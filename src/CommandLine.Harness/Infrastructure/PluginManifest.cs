namespace CommandLine.Harness.Infrastructure
{
    public record PluginManifest(string name, string entryAssembly, List<PluginCommand> commands);
}
