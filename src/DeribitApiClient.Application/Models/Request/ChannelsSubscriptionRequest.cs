using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace DeribitApiClient.Application.Models.Request;

public class ChannelsSubscriptionRequest
{
    [JsonPropertyName("channels")]
    public List<string> Channels { get; init; }
}
