namespace SCP_575.NestingObjects
{
    using SCP_575.Npc;

    public class Npc
    {
        public Methods Methods { get; set; }
        public EventHandlers EventHandlers { get; set; }

        public Npc(Plugin plugin)
        {
            Methods = new Methods(plugin);
            EventHandlers = new EventHandlers(plugin);
        }
    }
}