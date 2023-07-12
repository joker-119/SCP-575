namespace SCP_575.Npc
{
    using System.Collections.Generic;
    using Exiled.API.Features;
    using Exiled.Events.EventArgs.Player;
    using Exiled.Events.EventArgs.Server;
    using Exiled.Events.Handlers;
    using Exiled.Loader;
    using MEC;
    using PluginAPI.Events;

    public class EventHandlers
    {
        private readonly Plugin _plugin;
        public EventHandlers(Plugin plugin) => _plugin = plugin;

        public bool TeslasDisabled;
        public bool NukeDisabled;
        public List<CoroutineHandle> Coroutines = new List<CoroutineHandle>();

        public void OnRoundStart()
        {
            if (Loader.Random.Next(100) < _plugin.Config.NpcConfig.SpawnChance)
                Coroutines.Add(Timing.RunCoroutine(_plugin.Npc.Methods.RunBlackoutTimer()));
        }

        public void OnRoundEnd(RoundEndedEventArgs ev)
        {
            foreach (CoroutineHandle handle in Coroutines)
                Timing.KillCoroutines(handle);
            TeslasDisabled = false;
            NukeDisabled = false;
            Coroutines.Clear();
            _plugin.Npc.Methods.Disable();
        }

        // This shouldn't be necessary, but incase someone force-restarts a round, ig.
        public void OnWaitingForPlayers()
        {
            foreach (CoroutineHandle handle in Coroutines)
                Timing.KillCoroutines(handle);
            TeslasDisabled = false;
            NukeDisabled = false;
            Coroutines.Clear();
        }

        public void OnTriggerTesla(TriggeringTeslaEventArgs ev)
        {
            if (TeslasDisabled)
            {
                ev.IsAllowed = false;
            }
        }

        public void onTriggerNukeStart(ActivatingWarheadPanelEventArgs ev)
        {
            if (NukeDisabled)
            {
                ev.IsAllowed = false;
            }

        }

    }
}
