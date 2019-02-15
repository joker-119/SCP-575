using scp4aiur;
using Smod2.EventHandlers;
using Smod2.Events;
using System;
using System.Timers;
using Smod2;
using Smod2.API;
using Smod2.Commands;

namespace Blackout
{
    public class EventHandlers : IEventHandlerRoundStart, IEventHandlerRoundEnd, IEventHandlerRoundRestart, IEventHandlerPlayerTriggerTesla, IEventHandlerWaitingForPlayers
    {
		public static Timer timer;
        public bool timed = false;
        public bool tesla = true;

        public void OnWaitingForPlayers(WaitingForPlayersEvent ev)
        {
            Plugin.validRanks = Plugin.instance.GetConfigList("blackout_ranks");
            Plugin.delayTime = Plugin.instance.GetConfigInt("blackout_delay");
            Plugin.waitTime = Plugin.instance.GetConfigInt("blackout_wait");
            Plugin.durTime = Plugin.instance.GetConfigInt("blackout_duration");
            Plugin.Timed = Plugin.instance.GetConfigBool("blackout_timer");
            Plugin.announce = Plugin.instance.GetConfigBool("blackout_light_announcments");
            Plugin.toggle_lcz = Plugin.instance.GetConfigBool("blackout_toggle_lcz");
            Plugin.timed_lcz = Plugin.instance.GetConfigBool("blackout_timed_lcz");
        }
        public void OnRoundStart(RoundStartEvent ev)
        {
            timer = new Timer();
            timer.Interval = Plugin.delayTime;
            timer.Elapsed += OnTimedEvent;
            timer.AutoReset = false;
            timer.Enabled = true;
            timed = false;
            tesla = true;
        }
        public void OnTimedEvent(Object source, System.Timers.ElapsedEventArgs e) {
            timer.Interval = Plugin.waitTime;
            if (Plugin.enabled) {
                if (Plugin.Timed){
                    timed = !timed;
                    if (timed) { 
                        if (Plugin.announce)
                        {
                            if (Plugin.timed_lcz)
                            {
                                PlayerManager.localPlayer.GetComponent<MTFRespawn>().CallRpcPlayCustomAnnouncement("FACILITY POWER SYSTEM FAILURE IN 3. . 2. . 1. . ", false);
                            }
                            else
                            {
                                PlayerManager.localPlayer.GetComponent<MTFRespawn>().CallRpcPlayCustomAnnouncement("HEAVY CONTAINMENT POWER SYSTEM FAILURE IN 3. . 2. . 1. . ", false);
                            }
                            
                        }
                        Timing.Timer(TimedBlackout, 8.7f);
                        // if (Plugin.timed_lcz)
                        // {
                        //     Timing.Timer(TimedLightBlackout, 8.7f);
                        // }
                        timer.Interval = Plugin.durTime;
                        tesla = !tesla;
                    }
                    if (!timed) {
                        Timing.Timer(LightsOn, 10f);
                    }
               }
            }            
        }
        private void TimedBlackout(float inaccuracy = 0) {
            Generator079.generators[0].CallRpcOvercharge();

            if (timed)  {
                Timing.Timer(TimedBlackout, 11 + inaccuracy);
                }
        }

        // private void TimedLightBlackout(float inaccuracy = 0)
        // {
        //     foreach (Room room in PluginManager.Manager.Server.Map.Get079InteractionRooms(Scp079InteractionType.CAMERA))
        //     {
        //         if (room.ZoneType == ZoneType.LCZ)
        //         {
        //             room.FlickerLights();
        //         }
        //     }
        //     if (timed && Plugin.timed_lcz)
        //     {
        //         Timing.Timer(TimedLightBlackout, 6 + inaccuracy);
        //     }
        // }
        private void LightsOn(float inaccuracy = 0) {
            if (Plugin.announce)
            {
                PlayerManager.localPlayer.GetComponent<MTFRespawn>().CallRpcPlayCustomAnnouncement("HEAVY CONTAINMENT POWER SYSTEM NOW OPERATIONAL", false);
            }
            tesla = !tesla;
        }

        public void OnRoundRestart(RoundRestartEvent ev) {
            if (timed) {
                timed = !timed;
            }
            timer.Interval = Plugin.delayTime;
            timer.Enabled = false;
        }
        public void OnRoundEnd(RoundEndEvent ev) {
            if (timed) {
                timed = !timed;
            }
            timer.Interval = Plugin.delayTime;
            timer.Enabled = false;
        }
        public void OnPlayerTriggerTesla(PlayerTriggerTeslaEvent ev) {
            if (!tesla) {
            ev.Triggerable = false;
            }
        }
    }
}
