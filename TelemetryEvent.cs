using System;
using System.Diagnostics;
using Microsoft.Extensions.Logging;

namespace GradesNotification {

    public class TelemetryEvent : IDisposable
    {
        private string _taskName { get; set; } 
        private Stopwatch _watch { get; set; }
        private ILogger _logger {get; set;}
        private bool _disposed { get; set; } = false;
        public TelemetryEvent(ILogger logger, string taskName) 
        {
            _taskName = taskName;
            _logger = logger;
            _logger.LogInformation($"Started {_taskName}.");
            _watch = Stopwatch.StartNew();
        }

        public void Dispose()
        {
            if (!_disposed) {
                _watch.Stop();
                _logger.LogInformation($"Finished {_taskName}. Elapsed {_watch.ElapsedMilliseconds} ms.");
                _disposed = true;
            }

            _logger.LogWarning($"Task event {_taskName} already disposed!");
        }
    }

}