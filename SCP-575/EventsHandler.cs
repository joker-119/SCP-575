using System;
using Smod2.EventHandlers;
using Smod2.Events;
using Smod2.API;
using UnityEngine;

namespace SCP575
{
    public class EventsHandler : IEventHandlerRoundStart, IEventHandlerPlayerTriggerTesla, IEventHandlerWaitingForPlayers, IEventHandlerRoundEnd, IEventHandlerRoundRestart
    {
        private readonly SCP575 plugin;
        public EventsHandler(SCP575 plugin) => this.plugin = plugin;
        DateTime updateTimer = DateTime.Now;


        public void OnWaitingForPlayers(WaitingForPlayersEvent ev)
        {
            SCP575.validRanks = this.plugin.GetConfigList("575_ranks");
            SCP575.delayTime = this.plugin.GetConfigFloat("575_delay");
            SCP575.waitTime = this.plugin.GetConfigFloat("575_wait");
            SCP575.durTime = this.plugin.GetConfigFloat("575_duration");
            SCP575.Timed = this.plugin.GetConfigBool("575_timed");
            SCP575.announce = this.plugin.GetConfigBool("575_announce");
            SCP575.toggle_lcz = this.plugin.GetConfigBool("575_toggle_lcz");
            SCP575.timed_lcz = this.plugin.GetConfigBool("575_timed_lcz");
            SCP575.timedTesla = this.plugin.GetConfigBool("575_timed_tesla");
            SCP575.toggleTesla = this.plugin.GetConfigBool("575_toggle_tesla");
            SCP575.keter = this.plugin.GetConfigBool("575_keter");
            SCP575.KeterDamage = this.plugin.GetConfigInt("575_keter_damage");
            SCP575.toggleketer = this.plugin.GetConfigBool("575_keter_toggle");
            SCP575.keterkill = this.plugin.GetConfigBool("575_keter_kill");
            SCP575.keterkill_num = this.plugin.GetConfigInt("575_keter_kill_num");
            SCP575.timer = false;
            SCP575.triggerkill = false;
        }

        public void OnRoundStart(RoundStartEvent ev)
        {
            plugin.Debug("Getting 079 rooms.");
            Functions.Get079Rooms();
            plugin.Debug("Initial Delay: " + SCP575.delayTime + "s.");
            plugin.StartCoroutine(Functions.TimedBlackout(SCP575.delayTime));
            if (SCP575.toggle)
            {
                SCP575.timed_override = true;
                SCP575.Timed = false;
                plugin.StartCoroutine(Functions.ToggledBlackout(0));
            }
        }
        public void OnRoundRestart(RoundRestartEvent ev)
        {
            SCP575.timer = false;
            SCP575.triggerkill = false;
            
        }
        public void OnRoundEnd(RoundEndEvent ev)
        {
            SCP575.triggerkill = false;
            SCP575.timer = false;
        }
        public void OnPlayerTriggerTesla(PlayerTriggerTeslaEvent ev)
        {
            if ((SCP575.timer && SCP575.timedTesla) || (SCP575.toggle && SCP575.toggleTesla))
            {
                ev.Triggerable = false;
            }
        }
    }
}