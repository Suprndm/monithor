using System;
using Monithor.Dtos;
using Monithor.Entities;

namespace Monithor
{
    public interface IMessageHandler
    {
        void EmitterConnected(Emitter emitter);
        void ReceiverConnected(Receiver receiver);
        void SomeoneDisconnected(string clientId);
        void TraceReceived(Trace trace);
        void MetricUpdated(Metric metric);

        Actor GetActorById(string id);
    }
}