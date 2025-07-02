namespace SandlotWizards.CommandLineParser.Localization;

public static class LocalizationFactory
{
    public static ILocalizationService Create(string langCode)
    {
        return langCode.ToLower() switch
        {
            "es" => new SpanishLocalizationService(),
            _ => new DefaultLocalizationService(),
        };
    }
}
