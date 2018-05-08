using Monithor.Dtos;
using Monithor.Entities;

namespace Monithor.Ports
{
    public interface IHub
    {
        void NotifyTraceReceived(Receiver receiver, Trace trace);
        void NotifyMetricUpdated(Receiver receiver, Metric metric);
        void NotifyDisconnection(Actor actor);
        void NotifyError(Actor actor, Error error);
    }
}
