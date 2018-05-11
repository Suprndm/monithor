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
        private readonly ILogger _logger;
        private Timer _timer;


        public MessageHandler(
            IHub hub,
            ITraceStorage traceStorage,
            ILogger logger)
        {
            _hub = hub;
            _traceStorage = traceStorage;
            _logger = logger;

            _receivers = new List<Receiver>();
            _emitters = new List<Emitter>();
        }

        public void EmitterConnected(Emitter emitter)
        {
            emitter.LastMessageEmittedDate = DateTime.UtcNow;
            _emitters.Add(emitter);
            _logger.Log($"emitter connected : {emitter.Name} ({emitter.Id})");
        }

        public void ReceiverConnected(Receiver receiver)
        {
            _logger.Log($"receiver connected : {receiver.Name} : ({receiver.Id})");

            _receivers.Add(receiver);
        }

        public void SomeoneDisconnected(string clientId)
        {
            var actor = GetActorById(clientId);

            if (_receivers.Contains(actor))
            {
                _receivers.Remove((Receiver)actor);
                _logger.Log($"receiver disconnected : {actor.Name} ({actor.Id}) ");
            }

            else if (_emitters.Contains(actor))
            {
                _emitters.Remove((Emitter)actor);
                _logger.Log($"emitter disconnected : {actor.Name} ({actor.Id})");
            }
        }

        public void TraceReceived(Trace trace)
        {
            if (!_emitters.Contains(trace.Emitter))
            {
                var error = new Error("Not Connected", "Must be connected before emitting", ErrorCode.NotConnected);
                _hub.NotifyError(trace.Emitter, error);
                _logger.Log($"an error occured {error}");
                return;
            }

            _logger.Log($"trace received from {trace.Emitter.Name} ({trace.Emitter.Id})");

            _traceStorage.StoreTrace(trace);

            foreach (var receiver in _receivers)
            {
                _logger.Log($"trace sent to {receiver.Name} ({receiver.Id})");
                _hub.NotifyTraceReceived(receiver, trace);
            }
        }

        public void MetricUpdated(Metric metric)
        {
            if (!_emitters.Contains(metric.Emitter))
            {
                var error = new Error("Not Connected", "Must be connected before emitting", ErrorCode.NotConnected);
                _hub.NotifyError(metric.Emitter, error);
                _logger.Log($"an error occured {error}");
                return;
            }

            _logger.Log($"metric received from {metric.Emitter.Name}({ metric.Emitter.Id})");


            foreach (var receiver in _receivers)
            {
                _logger.Log($"metric sent to {receiver.Name} ({receiver.Id})");
                _hub.NotifyMetricUpdated(receiver, metric);
            }
        }

        public Actor GetActorById(string id)
        {
            var emitter = _emitters.SingleOrDefault(e => e.Id == id);

            if (emitter == null)
                return _receivers.Single(e => e.Id == id);

            return emitter;
        }
    }
}
