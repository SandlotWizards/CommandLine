namespace CommandLine.Harness.Infrastructure
{
    public record PluginManifest(string Name, string EntryAssembly, List<PluginCommand> Commands);
}
