using System.Collections.Generic;
using System.Threading.Tasks;
using Monithor.Api.Logging;
using Monithor.Definitions;
using Monithor.Dtos;
using Monithor.Entities;
using SignalRHelper.Server;

namespace Monithor.Api.Hub
{
    public class ThorHub : ManagedHub
    {
        private readonly IMessageHandler _messageHandler;
        private readonly IList<Emitter> _connectedEmitters;
        private readonly IList<Receiver> _connectedReceivers;
        private readonly ILogger _logger;

        public ThorHub(IMessageHandler messageHandler, ILogger logger)
        {
            _messageHandler = messageHandler;
            _logger = logger;
            _connectedEmitters= new List<Emitter>();
            _connectedReceivers = new List<Receiver>();
        }

        public override async Task OnConnectedAsync()
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, "SignalR Users");
            await base.OnConnectedAsync();
        }

        public void DeclareReceiver(string name)
        {
            var receiver = new Receiver(name, Context.ConnectionId);
            _connectedReceivers.Add(receiver);
            _messageHandler.ReceiverConnected(receiver);
        }

        public void DeclareEmitter(string name)
        {
            var emitter = new Emitter(name, Context.ConnectionId);
            _connectedEmitters.Add(emitter);
            _messageHandler.EmitterConnected(emitter);
        }

        public void SendTrace(MessageLevel level, MessageType type, string name, string message, string metaData)
        {
            var emitter = _messageHandler.GetActorById(Context.ConnectionId);
            var trace = new Trace((Emitter)emitter, level, type, name, message, message);
            _messageHandler.TraceReceived(trace);
        }


        public void UpdateMetric(MessageLevel level, MessageType type, string name, string value)
        {
            var emitter = _messageHandler.GetActorById(Context.ConnectionId);
            var metric = new Metric((Emitter)emitter, level, type, name, value);
            _messageHandler.MetricUpdated(metric);
        }
    }
}
