using System.Collections.Generic;

namespace SandlotWizards.CommandLineParser.Localization;

public class SpanishLocalizationService : ILocalizationService
{
    private readonly Dictionary<string, string> _translations = new()
    {
        ["UnknownCommand"] = "Comando desconocido: '{0}'",
        ["DidYouMean"] = "¿Quisiste decir '{0}'?",
        ["HelpPrompt"] = "Escribe `lore --help` para ver los comandos disponibles.",
        ["Command"] = "Comando: {0}",
        ["Description"] = "Descripción:",
        ["Usage"] = "Uso:",
        ["Examples"] = "Ejemplos:",
        ["Group"] = "Grupo:"
    };

    public string Get(string key) =>
        _translations.TryGetValue(key, out var value) ? value : $"[[{key}]]";

    public string Get(string key, params object[] args) =>
        string.Format(Get(key), args);
}
