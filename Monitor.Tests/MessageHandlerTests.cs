using System;
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

        [SetUp]
        public void Setup()
        {
            _hubConnector = Substitute.For<IHubConnector>();
            _traceStorage = Substitute.For<ITraceStorage>();

            _messageHandler = new MessageHandler(_hubConnector, _traceStorage, TimeSpan.FromMilliseconds(100), TimeSpan.FromMilliseconds(500));
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
