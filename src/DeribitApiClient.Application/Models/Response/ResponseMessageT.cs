using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace DeribitApiClient.Application.Models.Response
{
    public class ResponseMessage<T> where T : class
    {
        [JsonPropertyName("id")]
        public ulong? Id { get; init; }

        [JsonPropertyName("result")]
        public T? Result { get; init; }

        [JsonPropertyName("testnet")]
        public bool Testnet { get; set; }

    }
}
