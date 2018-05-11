using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Monithor.Api.Logging
{
    public interface ILogCollector
    {
        IList<string> GetAllLogs();
    }
}
