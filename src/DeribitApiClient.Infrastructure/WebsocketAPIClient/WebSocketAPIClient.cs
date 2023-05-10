using DeribitApiClient.Application.Interfaces;
using DeribitApiClient.Application.Models.Request;
using DeribitApiClient.Application.Models.Response;
using Newtonsoft.Json;
using System.Buffers;
using System.Net.WebSockets;
using System.Text;

namespace DeribitApiClient.Infrastructure.WebsocketAPIClient;

/// <summary>
/// Wrapper class around ClientWebSocket
/// </summary>
internal class WebSocketAPIClient : IWebSocketAPIClient, IDisposable
{
    private bool disposedValue;
    private readonly ClientWebSocket _webSocketClient;
    private bool _isConnected;
    private bool _isAuthenticated;

    public WebSocketState State => _webSocketClient.State;

    public WebSocketAPIClient()
    {
        _webSocketClient = new ClientWebSocket();
    }

    public async ValueTask Connect(string url, CancellationToken token)
    {
        await _webSocketClient.ConnectAsync(new Uri(url), token);
        _isConnected = false;
    }

    public async ValueTask<WebSocketRequestResponse> Authenticate(AuthenticationRequest authenticationMessage, CancellationToken token)
    {
        if (_isConnected == false)
        {
            return new WebSocketRequestResponse(false, "Client is not connected");
        }
        var buffer = ArrayPool<byte>.Shared.Rent(1024);
        string message = JsonConvert.SerializeObject(authenticationMessage);
        await _webSocketClient.SendAsync(Encoding.UTF8.GetBytes(message), WebSocketMessageType.Text, true, token);
        var response = await _webSocketClient.ReceiveAsync(buffer, token);

        if (response.MessageType != WebSocketMessageType.Close)
        {
            var authentication = JsonConvert.DeserializeObject<AuthenticationResponse>(Encoding.ASCII.GetString(buffer, 0, response.Count));
            SetAuthencitaionToken(authentication);
        }

        ArrayPool<byte>.Shared.Return(buffer);
        return new WebSocketRequestResponse(true, "Authentication is succesfull");
    }

    public async ValueTask<WebSocketRequestResponse> SubscripbeToChannels(ChannelsSubscriptionRequest subscriptionMessage, CancellationToken token)
    {
        if (_isAuthenticated == false)
        {
            return new WebSocketRequestResponse(false, "Client is not authenticated");
        }
        var buffer = ArrayPool<byte>.Shared.Rent(1024);
        string message = JsonConvert.SerializeObject(subscriptionMessage);
        await _webSocketClient.SendAsync(Encoding.UTF8.GetBytes(message), WebSocketMessageType.Text, true, token);
        var response = await _webSocketClient.ReceiveAsync(buffer, token);
        if (response.MessageType != WebSocketMessageType.Close)
        {
            ArrayPool<byte>.Shared.Return(buffer);
            return new WebSocketRequestResponse(true, "Subscription is successfull");
        }
        ArrayPool<byte>.Shared.Return(buffer);
        return new WebSocketRequestResponse(false, "Subscription is failed because of Web socket is closed");
    }

    public async ValueTask<string> ReadAsync(CancellationToken token)
    {
        var buffer = ArrayPool<byte>.Shared.Rent(1024);
        var response = await _webSocketClient.ReceiveAsync(buffer, token);
        var resultString = Encoding.ASCII.GetString(buffer, 0, response.Count);
        ArrayPool<byte>.Shared.Return(buffer);
        return resultString;
    }

    private void SetAuthencitaionToken(AuthenticationResponse authenticationResponse)
    {
        _webSocketClient.Options.SetRequestHeader("Authorization", $"{authenticationResponse.TokenType} {authenticationResponse.AccessToken}");
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                _webSocketClient.Dispose();
            }
            disposedValue = true;
        }
    }

    ~WebSocketAPIClient()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: false);
    }

    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}
