using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;
using Monithor.Definitions;

namespace Monithor.Client
{
    public class MonithorEmitter : MonithorActorBase, IMonithorEmitter
    {
        public MonithorEmitter(string baseUrl, string name) : base(baseUrl, name)
        {
        }

        public async Task Trace(MessageLevel level, MessageType type, string name, string message, string metaData)
        {
            try
            {
                await Connection.SendAsync("SendTrace", level, type, name, message, metaData);
            }
            catch (Exception e)
            {
                throw new MonithorClientException("Send trace failed", e);
            }
        }

        public async Task UpdateMetric(MessageLevel level, MessageType type, string name, string value)
        {
            try
            {
                await Connection.SendAsync("UpdateMetric", level, type, name, value);
            }
            catch (Exception e)
            {
                throw new MonithorClientException("Update metric failed", e);
            }
        }

        protected override string GetActorTypeName()
        {
            return "Emitter";
        }
    }
}
