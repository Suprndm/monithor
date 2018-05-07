using System.Collections.Generic;
using Monithor.Dtos;

namespace Monithor.Components
{
    public interface ITraceStorage
    {
        void StoreTrace(Trace trace);
        IList<Trace> GetAllTraces();
    }
}
