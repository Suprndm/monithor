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

            var monithorEmitter = new MonithorEmitter("https://monithor.azurewebsites.net/thorhub", "ConsoleEmitter");

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
            Random random = new Random();
            int i = 0;
            do
            {
                await Task.Delay(200);
                try
                {
                    await monithorEmitter.UpdateMetric(MessageLevel.Verbose, MessageType.Communication, "posX",
                        i.ToString());
                    displayer.LogTrace($"sent metric posX {i}");
                    await monithorEmitter.UpdateMetric(MessageLevel.Verbose, MessageType.Communication, "posY",
                        i.ToString());
                    displayer.LogTrace($"sent metric posY {i}");
                    await monithorEmitter.UpdateMetric(MessageLevel.Verbose, MessageType.Communication, "posZ",
                        i.ToString());
                    displayer.LogTrace($"sent metric posZ {i}");


                    if (i % 7 == 0)
                    {
                        var criticity = random.Next(10);
                        MessageLevel level;
                        string message;

                        if (criticity <= 1)
                        {
                            level = MessageLevel.Error;
                            message = "An engine just crashed.";
                        }   
                        else if (criticity <= 4)
                        {
                            level = MessageLevel.Informational;
                            message = "Target position reached.";
                        }
                        else
                        {
                            level = MessageLevel.Verbose;
                            message = "Nothing detected in radar";
                        }

                        await monithorEmitter.Trace(level, MessageType.Communication, "trace", message, "");
                        displayer.LogTrace($"sent trace {i}");
                    }

                    i++;
                }
                catch (MonithorClientException e)
                {
                    displayer.LogException("send trace failed", e);
                }


            } while (true);
        }
    }
}
