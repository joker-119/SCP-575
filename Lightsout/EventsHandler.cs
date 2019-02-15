using scp4aiur;
using Smod2.EventHandlers;
using Smod2.Events;
using System;
using System.Timers;

namespace Lightsout
{
    public class EventsHandler : IEventHandlerRoundStart, IEventHandlerRoundEnd, IEventHandlerRoundRestart, IEventHandlerPlayerTriggerTesla, IEventHandlerWaitingForPlayers
    {
        private readonly Lightsout plugin;
        public EventsHandler(Lightsout plugin) => this.plugin = plugin;

        public static Timer timer;
        public static bool tesla = true;

        public void OnWaitingForPlayers(WaitingForPlayersEvent ev)
        {
            Lightsout.validRanks = this.plugin.GetConfigList("lo_ranks");
            Lightsout.delayTime = this.plugin.GetConfigInt("lo_delay");
            Lightsout.waitTime = this.plugin.GetConfigInt("lo_wait");
            Lightsout.durTime = this.plugin.GetConfigInt("lo_duration");
            Lightsout.Timed = this.plugin.GetConfigBool("lo_timed");
            Lightsout.announce = this.plugin.GetConfigBool("lo_announce");
            Lightsout.toggle_lcz = this.plugin.GetConfigBool("lo_toggle_lcz");
            Lightsout.timed_lcz = this.plugin.GetConfigBool("lo_timed_lcz");
        }

        public void OnRoundStart(RoundStartEvent ev)
        {
            timer = new Timer();
            timer.Interval = Lightsout.delayTime * 1000;
            timer.Elapsed += OnTimedEvent;
            timer.AutoReset = false;
            timer.Enabled = true;
            Lightsout.timed = false;
            tesla = true;
        }

        public void OnTimedEvent(Object source, System.Timers.ElapsedEventArgs e)
        {
            timer.Interval = Lightsout.waitTime * 1000;
            if (Lightsout.enabled && Lightsout.Timed)
            {
                Lightsout.timed = !Lightsout.timed;
                if (Lightsout.timed)
                {
                    if (Lightsout.announce)
                    {
                        PlayerManager.localPlayer.GetComponent<MTFRespawn>().CallRpcPlayCustomAnnouncement("HEAVY CONTAINMENT POWER SYSTEM FAILURE IN 3. . 2. . 1. . ", false);
                    }
                    Timing.Timer(Functions.Blackout, 8.7f);
                    timer.Interval = Lightsout.durTime * 1000;
                    tesla = !tesla;
                }
                else if (!Lightsout.timed)
                {
                    Timing.Timer(Functions.LightsOn, 10f);
                }
            }
        }

        public void OnRoundRestart(RoundRestartEvent ev)
        {
            if (Lightsout.timed)
            {
                Lightsout.timed = !Lightsout.timed;
            }
            timer.Interval = Lightsout.delayTime * 1000;
            timer.Enabled = false;
        }

        public void OnRoundEnd(RoundEndEvent ev)
        {
            if (Lightsout.timed)
            {
                Lightsout.timed = !Lightsout.timed;
            }
            timer.Interval = Lightsout.delayTime * 1000;
            timer.Enabled = false;
        }

        public void OnPlayerTriggerTesla(PlayerTriggerTeslaEvent ev)
        {
            if (!tesla)
            {
                ev.Triggerable = false;
            }
        }
    }
}