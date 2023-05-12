namespace DeribitApiClient.Application.Interfaces;

public interface IDeribitAPIHandler
{
    ValueTask RunAsync(CancellationToken token);
}
