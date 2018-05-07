using System;
using System.Threading.Tasks;
using Monithor.Dtos;
using Monithor.Entities;

namespace Monithor.Ports
{
    public interface IHubConnector
    {
        event Action<Emitter> EmitterConnected;
        event Action<Receiver> ReceiverConnected;
        event Action<Trace> TraceReceived;
        event Action<Metric> MetricUpdated;
        event Action<Actor> ActorHeartbeated;

        void NotifyTraceReceived(Receiver receiver, Trace trace);
        void NotifyMetricUpdated(Receiver receiver, Metric metric);
        void NotifyDisconnection(Actor actor);
        void NotifyError(Actor actor, Error error);
    }
}
