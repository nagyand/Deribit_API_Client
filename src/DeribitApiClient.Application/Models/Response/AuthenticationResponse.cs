using System.Text.Json.Serialization;

namespace DeribitApiClient.Application.Models.Response;

public class AuthenticationResponse
{
    [JsonPropertyName("id")]
    public ulong? Id { get; set; }

    [JsonPropertyName("access_token")]
    public string AccessToken { get; set; } = default!;
    
    [JsonPropertyName("refresh_token")]
    public string RefreshToken { get; set; } = default!;

    [JsonPropertyName("token_type")]
    public string TokenType { get; set; } = default!;

    [JsonPropertyName("expires_in")]
    public ulong ExpiresIn { get; set; } = default!;
}
