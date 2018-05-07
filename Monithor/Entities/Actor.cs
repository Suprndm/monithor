using System;

namespace Monithor.Entities
{
    public abstract class Actor
    {
        protected Actor(string name, string id)
        {
            Name = name;
            Id = id;
        }

        public string Name { get; }

        public string Id { get; }


        public DateTime? LastHeartbeatReceivedDate { get; set; }
    }
}
