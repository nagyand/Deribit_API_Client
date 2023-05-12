using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace DeribitApiClient.Application.Models.Request
{
    public class RequestMessage<T> where T : class
    {
        [JsonPropertyName("jsonrpc")]
        public double Jsonrpc { get; init; } = 2.0;

        [JsonPropertyName("id")]
        public ulong? Id { get; init; }

        [JsonPropertyName("method")]
        public string Method { get; set; }

        [JsonPropertyName("params")]
        public T Params { get; init; }

    }
}
