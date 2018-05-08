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

            try
            {
                await monithorReceiver.Connect();
            }
            catch (MonithorClientException e)
            {
                displayer.LogException("monithor receiver error occured =(", e);
            }

            monithorReceiver.TraceReceived += (trace) => displayer.LogTrace(trace.Name);
        }
    }
}
