using DeribitApiClient.Application.Interfaces;
using DeribitApiClient.Application.Models.Request;
using DeribitApiClient.Application.Models.Response;
using System.Buffers;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;

namespace DeribitApiClient.Infrastructure.WebsocketAPIClient;

/// <summary>
/// Wrapper class around ClientWebSocket
/// </summary>
public class WebSocketAPIClient : IWebSocketAPIClient, IDisposable
{
    private bool disposedValue;
    private readonly ClientWebSocket _webSocketClient;
    private readonly Encoding _encoding;
    private readonly JsonSerializerOptions _jsonOptions;
    private bool _isConnected;
    private bool _isAuthenticated;

    public WebSocketState State => _webSocketClient.State;

    public WebSocketAPIClient()
    {
        _webSocketClient = new ClientWebSocket();
        _encoding = Encoding.UTF8;
        _jsonOptions = new JsonSerializerOptions() { DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull, };
    }

    public async ValueTask Connect(string url, CancellationToken token)
    {
        var uri = new Uri($"wss://{url}/ws/api/v2");
        await _webSocketClient.ConnectAsync(uri, token);
        _isConnected = true;
    }

    public async ValueTask<WebSocketRequestResponse> Authenticate(AuthenticationRequest authenticationMessage, CancellationToken token)
    {
        if (_isConnected == false)
        {
            return new WebSocketRequestResponse(false, "Client is not connected");
        }
        
        var message = new RequestMessage<AuthenticationRequest>()
        {
            Method = "public/auth",
            Params = authenticationMessage
        };
        string jsonString = JsonSerializer.Serialize(message, _jsonOptions);
        await _webSocketClient.SendAsync(_encoding.GetBytes(jsonString), WebSocketMessageType.Text, true, token);

        var buffer = ArrayPool<byte>.Shared.Rent(4096);
        try
        {
            var response = await _webSocketClient.ReceiveAsync(buffer, token);
            if (response.MessageType != WebSocketMessageType.Close)
            {
                jsonString = _encoding.GetString(buffer, 0, response.Count);
                var responseMsg = JsonSerializer.Deserialize<ResponseMessage<AuthenticationResponse>>(jsonString, _jsonOptions);
                SetAuthenticationToken(responseMsg.Result);
            }
        }
        finally
        {
            ArrayPool<byte>.Shared.Return(buffer);
        }

        _isAuthenticated = true;
        return new WebSocketRequestResponse(true, "Authentication is succesfull");
    }

    public async ValueTask<WebSocketRequestResponse> SubscribeToChannels(ChannelsSubscriptionRequest subscriptionMessage, CancellationToken token)
    {
        if (_isAuthenticated == false)
        {
            return new WebSocketRequestResponse(false, "Client is not authenticated");
        }

        var message = new RequestMessage<ChannelsSubscriptionRequest>()
        {
            Method = "private/subscribe",
            Params = subscriptionMessage
        };
        string jsonString = JsonSerializer.Serialize(message, _jsonOptions);
        await _webSocketClient.SendAsync(_encoding.GetBytes(jsonString), WebSocketMessageType.Text, true, token);

        var buffer = ArrayPool<byte>.Shared.Rent(4096);
        try
        {
            var response = await _webSocketClient.ReceiveAsync(buffer, token);
            if (response.MessageType != WebSocketMessageType.Close)
            {
                jsonString = _encoding.GetString(buffer, 0, response.Count);
                var subscribeResponse = JsonSerializer.Deserialize<ChannelsSubscriptionResponse>(jsonString);
                if (subscribeResponse == null)
                    return new WebSocketRequestResponse(false, "Cound not parse subscription response");

                // figure out whether we managed to subscribe to everything we wanted to
                var notSubscribedTo = subscriptionMessage.Channels.Except(subscribeResponse.Result);
                if (notSubscribedTo.Any())
                    return new WebSocketRequestResponse(false, "Cound not subscribe to " + string.Join(", ", notSubscribedTo));
                else
                    return new WebSocketRequestResponse(true, "Subscription is successfull");
            }
        }
        finally
        {
            ArrayPool<byte>.Shared.Return(buffer);
        }
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

    private void SetAuthenticationToken(AuthenticationResponse authenticationResponse)
    {
        //_webSocketClient.Options.SetRequestHeader("Authorization", $"{authenticationResponse.TokenType} {authenticationResponse.AccessToken}");
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
