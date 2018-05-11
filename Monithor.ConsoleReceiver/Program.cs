using System;
using System.Threading.Tasks;
using Monithor.Client;

namespace Monithor.ConsoleReceiver
{
    class Program
    {
        static void Main(string[] args)
        {
            MainAsync().Wait();
            Console.Read();
        }

        static async Task MainAsync()
        {
            var displayer = new Displayer();

            var monithorReceiver = new MonithorReceiver("http://localhost:52903/thorhub", "ConsoleReceiver");

            monithorReceiver.Connected += () => { displayer.LogTrace("Connected !"); };
            monithorReceiver.Disconnected += () => { displayer.LogTrace("Disconnected =( "); };
            monithorReceiver.ConnectionStatusChanged += (status) => { displayer.LogTrace($"Connection status changed to {status}"); };

            try
            {
                 monithorReceiver.Connect();

            }
            catch (MonithorClientException e)
            {
                displayer.LogException("monithor receiver error occured =(", e);
            }

            monithorReceiver.TraceReceived  += (trace) => displayer.LogTrace(trace.Name);
            monithorReceiver.MetricUpdated  += (metric) => displayer.LogTrace($" {metric.Name} : {metric.Value}");
        }
    }
}
