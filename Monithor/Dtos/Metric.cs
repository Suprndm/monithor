using Monithor.Definitions;
using Monithor.Entities;

namespace Monithor.Dtos
{
    public class Metric
    {
        public Metric(Emitter emitter, MessageLevel level, MessageType type, string name, string value)
        {
            Emitter = emitter;
            Level = level;
            Type = type;
            Name = name;
            Value = value;
        }

        public Emitter Emitter { get;}
        public MessageLevel Level { get; }
        public MessageType Type { get; }
        public string Name { get; }
        public string Value { get;  }
    }
}
