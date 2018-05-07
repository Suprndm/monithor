using System.Collections.Generic;
using System.Linq;
using Monithor.Dtos;

namespace Monithor.Components
{
    public class MemoryStorage : ITraceStorage
    {
        private readonly IList<Trace> _traces;

        public MemoryStorage()
        {
            _traces = new List<Trace>();
        }

        public void StoreTrace(Trace trace)
        {
            _traces.Add(trace);
        }

        public IList<Trace> GetAllTraces()
        {
            return _traces.ToList();
        }
    }
}
