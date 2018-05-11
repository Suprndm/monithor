using System;
using System.Threading.Tasks;
using Monithor.Client;
using Monithor.Definitions;

namespace Monithor.ConsoleEmitter
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

            var monithorEmitter = new MonithorEmitter("http://localhost:52903/thorhub", "ConsoleEmitter");

            monithorEmitter.Connected += () => { displayer.LogTrace("Connected !"); };
            monithorEmitter.Disconnected += () => { displayer.LogTrace("Disconnected =( "); };
            monithorEmitter.ConnectionStatusChanged += (status) => { displayer.LogTrace($"Connection status changed to {status}"); };

            try
            {
                monithorEmitter.Connect();
            }
            catch (MonithorClientException e)
            {
                displayer.LogException("monithor emitter error occured =(", e);
            }

            do
            {
                await Task.Delay(3000);
                try
                {
                    await monithorEmitter.Trace(MessageLevel.Verbose, MessageType.Communication, "Basic Trace",
                        "just been able to trace stuff", string.Empty);
                }
                catch (MonithorClientException e)
                {
                    displayer.LogException("send trace failed", e);
                }
             

            } while (true);
        }
    }
}
