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

            try
            {
                await monithorEmitter.Connect();
            }
            catch (MonithorClientException e)
            {
                displayer.LogException("monithor emitter error occured =(", e);
            }

            do
            {
                await Task.Delay(10000);
                await monithorEmitter.Trace(MessageLevel.Verbose, MessageType.Communication, "Basic Trace",
                    "just been able to trace stuff", string.Empty);

            } while (true);
        }
    }
}
