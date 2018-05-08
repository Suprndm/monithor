using Microsoft.AspNetCore.SignalR;
using Monithor.Dtos;
using Monithor.Entities;
using Monithor.Ports;

namespace Monithor.Api.Hub
{
    public class HubInterface : IHub
    {
        private readonly IHubContext<ThorHub> _hubContext;

        public HubInterface(IHubContext<ThorHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public void NotifyTraceReceived(Receiver receiver, Trace trace)
        {
            _hubContext.Clients.Client(receiver.Id).SendAsync("OnTraceReceived");
        }

        public void NotifyMetricUpdated(Receiver receiver, Metric metric)
        {
            _hubContext.Clients.Client(receiver.Id).SendAsync("OnMetricUpdated");
        }

        public void NotifyDisconnection(Actor actor)
        {
            _hubContext.Clients.Client(actor.Id).SendAsync("Disconnection");
        }

        public void NotifyError(Actor actor, Error error)
        {
            _hubContext.Clients.Client(actor.Id).SendAsync("Error", error);
        }
    }
}
