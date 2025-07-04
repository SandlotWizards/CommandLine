namespace SandlotWizards.CommandLineParser.Input
{
    public interface IInteractivePromptService
    {
        string Prompt(string label, string? pattern = null, string? errorMessage = null);
        int PromptChoice(string label, string[] options);
    }
}
