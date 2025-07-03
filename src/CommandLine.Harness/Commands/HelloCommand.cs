using SandlotWizards.CommandLineParser.Core;

namespace CommandLine.Harness.Commands
{
    public class HelloCommand : ICommand
    {
        public async Task<CommandResult?> ExecuteAsync(CommandContext context)
        {
            var message = "Copilot CLI is running and command dispatch works!";
            Console.WriteLine(message);

            return await Task.FromResult(new CommandResult
            {
                Status = "success",
                Messages = new[] { message }
            });
        }
    }
}
