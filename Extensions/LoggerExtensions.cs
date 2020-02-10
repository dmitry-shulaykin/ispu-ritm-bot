using Microsoft.Extensions.Logging;

namespace GradesNotification.Extensions
{
    public static class LoggerExtensions {
        public static TelemetryEvent GetTelemetryEventDisposable(this ILogger logger, string taskName)  {
            return new TelemetryEvent(logger, taskName);
        }
    }
}