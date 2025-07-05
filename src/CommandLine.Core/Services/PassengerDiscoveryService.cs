using SandlotWizards.CommandLineParser.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace SandlotWizards.CommandLineParser.Services;

public class PassengerDiscoveryService
{
    public async Task<List<PassengerManifest>> DiscoverAsync()
    {
        var passengers = new List<PassengerManifest>();
        var toolList = await GetDotNetToolListAsync();

        foreach (var tool in toolList)
        {
            if (!tool.PackageId.EndsWith(".passenger"))
                continue;

            var describeOutput = await RunDescribeCommandAsync(tool.CommandName);
            if (describeOutput is null) continue;

            try
            {
                var manifest = JsonSerializer.Deserialize<PassengerManifest>(describeOutput, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (manifest is not null)
                    passengers.Add(manifest);
            }
            catch
            {
                // Log or skip
            }
        }

        return passengers;
    }

    private async Task<List<(string PackageId, string CommandName)>> GetDotNetToolListAsync()
    {
        var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "dotnet",
                Arguments = "tool list -g",
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            }
        };

        process.StartInfo.EnvironmentVariables["IS_PASSENGER"] = "1";

        process.Start();
        var output = await process.StandardOutput.ReadToEndAsync();
        await process.WaitForExitAsync();

        var results = new List<(string, string)>();
        var lines = output.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries).Skip(2);

        foreach (var line in lines)
        {
            var parts = line.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length >= 2)
            {
                results.Add((parts[0].Trim(), parts[2].Trim()));
            }
        }

        return results;
    }

    private async Task<string?> RunDescribeCommandAsync(string toolCommand)
    {
        var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = toolCommand,
                Arguments = "system describe --output json",
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            }
        };

        process.StartInfo.EnvironmentVariables["IS_PASSENGER"] = "1";

        try
        {
            process.Start();
            var output = await process.StandardOutput.ReadToEndAsync();
            await process.WaitForExitAsync();
            return output;
        }
        catch
        {
            return null;
        }
    }
}