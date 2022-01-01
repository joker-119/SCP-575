namespace SCP_575.NestingObjects
{
    using SCP_575.Playable;

    public class Playable
    {
        public Methods Methods { get; set; }
        public EventHandlers EventHandlers { get; set; }

        public Playable(Plugin plugin)
        {
            Methods = new Methods(plugin);
            EventHandlers = new EventHandlers(plugin);
        }
    }
}