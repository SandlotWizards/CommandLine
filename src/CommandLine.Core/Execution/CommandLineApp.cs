using SandlotWizards.ActionLogger;
using SandlotWizards.CommandLineParser.Commands.BuiltIn;
using SandlotWizards.CommandLineParser.Core;
using SandlotWizards.CommandLineParser.Help;
using SandlotWizards.CommandLineParser.IO.Output;
using SandlotWizards.CommandLineParser.Parsing;
using System;
using System.Threading.Tasks;

namespace SandlotWizards.CommandLineParser.Execution;

public static class CommandLineApp
{
    public static IHelpProvider HelpProvider { get; set; } = new DefaultHelpProvider();

    public static async Task Run(string[] args, Action<CommandRegistry> configure, IServiceProvider serviceProvider)
    {

        var registry = new CommandRegistry();

        configure(registry);

        var parser = new ContextParser();
        var context = parser.Parse(args);
        context.Metadata["ServiceProvider"] = serviceProvider;

        var outputAsJson = context.Arguments.TryGetValue("output", out var format) && format?.ToLowerInvariant() == "json";
        context.Metadata["OutputFormat"] = outputAsJson ? "json" : "text";

        var isChildProcess = Environment.GetEnvironmentVariable("IS_PASSENGER") == "1";
        var outputWriter = OutputWriterFactory.FromContext(context);

        if (context.Arguments.ContainsKey("version") && string.IsNullOrWhiteSpace(context.CommandName))
        {
            if (!isChildProcess) outputWriter.WriteHeader();
            var versionCommand = new VersionCommand();
            await versionCommand.ExecuteAsync(context);
            if (!isChildProcess) outputWriter.WriteTrailer();
            return;
        }

        try
        {
            var descriptor = registry.Resolve(context.Noun, context.Verb);
            ICommand? command = descriptor switch
            {
                IRoutableCommandDescriptor routable => routable.Resolve((IServiceProvider)context.Metadata["ServiceProvider"]),
                _ => null
            };

            if (command is null)
                throw new InvalidOperationException($"No command registered for {context.Noun} {context.Verb}.");

            if (context.Arguments.ContainsKey("help"))
            {
                context.Metadata["OutputFormat"] = "help";
                outputWriter = OutputWriterFactory.FromContext(context);
                if (!isChildProcess) outputWriter.WriteHeader();
                var helpText = await HelpProvider.GetHelpAsync(context.CommandName);
                outputWriter.WriteHelp(helpText);
                if (!isChildProcess) outputWriter.WriteTrailer();
                return;
            }

            if (!isChildProcess) outputWriter.WriteSplash(SplashRenderer.Render(context));

            if (!isChildProcess) outputWriter.WriteHeader(); 

            var result = await command.ExecuteAsync(context);

            if (!isChildProcess) outputWriter.WriteTrailer();
        }
        catch (Exception ex)
        {
            outputWriter.WriteError(ex);
            Environment.Exit(2);
        }
    }
}
