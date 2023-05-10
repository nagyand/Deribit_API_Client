using DeribitApiClient.Application.Interfaces;
using DeribitApiClient.Application.Models.Configuration;
using DeribitApiClient.Application.Models.Request;
using Microsoft.Extensions.Options;
using System.Net.WebSockets;

namespace DeribitApiClient.Application.Handlers;

public class DeribitAPIHandler:IDeribitAPIHandler
{
    private readonly DeribitApiClientConfig _deribitApiClientConfig;
    private readonly IWebSocketAPIClient _client;
    private readonly ILogProvider _logger;

    public DeribitAPIHandler(IWebSocketAPIClient client, ILogProvider logProvider ,IOptions<DeribitApiClientConfig> config)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
        _logger = logProvider ?? throw new ArgumentNullException(nameof(logProvider));
        ArgumentNullException.ThrowIfNull(config, nameof(config));
        _deribitApiClientConfig = config.Value;
    }

    public async ValueTask RunAsync(CancellationToken token)
    {
        await _client.Connect(_deribitApiClientConfig.BaseUrl, token);
        var authenticationResult = await _client.Authenticate(new AuthenticationRequest(), token);
        if (!authenticationResult.IsSuccessfull)
        {
            _logger.LogError("Authentication is failed");
            return;
        }
        var subscriptionResult = await _client.SubscripbeToChannels(new ChannelsSubscriptionRequest(), token);
        if (!subscriptionResult.IsSuccessfull)
        {
            _logger.LogError("Subscription is failed");
            return;
        }
        while(_client.State != WebSocketState.Closed && !token.IsCancellationRequested)
        {
            string response = await _client.ReadAsync(token);
            _logger.LogInformation(response);
        }
    }
}
