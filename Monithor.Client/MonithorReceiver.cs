using System;
using Microsoft.AspNetCore.SignalR.Client;
using Monithor.Dtos;

namespace Monithor.Client
{
    public class MonithorReceiver : MonithorActorBase , IMonithorReceiver
    {
        public MonithorReceiver(string baseUrl, string name) : base(baseUrl, name)
        {
        }

        protected override string GetActorTypeName()
        {
            return "Receiver";
        }

        protected override void OnConnected()
        {
            Connection.On<Trace>("TraceReceived", (trace) => TraceReceived?.Invoke(trace));
            Connection.On<Metric>("MetricUpdated", (metric) => MetricUpdated?.Invoke(metric));

            base.OnConnected();
        }

        public event Action<Trace> TraceReceived;
        public event Action<Metric> MetricUpdated;
    }
}
