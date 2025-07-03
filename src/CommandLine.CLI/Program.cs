using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SandlotWizards.CommandLineParser.Core;

var builder = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddSingleton<HelloWorldCommand>();
    });

var app = builder.Build();

await CommandLineApp.Run(args, registry =>
{
    registry.Register("hello", "world", app.Services.GetRequiredService<HelloWorldCommand>());
}, app.Services);