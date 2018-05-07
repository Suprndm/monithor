using Monithor.Definitions;
using Monithor.Entities;

namespace Monithor.Dtos
{
    public class Trace
    {
        public Trace(Emitter emitter, MessageLevel level, MessageType type, string name, string message, string metadata)
        {
            Emitter = emitter;
            Level = level;
            Type = type;
            Name = name;
            Message = message;
            Metadata = metadata;
        }

        public Emitter Emitter { get; }
        public MessageLevel Level { get; }
        public MessageType Type { get; }
        public string Name { get; }
        public string Message { get; }
        public string Metadata { get; }
    }
}
