using SignalRHelper.Server;

namespace Monithor.Api.Hub
{
    public class DisconnectionDetector
    {
        private readonly IMessageHandler _messageHandler;
        private readonly ILogger _logger;

        public DisconnectionDetector(IMessageHandler messageHandler, ILogger logger)
        {
            _messageHandler = messageHandler;
            _logger = logger;
        }

        public void Start()
        {
            HubConnectionManager.Instance.ClientDisconnected += Instance_ClientDisconnected;
        }

        private void Instance_ClientDisconnected(Client client)
        {
            _messageHandler.SomeoneDisconnected(client.Id);
        }
    }
}
