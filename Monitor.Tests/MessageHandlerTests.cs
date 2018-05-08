using System;
using System.Threading.Tasks;
using Monithor;
using Monithor.Components;
using Monithor.Definitions;
using Monithor.Dtos;
using Monithor.Entities;
using Monithor.Ports;
using NSubstitute;
using NUnit.Framework;

namespace Monitor.Tests
{
    public class MessageHandlerTests
    {
        private IHubConnector _hubConnector;
        private ITraceStorage _traceStorage;
        private MessageHandler _messageHandler;
        private readonly TimeSpan _idleDetectionFrequency = TimeSpan.FromMilliseconds(100);
        private readonly TimeSpan _idleDetectionThreshold = TimeSpan.FromMilliseconds(500);

        [SetUp]
        public void Setup()
        {
            _hubConnector = Substitute.For<IHubConnector>();
            _traceStorage = Substitute.For<ITraceStorage>();

            _messageHandler = new MessageHandler(_hubConnector, _traceStorage, _idleDetectionFrequency, _idleDetectionThreshold);
        }

        [Test]
        public void ShouldNotifyErrorWhenEmitterTraceButNotConnected()
        {
            // Given the emitter
            var emitter = CraftEmitter("A");
            var trace = CraftTrace(emitter);

            // When _hubConnector receive trace
            _hubConnector.TraceReceived += Raise.Event<Action<Trace>>(trace);

            // Then message Handler should notify connection error
            _hubConnector.Received(1).NotifyError(emitter, Arg.Is<Error>((e)=> e.Code==ErrorCode.NotConnected));
        }

        [Test]
        public void ShouldNotifyErrorWhenEmitterMetricButNotConnected()
        {
            // Given the emitter
            var emitter = CraftEmitter("A");
            var metric = CraftMetric(emitter);

            // When _hubConnector receive trace
            _hubConnector.MetricUpdated += Raise.Event<Action<Metric>>(metric);

            // Then message Handler should notify connection error
            _hubConnector.Received(1).NotifyError(emitter, Arg.Is<Error>((e) => e.Code == ErrorCode.NotConnected));
        }


        [Test]
        public void ShouldDispatchMetricFromEmitterToReceivers()
        {
            // Given the connected emitter
            var emitter = CraftEmitter("A");
            _hubConnector.EmitterConnected += Raise.Event<Action<Emitter>>(emitter);

            // Given the connected receivers
            var receiver1 = CraftReceiver("1");
            _hubConnector.ReceiverConnected += Raise.Event<Action<Receiver>>(receiver1);
            var receiver2 = CraftReceiver("2");
            _hubConnector.ReceiverConnected += Raise.Event<Action<Receiver>>(receiver2);
            var receiver3 = CraftReceiver("3");
            _hubConnector.ReceiverConnected += Raise.Event<Action<Receiver>>(receiver3);

            // When emitter update metric
            var metric = CraftMetric(emitter);
            _hubConnector.MetricUpdated += Raise.Event<Action<Metric>>(metric);

            // Then the metric should be dispatched
            _hubConnector.Received(1).NotifyMetricUpdated(receiver1, metric);
            _hubConnector.Received(1).NotifyMetricUpdated(receiver2, metric);
            _hubConnector.Received(1).NotifyMetricUpdated(receiver3, metric);
        }

        [Test]
        public void ShouldDispatchTraceFromEmitterToReceivers()
        {
            // Given the connected emitter
            var emitter = CraftEmitter("A");
            _hubConnector.EmitterConnected += Raise.Event<Action<Emitter>>(emitter);

            // Given the connected receivers
            var receiver1 = CraftReceiver("1");
            _hubConnector.ReceiverConnected += Raise.Event<Action<Receiver>>(receiver1);
            var receiver2 = CraftReceiver("2");
            _hubConnector.ReceiverConnected += Raise.Event<Action<Receiver>>(receiver2);
            var receiver3 = CraftReceiver("3");
            _hubConnector.ReceiverConnected += Raise.Event<Action<Receiver>>(receiver3);

            // When emitter update trace   
            var trace = CraftTrace(emitter);
            _hubConnector.TraceReceived += Raise.Event<Action<Trace>>(trace);

            // Then the trace should be stored
            _traceStorage.Received(1).StoreTrace(trace);

            // Then the trace should be dispatched
            _hubConnector.Received(1).NotifyTraceReceived(receiver1, trace);
            _hubConnector.Received(1).NotifyTraceReceived(receiver2, trace);
            _hubConnector.Received(1).NotifyTraceReceived(receiver3, trace);
        }


