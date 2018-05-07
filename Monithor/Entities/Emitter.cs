using System;

namespace Monithor.Entities
{
    public class Emitter : Actor
    {
        public Emitter(string name, string id) : base(name, id)
        {
        }

        public DateTime? LastMessageEmittedDate { get; set; }
    }
}
