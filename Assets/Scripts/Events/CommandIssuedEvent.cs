using GameDevTV.RTS.Commands;
using GameDevTV.RTS.EventBus;

namespace GameDevTV.RTS.Events
{
    public struct CommandIssuedEvent : IEvent
    {
        public BaseCommand Command { get; }

        public CommandIssuedEvent(BaseCommand command)
        {
            Command = command;
        }
    }
}