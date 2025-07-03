using SandlotWizards.ActionLogger;
using SandlotWizards.ActionLogger.Services;
using SandlotWizards.CommandLineParser.Help;
using SandlotWizards.CommandLineParser.Parsing;
using System;
using System.Text.Json;
using System.Threading.Tasks;

namespace SandlotWizards.CommandLineParser.Core;

public static class CommandLineApp
{
    public static IHelpProvider HelpProvider { get; set; } = new DefaultHelpProvider();

    public static async Task Run(string[] args, Action<CommandRegistry> configure, IServiceProvider serviceProvider)
    {
        if (!ActionLog.IsInitialized)
        {
            var defaultLogger = new ActionLoggerService();
            ActionLog.Initialize(defaultLogger);
        }

        var registry = new CommandRegistry();
        registry.Register("core", "greet", new BuiltIn.GreetCommand());
        registry.Register("core", "version", new BuiltIn.VersionCommand());

        configure(registry);
        var parser = new ContextParser();
        var context = parser.Parse(args);
        context.Metadata["ServiceProvider"] = serviceProvider;

        var outputAsJson = context.Arguments.TryGetValue("output", out var format) && format?.ToLowerInvariant() == "json";
        context.Metadata["OutputFormat"] = outputAsJson ? "json" : "text";

        ActionLog.Global.PrintHeader(ConsoleColor.Cyan);

        if (string.IsNullOrWhiteSpace(context.CommandName))
        {
            if (context.Arguments.ContainsKey("version"))
            {
                ActionLog.Global.Message("lore CLI version 1.0.0", ConsoleColor.Gray);
                ActionLog.Global.PrintTrailer(ConsoleColor.Cyan);
                Environment.Exit(0);
            }

            if (context.Arguments.ContainsKey("help") || context.Arguments.ContainsKey("list"))
            {
                Console.WriteLine("Available Commands:");
                Console.WriteLine("- core greet: Prints a friendly greeting");
                Console.WriteLine("- core version: Shows the version");
                Console.WriteLine("- core autocomplete: Generates autocomplete script");
                Environment.Exit(0);
            }
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
            {
                throw new InvalidOperationException($"No command registered for {context.Noun} {context.Verb}.");
            }

            if (context.Arguments.ContainsKey("help"))
            {
                var helpText = await HelpProvider.GetHelpAsync(context.CommandName);
                Console.WriteLine(helpText);
                Environment.Exit(0);
            }

            var result = await command.ExecuteAsync(context);

            if (outputAsJson)
            {
                var output = result ?? new CommandResult
                {
                    Status = "success",
                    Messages = new[] { "Command executed successfully." }
                };
                Console.WriteLine(JsonSerializer.Serialize(output));
            }

            Environment.Exit(0);
        }
        catch (Exception ex)
        {
            if (outputAsJson)
            {
                var result = new CommandResult
                {
                    Status = "error",
                    Messages = new[] { ex.Message },
                    Data = new { errorType = ex.GetType().Name }
                };
                Console.WriteLine(JsonSerializer.Serialize(result));
            }
            else
            {
                Console.Error.WriteLine($"[ERROR] {ex.Message}");
            }

            Environment.Exit(2);
        }
    }

}
