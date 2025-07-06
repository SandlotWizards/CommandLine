using SandlotWizards.CommandLineParser.Execution;
using System.Text;

namespace SandlotWizards.CommandLineParser.Core;

public static class SplashRenderer
{
    public static string Render(CommandContext context)
    {
        var banner = @"  
 _                        
| |                       
| |      ___   _ __   ___ 
| |     / _ \ | '__| / _ \
| |____| (_) || |   |  __/
\_____/ \___/ |_|    \___|

";
        var splashOut = new StringBuilder()
            .AppendLine(banner)
            .AppendLine("The shell you know. The AI you’ve needed.")
            .ToString();
        return splashOut;
    }
}
