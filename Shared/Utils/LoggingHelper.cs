using Microsoft.Extensions.Logging;

namespace Shared.Utils
{
    public static class LoggingHelper
    {
        public static void LogInformation(ILogger logger, string message)
        {
            logger.LogInformation(message);
        }

        public static void LogError(ILogger logger, string message, Exception ex)
        {
            logger.LogError(ex, message);
        }
    }
}