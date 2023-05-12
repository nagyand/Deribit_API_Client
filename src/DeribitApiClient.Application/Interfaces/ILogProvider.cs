namespace DeribitApiClient.Application.Interfaces;

public interface ILogProvider
{
    public void LogInformation(string information);
    public void LogError(string errorMessage);
}
