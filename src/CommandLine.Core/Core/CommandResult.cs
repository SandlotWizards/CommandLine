namespace SandlotWizards.CommandLineParser.Core
{
    public class CommandResult
    {
        public string Status { get; set; } = "success";
        public string[] Messages { get; set; } = [];
        public object? Data { get; set; } = null;
    }
}
