using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Monithor.Components;
using Monithor.Definitions;
using Monithor.Dtos;
using Monithor.Entities;
using Monithor.Ports;

namespace Monithor
{
    public class MessageHandler : IMessageHandler
    {
        private readonly IHub _hub;
        private readonly ITraceStorage _traceStorage;
        private readonly IList<Receiver> _receivers;
        private readonly IList<Emitter> _emitters;

        private Timer _timer;

        private readonly TimeSpan _idleDetectionFrequency;
        private readonly TimeSpan _idleDetectionThreshold;

        public MessageHandler(
            IHub hub,
            ITraceStorage traceStorage,
            TimeSpan idleDetectionFrequency,
            TimeSpan idleDetectionThreshold)
        {
            _hub = hub;
            _traceStorage = traceStorage;
            _idleDetectionFrequency = idleDetectionFrequency;
            _idleDetectionThreshold = idleDetectionThreshold;

            _receivers = new List<Receiver>();
            _emitters = new List<Emitter>();


            _timer = new Timer((e) => DetectIdles(), null, TimeSpan.FromMilliseconds(0), _idleDetectionFrequency);
        }



        public void EmitterConnected(Emitter emitter)
        {
            emitter.LastHeartbeatReceivedDate = DateTime.UtcNow;
            emitter.LastMessageEmittedDate = DateTime.UtcNow;
            _emitters.Add(emitter);
        }

        public void ReceiverConnected(Receiver receiver)
        {
            receiver.LastHeartbeatReceivedDate = DateTime.UtcNow;
            _receivers.Add(receiver);
        }

        public void TraceReceived(Trace trace)
        {
            if (!_emitters.Contains(trace.Emitter))
            {
                _hub.NotifyError(trace.Emitter, new Error("Not Connected", "Must be connected before emitting", ErrorCode.NotConnected));
                return;
            }

            _traceStorage.StoreTrace(trace);

            foreach (var receiver in _receivers)
            {
                _hub.NotifyTraceReceived(receiver, trace);
            }
        }

        public void MetricUpdated(Metric metric)
        {
            if (!_emitters.Contains(metric.Emitter))
            {
                _hub.NotifyError(metric.Emitter, new Error("Not Connected", "Must be connected before emitting", ErrorCode.NotConnected));
                return;
            }

            foreach (var receiver in _receivers)
            {
                _hub.NotifyMetricUpdated(receiver, metric);
            }
        }

        public void ActorHeartbeated(Actor actor)
        {
            actor.LastHeartbeatReceivedDate = DateTime.UtcNow;
        }

        public Actor GetActorById(string id)
        {
            var emitter = _emitters.SingleOrDefault(e => e.Id == id);

            if (emitter == null)
                return _receivers.Single(e => e.Id == id);

            return emitter;
        }

        private void DetectIdles()
        {
            foreach (var emitter in _emitters.ToList())
            {
                if (DateTime.UtcNow - emitter.LastHeartbeatReceivedDate > _idleDetectionThreshold)
                {
                    _emitters.Remove(emitter);
                    _hub.NotifyDisconnection(emitter);
                }
            }

            foreach (var receiver in _receivers.ToList())
            {
                if (DateTime.UtcNow - receiver.LastHeartbeatReceivedDate > _idleDetectionThreshold)
                {
                    _receivers.Remove(receiver);
                    _hub.NotifyDisconnection(receiver);
                }
            }
        }
    }
}
