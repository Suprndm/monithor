using System;
using System.Collections.Generic;
using System.Linq;

namespace Monithor.Api.Logging
{
    public class SimpleLogger : ILogger, ILogCollector
    {
        private readonly IList<string> _allLogs;

        public SimpleLogger()
        {
            _allLogs = new List<string>();
        }

        public void Log(string message)
        {
            if (_allLogs.Count > 1000)
            {
                _allLogs.RemoveAt(0);
            }

            _allLogs.Add($"{DateTimeOffset.UtcNow.ToString("MM/dd/yyyy hh:mm:ss.fff")} : {message}");
        }

        public IList<string> GetAllLogs()
        {
            return _allLogs.Reverse().ToList();
        }
    }
}
