namespace SCP_575.Playable
{
    using MEC;

    public class EventHandlers
    {
        private readonly Plugin _plugin;
        public EventHandlers(Plugin plugin) => _plugin = plugin;

        public void OnRoundStarted()
        {
            Timing.CallDelayed(0.75f, _plugin.Playable.Methods.TrySpawn575);
        }
    }
}