using SandlotWizards.ActionLogger;
using SandlotWizards.CommandLineParser.Execution;
using SandlotWizards.CommandLineParser.IO.Input;
using System;
using System.Threading.Tasks;

namespace SandlotWizards.Passenger.Commands;

public sealed class AddConnectionProfileCommand : IRoutableCommand
{
    public string Noun => "connection-profile";
    public string Verb => "add";
    public string Description => "Adds a GitHub, GitLab, or Azure DevOps connection profile with PAT.";
    public string Group => "GitHub Wrappers";
    public bool IsEnabled => true;
    public bool ShowInList => true;

    public string? type { get; set; }
    public string? organizationName { get; set; }
    public string? personalAccessToken { get; set; }

    public async Task<CommandResult?> ExecuteAsync(CommandContext context)
    {
        ActionLog.Global.Message("Add Connection Profile Command");
        ActionLog.Global.Message("");

        await ValidateAsync(context);

        using (ActionLog.Global.BeginStep("Adding connection profile..."))
        {
            if (context.IsDryRun)
            {
                ActionLog.Global.Info($"[Dry-Run] Connection profile added: Type={type}, Organization={organizationName}, Token={personalAccessToken}");
                return await Task.FromResult<CommandResult?>(new CommandResult
                {
                    Status = "success",
                    Messages = new[] { "Success!" }
                });
            }
            //var service = context.Resolve<IConnectionProfileService>();
            //await service.ExecuteAsync(this);

            ActionLog.Global.Info($"Connection profile added: Type={type}, Organization={organizationName}, Token={personalAccessToken}");
        }

        return await Task.FromResult<CommandResult?>(new CommandResult
        {
            Status = "success",
            Messages = new[] { "Success!" }
        });
    }

    private async Task ValidateAsync(CommandContext context)
    {
        var prompt = context.Resolve<IInteractivePromptService>();
        var outputMode = context.Metadata["OutputFormat"]?.ToString();
        var noPrompt = context.Arguments.ContainsKey("no-prompt");
        var canPrompt = outputMode == "text" && !noPrompt;

        if (string.IsNullOrWhiteSpace(type) && canPrompt)
        {
            var choice = prompt.PromptChoice("Select connection type:", new[] { "GitHub", "GitLab", "AzureDevOps" });
            type = choice switch
            {
                0 => "GitHub",
                1 => "GitLab",
                2 => "AzureDevOps",
                _ => null
            };
        }

        if (string.IsNullOrWhiteSpace(organizationName))
        {
            if (!canPrompt)
                throw new InvalidOperationException("Organization name is required.");
            organizationName = prompt.Prompt(
                "Organization Name",
                pattern: @"\\S",
                errorMessage: "Organization name cannot be blank."
            );
        }

        if (string.IsNullOrWhiteSpace(personalAccessToken))
        {
            if (!canPrompt)
                throw new InvalidOperationException("Personal Access Token is required.");
            personalAccessToken = prompt.Prompt(
                "Personal Access Token (starts with 'ghp_')",
                pattern: "^ghp_[A-Za-z0-9]{20,}$",
                errorMessage: "Token must start with 'ghp_' and be at least 20 characters."
            );
        }

        await Task.CompletedTask;
    }
}
