using SandlotWizards.ActionLogger;
using SandlotWizards.CommandLineParser.Localization;
using System;

namespace SandlotWizards.CommandLineParser.Help;

public static class SplashRenderer
{
    public static ILocalizationService Localizer { get; set; } = new DefaultLocalizationService();

    public static void RenderWhy(CommandDescriptor descriptor)
    {
        ActionLog.Global.Message($"Why use '{descriptor.Name}'?", ConsoleColor.Yellow);
        ActionLog.Global.Message(descriptor.WhyExplanation ?? "No explanation provided.", ConsoleColor.Gray);
    }

    public static void RenderExamples(CommandDescriptor descriptor)
    {
        if (descriptor.Examples.Count == 0)
        {
            ActionLog.Global.Warning("No examples available for this command.");
            return;
        }

        ActionLog.Global.Message($"Examples for '{descriptor.Name}':", ConsoleColor.Cyan);
        foreach (var ex in descriptor.Examples)
            ActionLog.Global.Message($"  {ex}", ConsoleColor.Gray);
    }

    public static void RenderDescribe(CommandDescriptor descriptor)
    {
        ActionLog.Global.Message($"Command: {descriptor.Name}", ConsoleColor.Green);
        ActionLog.Global.Message($"Description: {descriptor.Description}", ConsoleColor.Gray);
        ActionLog.Global.Message($"Why: {descriptor.WhyExplanation}", ConsoleColor.Gray);
        ActionLog.Global.Message($"Aliases: {string.Join(", ", descriptor.Aliases)}", ConsoleColor.Gray);
        ActionLog.Global.Message("Examples:", ConsoleColor.Cyan);
        foreach (var example in descriptor.Examples)
            ActionLog.Global.Message($"  {example}", ConsoleColor.Gray);
        ActionLog.Global.Message("");
    }
}
