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
    public class MessageHandler
    {
        private readonly IHubConnector _hubConnector;
        private readonly ITraceStorage _traceStorage;
        private readonly IList<Receiver> _receivers;
        private readonly IList<Emitter> _emitters;

        private Timer _timer;

        private readonly TimeSpan _idleDetectionFrequency;
        private readonly TimeSpan _idleDetectionThreshold;

        public MessageHandler(
            IHubConnector hubConnector,
            ITraceStorage traceStorage, 
            TimeSpan idleDetectionFrequency,
            TimeSpan idleDetectionThreshold)
        {
            _hubConnector = hubConnector;
            _traceStorage = traceStorage;
            _idleDetectionFrequency = idleDetectionFrequency;
            _idleDetectionThreshold = idleDetectionThreshold;

            _receivers = new List<Receiver>();
            _emitters = new List<Emitter>();

            _hubConnector.EmitterConnected += OnEmitterConnected;
            _hubConnector.ReceiverConnected += OnReceiverConnected;
            _hubConnector.MetricUpdated += OnMetricUpdated;
            _hubConnector.TraceReceived += OnTraceReceived;
            _hubConnector.ActorHeartbeated += OnActorHeartbeated;

            _timer = new Timer((e) => DetectIdles(), null, TimeSpan.FromMilliseconds(0), _idleDetectionFrequency);
        }

        private void OnActorHeartbeated(Actor actor)
        {
            actor.LastHeartbeatReceivedDate = DateTime.UtcNow;

        }

        private void OnTraceReceived(Trace trace)
        {
            if (!_emitters.Contains(trace.Emitter))
            {
                _hubConnector.NotifyError(trace.Emitter, new Error("Not Connected", "Must be connected before emitting", ErrorCode.NotConnected));
                return;
            }

            _traceStorage.StoreTrace(trace);

            foreach (var receiver in _receivers)
            {
                _hubConnector.NotifyTraceReceived(receiver, trace);
            }
        }

        private void OnMetricUpdated(Metric metric)
        {
            if (!_emitters.Contains(metric.Emitter))
            {
                _hubConnector.NotifyError(metric.Emitter, new Error("Not Connected", "Must be connected before emitting", ErrorCode.NotConnected));
                return;
            }

            foreach (var receiver in _receivers)
            {
                _hubConnector.NotifyMetricUpdated(receiver, metric);
            }
        }

        private void OnReceiverConnected(Receiver receiver)
        {
            receiver.LastHeartbeatReceivedDate = DateTime.UtcNow;
            _receivers.Add(receiver);
        }

        private void OnEmitterConnected(Emitter emitter)
        {
            emitter.LastHeartbeatReceivedDate = DateTime.UtcNow;
            emitter.LastMessageEmittedDate = DateTime.UtcNow;
            _emitters.Add(emitter);
        }

        private void DetectIdles()
        {
            foreach (var emitter in _emitters.ToList())
            {
                if (DateTime.UtcNow - emitter.LastHeartbeatReceivedDate > _idleDetectionThreshold)
                {
                    _emitters.Remove(emitter);
                    _hubConnector.NotifyDisconnection(emitter);
                }
            }

            foreach (var receiver in _receivers.ToList())
            {
                if (DateTime.UtcNow - receiver.LastHeartbeatReceivedDate > _idleDetectionThreshold)
                {
                    _receivers.Remove(receiver);
                    _hubConnector.NotifyDisconnection(receiver);
                }
            }
        }
    }
}
