// See https://aka.ms/new-console-template for more information
using DeribitApiClient.Application.Interfaces;
using DeribitApiClient.Application.Models.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using IHost host = Host.CreateDefaultBuilder(args).ConfigureAppConfiguration
    ((hostingContext, configuration) =>
    {
        IHostEnvironment env = hostingContext.HostingEnvironment;
        configuration.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                     .AddJsonFile($"appsettings.{env.EnvironmentName}.json", true, true);

        IConfigurationRoot configurationRoot = configuration.Build();

    }).ConfigureServices((hostContext,services) =>
    {
        var valami = hostContext.Configuration;
        services.Configure<DeribitApiClientConfig>(hostContext.Configuration.GetSection(nameof(DeribitApiClientConfig)));
        services.AddSingleton<IWebSocketAPIClient, IWebSocketAPIClient>();
        services.AddSingleton<IDeribitAPIHandler, IDeribitAPIHandler>();
    }).Build();





await host.RunAsync();
