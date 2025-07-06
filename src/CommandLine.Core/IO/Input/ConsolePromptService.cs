using System;
using System.Text.RegularExpressions;

namespace SandlotWizards.CommandLineParser.IO.Input
{
    public class ConsolePromptService : IInteractivePromptService
    {
        public string Prompt(string label, string? pattern = null, string? errorMessage = null)
        {
            while (true)
            {
                Console.Write($"{label}: ");
                Console.Out.Flush();
                var input = Console.ReadLine() ?? string.Empty;

                if (string.IsNullOrWhiteSpace(pattern) || Regex.IsMatch(input, pattern))
                    return input;

                Console.WriteLine(errorMessage ?? "Invalid input.");
                Console.Out.Flush();
            }
        }

        public int PromptChoice(string label, string[] options)
        {
            Console.WriteLine(label);
            Console.Out.Flush();
            for (int i = 0; i < options.Length; i++)
            {
                Console.WriteLine($"{i + 1}) {options[i]}");
            }
            Console.Out.Flush();
            while (true)
            {
                Console.Write("> ");
                Console.Out.Flush();
                var input = Console.ReadLine();
                if (int.TryParse(input, out var index) && index >= 1 && index <= options.Length)
                    return index - 1;

                Console.WriteLine("Please enter a valid option number."); 
                Console.Out.Flush();
            }
        }
    }
}
