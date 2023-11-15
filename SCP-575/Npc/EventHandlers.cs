namespace SCP_575.Npc
{
    using System.Collections.Generic;
    using Exiled.API.Features;
    using Exiled.Events.EventArgs.Server;
    using Exiled.Events.Handlers;
    using Exiled.Loader;
    using MEC;

    public class EventHandlers
    {
        private readonly Plugin _plugin;
        public EventHandlers(Plugin plugin) => _plugin = plugin;

        public List<CoroutineHandle> Coroutines = new List<CoroutineHandle>();
        public void OnRoundStart()
        {
            if (Loader.Random.Next(100) < _plugin.Config.NpcConfig.SpawnChance)
                Coroutines.Add(Timing.RunCoroutine(_plugin.Npc.Methods.RunBlackoutTimer()));
        }

        public void OnRoundEnd(RoundEndedEventArgs ev)
        {
            foreach (CoroutineHandle handle in Coroutines) Timing.KillCoroutines(handle);
            if (_plugin.Config.NpcConfig.DisableTeslas)
            {
                foreach (TeslaGate tg in TeslaGate.List) tg.CooldownTime = 0.5f;
            }

            Coroutines.Clear();
            _plugin.Npc.Methods.Disable();
        }

        // This shouldn't be necessary, but incase someone force-restarts a round, ig.
        public void OnWaitingForPlayers()
        {
            foreach (CoroutineHandle handle in Coroutines) Timing.KillCoroutines(handle);
            if (_plugin.Config.NpcConfig.DisableTeslas)
            {
                foreach (TeslaGate tg in TeslaGate.List) tg.ActivationTime = 0.5f;
            }

            Coroutines.Clear();
        }

    }
}
