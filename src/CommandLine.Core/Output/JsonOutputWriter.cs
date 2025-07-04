using SandlotWizards.ActionLogger;
using SandlotWizards.CommandLineParser.Core;
using System;
using System.Text.Json;

namespace SandlotWizards.CommandLineParser.Output
{
    public class JsonOutputWriter : IOutputWriter
    {
        public void WriteHeader() { /* No-op for JSON mode */ }
        public void WriteTrailer() { /* No-op for JSON mode */ }

        public void WriteResult(CommandResult result)
        {
            var output = result ?? new CommandResult
            {
                Status = "success",
                Messages = new[] { "Command executed successfully." }
            };
            ActionLog.Global.Message(JsonSerializer.Serialize(output));
        }

        public void WriteHelp(string helpText)
        {
            var result = new CommandResult
            {
                Status = "help",
                Messages = new[] { helpText }
            };
            ActionLog.Global.Message(JsonSerializer.Serialize(result));
        }

        public void WriteError(Exception ex)
        {
            var result = new CommandResult
            {
                Status = "error",
                Messages = new[] { ex.Message },
                Data = new { errorType = ex.GetType().Name }
            };
            ActionLog.Global.Message(JsonSerializer.Serialize(result));
        }
    }
}
