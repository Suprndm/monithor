using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Logging;
using Monithor.Dtos;

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
            // Keep trying to until we can start
            var displayer = new Displayer();
            var connection = new HubConnectionBuilder()
                .WithUrl("http://localhost:52903/thorhub")
                .Build();
            try
            {
                await connection.StartAsync();
                displayer.LogTrace("Connected !");
            }
            catch (Exception e)
            {
                displayer.LogException("connection failed =(", e);
            }

            connection.On("Disconnection", () => { displayer.LogTrace("Disconnected because idle"); });
            connection.On<Error>("Error", (error) => { displayer.LogError(error.ToString()); });

            await connection.SendAsync("DeclareReceiver", "ConsoleReceiver");
        }
    }
}
