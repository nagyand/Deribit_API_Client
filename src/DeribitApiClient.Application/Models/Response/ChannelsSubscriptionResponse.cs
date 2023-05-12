﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace DeribitApiClient.Application.Models.Response;

public class ChannelsSubscriptionResponse
{
    [JsonPropertyName("id")]
    public ulong? Id { get; set; }

    [JsonPropertyName("result")]
    public List<string> Result { get; init; }
}
