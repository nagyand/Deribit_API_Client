using DeribitApiClient.Application.Interfaces;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
namespace DeribitApiClient.Infrastructure.LogProvider;

/// <summary>
/// Generic class for logging
/// </summary>
public class LogProvider : ILogProvider
{
    public void LogError(string errorMessage)
    {
        Console.WriteLine($"info [{DateTime.UtcNow}]: {errorMessage}");
    }

    public void LogInformation(string information)
    {
        Console.WriteLine($"info [{DateTime.UtcNow}]: {information}");
    }
}
