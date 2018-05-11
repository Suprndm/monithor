using System;
using System.Threading.Tasks;
using Monithor.Components;
using Monithor.Definitions;
using Monithor.Dtos;
using Monithor.Entities;
using Monithor.Ports;
using NSubstitute;
using NUnit.Framework;

namespace Monithor.Tests
{
    public class MessageHandlerTests
    {
        private ITraceStorage _traceStorage;
        private IHub _hub;
        private MessageHandler _messageHandler;

        [SetUp]
        public void Setup()
        {
            _traceStorage = Substitute.For<ITraceStorage>();
            _hub = Substitute.For<IHub>();

            _messageHandler = new MessageHandler(_hub, _traceStorage, Substitute.For<ILogger>());
        }

        [Test]
        public void ShouldNotifyErrorWhenEmitterTraceButNotConnected()
        {
            // Given the emitter
            var emitter = CraftEmitter("A");
            var trace = CraftTrace(emitter);

            // When _hub receive trace
            _messageHandler.TraceReceived(trace);

            // Then message Handler should notify connection error
            _hub.Received(1).NotifyError(emitter, Arg.Is<Error>((e) => e.Code == ErrorCode.NotConnected));
        }

        [Test]
        public void ShouldNotifyErrorWhenEmitterMetricButNotConnected()
        {
            // Given the emitter
            var emitter = CraftEmitter("A");
            var metric = CraftMetric(emitter);

            // When _hub receive trace
            _messageHandler.MetricUpdated(metric);

            // Then message Handler should notify connection error
            _hub.Received(1).NotifyError(emitter, Arg.Is<Error>((e) => e.Code == ErrorCode.NotConnected));
        }


        [Test]
        public void ShouldDispatchMetricFromEmitterToReceivers()
        {
            // Given the connected emitter
            var emitter = CraftEmitter("A");
            _messageHandler.EmitterConnected(emitter);

            // Given the connected receivers
            var receiver1 = CraftReceiver("1");
            _messageHandler.ReceiverConnected(receiver1);
            var receiver2 = CraftReceiver("2");
            _messageHandler.ReceiverConnected(receiver2);
            var receiver3 = CraftReceiver("3");
            _messageHandler.ReceiverConnected(receiver3);

            // When emitter update metric
            var metric = CraftMetric(emitter);
            _messageHandler.MetricUpdated(metric);

            // Then the metric should be dispatched
            _hub.Received(1).NotifyMetricUpdated(receiver1, metric);
            _hub.Received(1).NotifyMetricUpdated(receiver2, metric);
            _hub.Received(1).NotifyMetricUpdated(receiver3, metric);
        }

        [Test]
        public void ShouldDispatchTraceFromEmitterToReceivers()
        {
            // Given the connected emitter
            var emitter = CraftEmitter("A");
            _messageHandler.EmitterConnected(emitter);

            // Given the connected receivers
            var receiver1 = CraftReceiver("1");
            _messageHandler.ReceiverConnected(receiver1);
            var receiver2 = CraftReceiver("2");
            _messageHandler.ReceiverConnected(receiver2);
            var receiver3 = CraftReceiver("3");
            _messageHandler.ReceiverConnected(receiver3);

            // When emitter update trace   
            var trace = CraftTrace(emitter);
            _messageHandler.TraceReceived(trace);

            // Then the trace should be stored
            _traceStorage.Received(1).StoreTrace(trace);

            // Then the trace should be dispatched
            _hub.Received(1).NotifyTraceReceived(receiver1, trace);
            _hub.Received(1).NotifyTraceReceived(receiver2, trace);
            _hub.Received(1).NotifyTraceReceived(receiver3, trace);
        }

        private Trace CraftTrace(Emitter emitter)
        {
            return new Trace(emitter, MessageLevel.Informational, MessageType.Communication, "emitter", "trace message", "metadata");
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
