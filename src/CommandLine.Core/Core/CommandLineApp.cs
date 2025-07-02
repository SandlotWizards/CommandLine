using SandlotWizards.ActionLogger;
using SandlotWizards.ActionLogger.Services;
using SandlotWizards.CommandLineParser.Help;
using SandlotWizards.CommandLineParser.Parsing;
using System;
using System.Linq;
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
        // Register built-in commands using noun-verb model
        registry.Register("core", "greet", new BuiltIn.GreetCommand());
        registry.Register("core", "version", new BuiltIn.VersionCommand());

        configure(registry);
        var parser = new ContextParser();
        var context = parser.Parse(args);
        context.Metadata["ServiceProvider"] = serviceProvider;

        ActionLog.Global.PrintHeader(ConsoleColor.Cyan);

        // Top-level help-like flags with no command
        if (string.IsNullOrWhiteSpace(context.CommandName))
        {
            if (context.Arguments.ContainsKey("version"))
            {
                ActionLog.Global.Message("lore CLI version 1.0.0", ConsoleColor.Gray);
                ActionLog.Global.PrintTrailer(ConsoleColor.Cyan);
                return;
            }

            if (context.Arguments.ContainsKey("help") || context.Arguments.ContainsKey("list"))
            {
                Console.WriteLine("Available Commands:");
                Console.WriteLine("- core greet: Prints a friendly greeting");
                Console.WriteLine("- core version: Shows the version");
                Console.WriteLine("- core autocomplete: Generates autocomplete script");
                ActionLog.Global.PrintTrailer(ConsoleColor.Cyan);
                return;
            }

            if (context.Arguments.ContainsKey("why"))
            {
                ActionLog.Global.Message("lore is your AI copilot for .NET CLI development.");
                ActionLog.Global.Message("It helps you generate, extend, and execute intelligent commands.");
                ActionLog.Global.PrintTrailer(ConsoleColor.Cyan);
                return;
            }

            if (context.Arguments.ContainsKey("examples"))
            {
                ActionLog.Global.Message("Examples:");
                ActionLog.Global.Message("  lore greet --name Alice");
                ActionLog.Global.Message("  lore version");
                ActionLog.Global.Message("  lore autocomplete --shell bash");
                ActionLog.Global.PrintTrailer(ConsoleColor.Cyan);
                return;
            }

            if (context.Arguments.ContainsKey("describe"))
            {
                ActionLog.Global.Message("lore CLI – powered by Sandlot.CommandLineParser");
                ActionLog.Global.Message("Version: 1.0.0");
                ActionLog.Global.Message("Docs: https://github.com/SandlotWizards");
                ActionLog.Global.PrintTrailer(ConsoleColor.Cyan);
                return;
            }

            // Default splash: no args
            ActionLog.Global.Message("Welcome to your CLI.", ConsoleColor.Gray);
            ActionLog.Global.Message("");
            ActionLog.Global.Message("Type `lore --help` to begin.");
            ActionLog.Global.PrintTrailer(ConsoleColor.Cyan);
            return;
        }

        if (!registry.TryGet(context.Noun, context.Verb, out var command))
        {
            ActionLog.Global.Warning($"Unknown command '{context.Noun} {context.Verb}'.");
            return;
        }

        if (command is null) return;

        BindCommandPropertiesFromArguments(command, context);
        await command.ExecuteAsync(context);

        ActionLog.Global.PrintTrailer(ConsoleColor.Cyan);
    }

    private static void BindCommandPropertiesFromArguments(ICommand command, CommandContext context)
    {
        var props = command.GetType().GetProperties();

        foreach (var prop in props)
        {
            if (!prop.CanWrite) continue;

            // Match CLI-style "project-type" to C# "ProjectType"
            var matchingKey = context.Arguments.Keys
                .FirstOrDefault(k => Normalize(k) == Normalize(prop.Name));

            if (matchingKey != null)
            {
                try
                {
                    var rawValue = context.Arguments[matchingKey];
                    var converted = Convert.ChangeType(rawValue, prop.PropertyType);
                    prop.SetValue(command, converted);
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException($"Failed to bind '{matchingKey}' to '{prop.Name}': {ex.Message}");
                }
            }
        }

        static string Normalize(string name) =>
            name.Replace("-", "", StringComparison.OrdinalIgnoreCase).ToLowerInvariant();
    }


}
