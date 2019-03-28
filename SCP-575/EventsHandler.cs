using System;
using Smod2.EventHandlers;
using Smod2.Events;
using Smod2.API;
using MEC;
using System.Collections.Generic;

namespace SCP575
{
    public class EventsHandler : IEventHandlerRoundStart, IEventHandlerPlayerTriggerTesla, IEventHandlerWaitingForPlayers, IEventHandlerRoundEnd, IEventHandlerRoundRestart
    {
        private readonly SCP575 plugin;
        public EventsHandler(SCP575 plugin)
        {
            this.plugin = plugin;
            var f = new Functions(plugin);
        }
        private DateTime updateTimer = DateTime.Now;


        public void OnWaitingForPlayers(WaitingForPlayersEvent ev)
        {
            plugin.RefreshConfig();
            Functions.singleton.Get079Rooms();
        }

        public void OnRoundStart(RoundStartEvent ev)
        {
            plugin.Debug("Getting 079 rooms.");

            plugin.Debug("Initial Delay: " + plugin.delayTime + "s.");
            if (plugin.toggle)
            {
                plugin.timed_override = true;
                plugin.Timed = false;
                plugin.toggle = true;
                plugin.coroutines.Add(Timing.RunCoroutine(Functions.singleton.TimedBlackout(0)));
            }
            else if (plugin.Timed)
            {
                plugin.coroutines.Add(Timing.RunCoroutine(Functions.singleton.TimedBlackout(plugin.delayTime)));
            }
        }

        public void OnRoundRestart(RoundRestartEvent ev)
        {
            foreach (CoroutineHandle handle in plugin.coroutines) Timing.KillCoroutines(handle);
            plugin.coroutines.Clear();
            plugin.timer = false;
            plugin.triggerkill = false;
        }

        public void OnRoundEnd(RoundEndEvent ev)
        {
            foreach (CoroutineHandle handle in plugin.coroutines) Timing.KillCoroutines(handle);
            plugin.coroutines.Clear();
            plugin.triggerkill = false;
            plugin.timer = false;
        }

        public void OnPlayerTriggerTesla(PlayerTriggerTeslaEvent ev)
        {
            if ((plugin.timer && plugin.timedTesla) || (plugin.toggle && plugin.toggleTesla))
            {
                ev.Triggerable = false;
            }
        }
    }
}