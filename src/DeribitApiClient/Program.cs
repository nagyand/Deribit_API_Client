// See https://aka.ms/new-console-template for more information
using DeribitApiClient.Application.Handlers;
using DeribitApiClient.Application.Interfaces;
using DeribitApiClient.Application.Models.Configuration;
using DeribitApiClient.Infrastructure.LogProvider;
using DeribitApiClient.Infrastructure.WebsocketAPIClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using IHost host = Host.CreateDefaultBuilder(args).ConfigureAppConfiguration
    ((hostingContext, configuration) =>
    {
        IHostEnvironment env = hostingContext.HostingEnvironment;
        
        // not needed, these are already added by default
        //configuration.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
        //             .AddJsonFile($"appsettings.{env.EnvironmentName}.json", true, true);

        IConfigurationRoot configurationRoot = configuration.Build();

    }).ConfigureServices((hostContext,services) =>
    {
        var valami = hostContext.Configuration;
        services.Configure<DeribitApiClientConfig>(hostContext.Configuration.GetSection(nameof(DeribitApiClientConfig)));
        services.AddSingleton<IWebSocketAPIClient, WebSocketAPIClient>();
        services.AddSingleton<IDeribitAPIHandler, DeribitAPIHandler>();
        services.AddSingleton<ILogProvider, LogProvider>();
    }).Build();

CancellationTokenSource cancellationToken = new CancellationTokenSource();
IDeribitAPIHandler apiHandler = host.Services.GetRequiredService<IDeribitAPIHandler>();
ILogProvider logger = host.Services.GetRequiredService<ILogProvider>();

await apiHandler.RunAsync(cancellationToken.Token);

Console.CancelKeyPress += Console_CancelKeyPress;
async void Console_CancelKeyPress(object? sender, ConsoleCancelEventArgs e)
{
    logger.LogInformation("CTRL+C pressed, initiating graceful shutdown...");
    cancellationToken.Cancel();
    e.Cancel = true;
}

await host.RunAsync();