        [Test]
        public void ShouldDisconnectEmitterIfIdleForTooLong()
        {
            // Given the emitter
            var emitter = CraftEmitter("A");
            var metric = CraftMetric(emitter);
            _hubConnector.EmitterConnected += Raise.Event<Action<Emitter>>(emitter);

            // When too much time passes
            Task.Delay((int)_idleDetectionThreshold.TotalMilliseconds * 2).Wait();

            // Then message Handler should notify disconnection
            _hubConnector.Received(1).NotifyDisconnection(emitter);

            // Then emitter no longer can emit
            _hubConnector.MetricUpdated += Raise.Event<Action<Metric>>(metric);
            _hubConnector.Received(1).NotifyError(emitter, Arg.Is<Error>((e) => e.Code == ErrorCode.NotConnected));
        }

        [Test]
        public void ShouldDisconnectReceiverIfIdleForTooLong()
        {
            // Given the receiver
            var receiver = CraftReceiver("1");
            _hubConnector.ReceiverConnected += Raise.Event<Action<Receiver>>(receiver);

            // When too much time passes
            Task.Delay((int)_idleDetectionThreshold.TotalMilliseconds *2).Wait();

            // Then message Handler should notify disconnection
            _hubConnector.Received(1).NotifyDisconnection(receiver);

            // Then emitter no longer can emit
            var emitter = CraftEmitter("A");
            var metric = CraftMetric(emitter);
            _hubConnector.EmitterConnected += Raise.Event<Action<Emitter>>(emitter);
            _hubConnector.MetricUpdated += Raise.Event<Action<Metric>>(metric);

            _hubConnector.Received(0).NotifyMetricUpdated(receiver, metric);
        }

        [Test]
        public void ShouldNotDisconnectReceiverIfHeartbeated()
        {
            // Given the receiver
            var receiver = CraftReceiver("1");
            _hubConnector.ReceiverConnected += Raise.Event<Action<Receiver>>(receiver);

            // When receiver send regular heartbeats
            for (int i = 0; i < 10; i++)
            {
                _hubConnector.ActorHeartbeated += Raise.Event<Action<Actor>>(receiver);
                Task.Delay(200);
            }

            // Then receiver should not be disconnected
            _hubConnector.Received(0).NotifyDisconnection(receiver);
        }

        [Test]
        public void ShouldNotDisconnectEmitterIfHeartbeated()
        {
            // Given the emitter
            var emitter = CraftEmitter("1");
            _hubConnector.EmitterConnected += Raise.Event<Action<Emitter>>(emitter);

            // When emitter send regular heartbeats
            for (int i = 0; i < 10; i++)
            {
                _hubConnector.ActorHeartbeated += Raise.Event<Action<Actor>>(emitter);
                Task.Delay(200);
            }

            // Then emitter should not be disconnected
            _hubConnector.Received(0).NotifyDisconnection(emitter);
        }

        private Trace CraftTrace(Emitter emitter)
        {
            return  new Trace(emitter, MessageLevel.Informational, MessageType.Communication, "emitter","trace message", "metadata");
        }

        private Metric CraftMetric(Emitter emitter)
        {
            return new Metric(emitter, MessageLevel.Informational, MessageType.Communication, "emitter", "trace");
        }

        private Emitter CraftEmitter(string id)
        {
            return new Emitter("emitter", id);
        }

        private Receiver CraftReceiver(string id)
        {
            return new Receiver("receiver", id);
        }
    }
}
