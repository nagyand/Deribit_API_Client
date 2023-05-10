using DeribitApiClient.Application.Models.Request;
using DeribitApiClient.Application.Models.Response;
using System.Net.WebSockets;

namespace DeribitApiClient.Application.Interfaces;

public interface IWebSocketAPIClient
{
    WebSocketState State { get; }

    ValueTask<WebSocketRequestResponse> Authenticate(AuthenticationRequest authenticationMessage, CancellationToken token);
    ValueTask Connect(string url, CancellationToken token);
    void Dispose();
    ValueTask<string> ReadAsync(CancellationToken token);
    ValueTask<WebSocketRequestResponse> SubscripbeToChannels(ChannelsSubscriptionRequest subscriptionMessage, CancellationToken token);
}
