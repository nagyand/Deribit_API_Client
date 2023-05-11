using System.Text.Json.Serialization;

namespace DeribitApiClient.Application.Models.Request;

public class AuthenticationRequest
{
    [JsonPropertyName("id")]
    public ulong? Id { get; set; }

    [JsonPropertyName("grant_type")]
    public string GrantType { get; init; }

    [JsonPropertyName("client_id")]
    public string? ClientId { get; init; }

    [JsonPropertyName("client_secret")]
    public string? ClientSecret { get; init; }

    [JsonPropertyName("refresh_token")]
    public string? RefreshToken { get; set; }
}
