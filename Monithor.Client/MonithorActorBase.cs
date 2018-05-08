using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;
using Monithor.Dtos;

namespace Monithor.Client
{
    public abstract class MonithorActorBase : IMonithorActor
    {
        private readonly string _baseUrl;
        private readonly string _name;
        private readonly TimeSpan _heartbeatFrequency = TimeSpan.FromMilliseconds(1000);
        private bool _isConnected;
        private Timer _timer;
        protected HubConnection Connection { get; private set; }

        public event Action<Error> OnError;

        protected MonithorActorBase(string baseUrl, string name)
        {
            _baseUrl = baseUrl;
            _name = name;
        }

        private void Initialize()
        {
            _isConnected = false;
            Connection.On("Disconnection", () => throw new MonithorClientException($"Client disconnected because idle"));
            Connection.On<Error>("Error", (error) => throw new MonithorClientException($"Server return error : {error.ToString()}"));
        }

        private void Heartbeat()
        {
            try
            {
                Connection.SendAsync("Heartbeat").Wait();
            }
            catch (Exception e)
            {
                throw new MonithorClientException($"HeartBeat failed. Reason :", e);
            }
        }

        protected async Task EnsureConnection()
        {
            if (_isConnected) return;

            await Connect();
        }

        public async Task Connect()
        {

            Connection = new HubConnectionBuilder()
                .WithUrl(_baseUrl)
                .Build();

            try
            {
                await Connection.StartAsync();

                Initialize();
                OnConnected();

                await Connection.SendAsync($"Declare{GetActorTypeName()}", _name);
                _isConnected = true;
                _timer = new Timer((e) => Heartbeat(), null, TimeSpan.FromMilliseconds(0), _heartbeatFrequency);
            }
            catch (Exception e)
            {
                throw new MonithorClientException($"Connection to {_baseUrl} failed. Reason :", e);
            }
        }

        protected virtual void OnConnected()
        {

        }

        protected abstract string GetActorTypeName();

        public void Disconnect()
        {
            _timer.Dispose();
            _isConnected = false;
        }
    }
}
