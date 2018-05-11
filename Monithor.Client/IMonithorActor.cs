using System;
using System.Threading.Tasks;
using Monithor.Dtos;

namespace Monithor.Client
{
    public interface IMonithorActor
    {
        event Action<Error> OnError;

        Task DisconnectAsync();

        void Connect();
    }
}
