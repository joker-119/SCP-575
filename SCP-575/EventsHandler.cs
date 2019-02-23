using System;
using scp4aiur;
using Smod2.EventHandlers;
using Smod2.Events;
using Smod2.API;
using UnityEngine;

namespace SCP575
{
    public class EventsHandler : IEventHandlerRoundStart, IEventHandlerPlayerTriggerTesla, IEventHandlerWaitingForPlayers, IEventHandlerMakeNoise, IEventHandlerPlayerJoin
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
        }

        public void OnRoundStart(RoundStartEvent ev)
        {
            plugin.Debug("Getting 079 rooms.");
            Functions.Get079Rooms();
            plugin.Debug("Initial Delay: " + SCP575.delayTime + "s.");
            Timing.Run(Functions.EnableTimer(SCP575.delayTime));
            foreach (Player player in ev.Server.GetPlayers())
            {
                if (!SCP575.canKeter.Contains(player.Name))
                {
                    SCP575.canKeter.Add(player.Name);
                }
            }
            if (SCP575.toggle)
            {
                SCP575.timed_override = true;
                SCP575.Timed = false;
                Timing.Run(Functions.RunBlackout(0));
            }
        }
        public void OnPlayerJoin(PlayerJoinEvent ev)
        {
            SCP575.canKeter.Add(ev.Player.Name);
        }
        public void OnMakeNoise(PlayerMakeNoiseEvent ev)
        {
            if (ev.Player.TeamRole.Team != Smod2.API.Team.SCP)
            {
                SCP575.plugin.Debug(ev.Player.Name + " made noise!");
                if (SCP575.enabled && (SCP575.timer || SCP575.toggle) && SCP575.canKeter.Contains(ev.Player.Name) && SCP575.keter)
                {
                    SCP575.plugin.Debug("Getting Player Position!");
                    if (ev.Player.GetCurrentItem().ItemType != ItemType.FLASHLIGHT && Functions.IsInDangerZone(ev.Player))
                    {
                        SCP575.plugin.Debug("Triggering Keter Damage!");
                        Timing.Run(Functions.KeterDamage(2.5f, ev.Player));
                    }
                }
            }
        }

        public void OnPlayerTriggerTesla(PlayerTriggerTeslaEvent ev)
        {
            if ((!SCP575.tesla && SCP575.timedTesla) || (SCP575.toggle && SCP575.toggleTesla))
            {
                ev.Triggerable = false;
            }
        }
    }
}