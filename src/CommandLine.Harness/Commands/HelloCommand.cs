using SandlotWizards.CommandLineParser.Core;

namespace CommandLine.Harness.Commands
{
    public class HelloCommand : ICommand
    {
        public async Task ExecuteAsync(CommandContext context)
        {
            Console.WriteLine("Copilot CLI is running and command dispatch works!");
            await Task.CompletedTask;
        }
    }
}