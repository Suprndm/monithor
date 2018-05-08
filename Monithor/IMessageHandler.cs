﻿using System;
using Monithor.Dtos;
using Monithor.Entities;

namespace Monithor
{
    public interface IMessageHandler
    {
        void EmitterConnected(Emitter emitter);
        void ReceiverConnected(Receiver receiver);
        void TraceReceived(Trace trace);
        void MetricUpdated(Metric metric);
        void ActorHeartbeated(Actor actor);

        Actor GetActorById(string id);
    }
}