using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeribitApiClient.Application.Interfaces;

public interface ILogger
{
    public void LogInformation(string information);
    public void LogError(string errorMessage);
}
