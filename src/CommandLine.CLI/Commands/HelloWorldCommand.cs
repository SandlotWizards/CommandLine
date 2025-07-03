using SandlotWizards.CommandLineParser.Core;

public class HelloWorldCommand : ICommand
{
    public Task<CommandResult?> ExecuteAsync(CommandContext context)
    {
        var name = context.Arguments.TryGetValue("name", out var val) ? val : "world";
        var message = $"Hello, {name}!";
        Console.WriteLine(message);

        return Task.FromResult<CommandResult?>(new CommandResult
        {
            Status = "success",
            Messages = new[] { message }
        });
    }
}
