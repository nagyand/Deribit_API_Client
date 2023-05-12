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

    public DeribitAPIHandler(IWebSocketAPIClient client, ILogProvider logProvider, IOptions<DeribitApiClientConfig> config)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
        _logger = logProvider ?? throw new ArgumentNullException(nameof(logProvider));
        ArgumentNullException.ThrowIfNull(config, nameof(config));
        _deribitApiClientConfig = config.Value;
    }

    public async ValueTask RunAsync(CancellationToken token)
    {
        // establish connection
        await _client.Connect(_deribitApiClientConfig.BaseUrl, token);

        // authenticate
        var authRequest = new AuthenticationRequest()
        {
            GrantType = "client_credentials",
            ClientId = _deribitApiClientConfig.ClientId,
            ClientSecret = _deribitApiClientConfig.ClientSecret
        };
        var authenticationResult = await _client.Authenticate(authRequest, token);
        if (!authenticationResult.IsSuccessfull)
        {
            _logger.LogError("Authentication is failed");
            return;
        }

        // subscribe to data feed
        var subscribeRequest = new ChannelsSubscriptionRequest()
        {
            Channels = _deribitApiClientConfig.SubscribeTo
        };
        var subscriptionResult = await _client.SubscribeToChannels(subscribeRequest, token);
        if (!subscriptionResult.IsSuccessfull)
        {
            _logger.LogError("Subscription is failed");
            return;
        }

        // handle incoming messages
        while(_client.State != WebSocketState.Closed && !token.IsCancellationRequested)
        {
            string response = await _client.ReadAsync(token);
            _logger.LogInformation(response);
        }
    }
}
