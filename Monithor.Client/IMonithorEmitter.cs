using System.Threading.Tasks;
using Monithor.Definitions;

namespace Monithor.Client
{
    public interface IMonithorEmitter : IMonithorActor
    {
        Task Trace(MessageLevel level, MessageType type, string name, string message, string metaData);
        Task UpdateMetric(MessageLevel level, MessageType type, string name, string value);
    }
}
