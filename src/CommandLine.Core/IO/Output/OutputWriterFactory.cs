using SandlotWizards.CommandLineParser.Execution;

namespace SandlotWizards.CommandLineParser.IO.Output
{
    public static class OutputWriterFactory
    {
        public static IOutputWriter FromContext(CommandContext context)
        {
            var format = context.Metadata.TryGetValue("OutputFormat", out var modeObj)
                ? modeObj?.ToString()?.ToLowerInvariant()
                : "text";

            return format switch
            {
                "json" => new JsonOutputWriter(),
                "help" => new HelpOutputWriter(),
                _ => new TextOutputWriter()
            };
        }
    }

}
