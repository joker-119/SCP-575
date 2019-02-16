using scp4aiur;
using Smod2.EventHandlers;
using Smod2.Events;
using System;
using System.Timers;

namespace SCP575
{
    public class EventsHandler : IEventHandlerRoundStart, IEventHandlerRoundEnd, IEventHandlerRoundRestart, IEventHandlerPlayerTriggerTesla, IEventHandlerWaitingForPlayers
    {
        private readonly SCP575 plugin;
        public EventsHandler(SCP575 plugin) => this.plugin = plugin;

        public static Timer timer;
        public static bool tesla = true;

        public void OnWaitingForPlayers(WaitingForPlayersEvent ev)
        {
            SCP575.validRanks = this.plugin.GetConfigList("575_ranks");
            SCP575.delayTime = this.plugin.GetConfigInt("575_delay");
            SCP575.waitTime = this.plugin.GetConfigInt("575_wait");
            SCP575.durTime = this.plugin.GetConfigInt("575_duration");
            SCP575.Timed = this.plugin.GetConfigBool("575_timed");
            SCP575.announce = this.plugin.GetConfigBool("575_announce");
            SCP575.toggle_lcz = this.plugin.GetConfigBool("575_toggle_lcz");
            SCP575.timed_lcz = this.plugin.GetConfigBool("575_timed_lcz");
        }

        public void OnRoundStart(RoundStartEvent ev)
        {
            timer = new Timer();
            timer.Interval = SCP575.delayTime * 1000;
            timer.Elapsed += OnTimedEvent;
            timer.AutoReset = false;
            timer.Enabled = true;
            SCP575.timer = false;
            tesla = true;
        }

        public void OnTimedEvent(Object source, System.Timers.ElapsedEventArgs e)
        {
            timer.Interval = SCP575.waitTime * 1000;
            if (SCP575.enabled && SCP575.Timed)
            {
                SCP575.timer = !SCP575.timer;
                if (SCP575.timer)
                {
                    if (SCP575.announce)
                    {
                        if (!SCP575.timed_lcz)
                        {
                            PlayerManager.localPlayer.GetComponent<MTFRespawn>().CallRpcPlayCustomAnnouncement("HEAVY CONTAINMENT POWER SYSTEM FAILURE IN 3. . 2. . 1. . ", false);
                        }
                        else if (SCP575.timed_lcz)
                        {
                            PlayerManager.localPlayer.GetComponent<MTFRespawn>().CallRpcPlayCustomAnnouncement("FACILITY POWER SYSTEM FAILURE IN 3. . 2. . 1. . ", false);
                        }
                    }
                    Timing.Timer(Functions.Blackout, 8.7f);
                    if (SCP575.timed_lcz)
                    {
                        Timing.Timer(Functions.LightBlackout, 8.7f);
                    }
                    timer.Interval = SCP575.durTime * 1000;
                    tesla = !tesla;
                }
                else if (!SCP575.timer)
                {
                    Timing.Timer(Functions.LightsOn, 10f);
                }
            }
        }

        public void OnRoundRestart(RoundRestartEvent ev)
        {
            if (SCP575.timer)
            {
                SCP575.timer = !SCP575.timer;
            }
            timer.Interval = SCP575.delayTime * 1000;
            timer.Enabled = false;
        }

        public void OnRoundEnd(RoundEndEvent ev)
        {
            if (SCP575.timer)
            {
                SCP575.timer = !SCP575.timer;
            }
            timer.Interval = SCP575.delayTime * 1000;
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