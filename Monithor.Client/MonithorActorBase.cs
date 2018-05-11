using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;
using Monithor.Definitions;
using Monithor.Dtos;
using SignalRHelper.Client;

namespace Monithor.Client
{
    public abstract class MonithorActorBase : IMonithorActor
    {
        protected HubConnection Connection => _client.HubConnection;

        private readonly string _name;
        private readonly SignalRClient _client;

        public event Action<Error> OnError;

        public event Action Connected;
        public event Action Disconnected;
        public event Action<ConnectionStatus> ConnectionStatusChanged;

        protected MonithorActorBase(string baseUrl, string name)
        {
            _name = name;
            _client = new SignalRClient(name, baseUrl, pingPongFrequencyMs:1000);
            _client.Connected += _client_Connected;
            _client.Disconnected += _client_Disconnected;
            _client.ConnectionStatusChanged += _client_ConnectionStatusChanged;
            _client.ExceptionOccured += _client_ExceptionOccured;
        }

        private void _client_ConnectionStatusChanged(SignalRHelper.Client.ConnectionStatus connectionStatus)
        {
            ConnectionStatusChanged?.Invoke(MapConnectionStatus(connectionStatus));
        }

        private void _client_Disconnected()
        {
            Disconnected?.Invoke();
        }

        private void _client_ExceptionOccured(SignalRClientException exception)
        {
            OnError?.Invoke(new Error("signalR exception occured", exception.ToString(), ErrorCode.NotConnected));
        }

        private void _client_Connected()
        {
            Connected?.Invoke();
            OnConnected();
        }

        private ConnectionStatus MapConnectionStatus(SignalRHelper.Client.ConnectionStatus connectionStatus)
        {
            switch (connectionStatus)
            {
                case SignalRHelper.Client.ConnectionStatus.Disconnected:
                    return ConnectionStatus.Disconnected;
                case SignalRHelper.Client.ConnectionStatus.Healthy:
                    return ConnectionStatus.Healthy;
                case SignalRHelper.Client.ConnectionStatus.Disturbed:
                    return ConnectionStatus.Disturbed;
                default:
                    throw new ArgumentOutOfRangeException(nameof(connectionStatus), connectionStatus, null);
            }
        }

        public Task DisconnectAsync()
        {
            return _client.DisconnectAsync();
        }

        public void Connect()
        {
            _client.Connect();
        }

        protected virtual async void OnConnected()
        {
            await Connection.SendAsync($"Declare{GetActorTypeName()}", _name);
        }

        protected abstract string GetActorTypeName();
    }
}
