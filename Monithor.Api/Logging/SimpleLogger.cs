using System;
using System.Collections.Generic;

namespace Monithor.Api.Logging
{
    public class SimpleLogger :ILogger, ILogCollector
    {
        private readonly IList<string> _allLogs;

        public SimpleLogger()
        {
            _allLogs = new List<string>();
        }

        public void Log(string message)
        {
            _allLogs.Add($"{DateTimeOffset.UtcNow} : {message}");
        }

        public IList<string> GetAllLogs()
        {
            return _allLogs;
        }
    }
}
