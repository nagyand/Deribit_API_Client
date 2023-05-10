﻿using System.Text.Json.Serialization;

namespace DeribitApiClient.Application.Models.Response;

public class AuthenticationResponse
{
    [JsonPropertyName("access_token")]
    public string AccessToken { get; set; } = default!;
    [JsonPropertyName("token_type")]
    public string TokenType { get; set; } = default!;
}
