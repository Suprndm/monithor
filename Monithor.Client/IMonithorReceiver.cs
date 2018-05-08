using System;
using Monithor.Dtos;

namespace Monithor.Client
{
    public interface IMonithorReceiver : IMonithorActor
    {
        event Action<Trace> TraceReceived;
        event Action<Metric> MetricUpdated;
    }
}
