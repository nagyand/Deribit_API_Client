namespace DeribitApiClient.Application.Models.Configuration;

public class DeribitApiClientConfig
{
    public string BaseUrl { get; set; }
    public string ClientId { get; set; }
    public string ClientSecret { get; set; }
    public List<string> SubscribeTo { get; set; }
}
