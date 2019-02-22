using scp4aiur;
using Smod2.EventHandlers;
using Smod2.Events;

namespace SCP575
{
    public class EventsHandler : IEventHandlerRoundStart, IEventHandlerPlayerTriggerTesla, IEventHandlerWaitingForPlayers
    {
        private readonly SCP575 plugin;
        public EventsHandler(SCP575 plugin) => this.plugin = plugin;


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
            plugin.Debug("Getting 079 rooms.");
            Functions.Get079Rooms();
            plugin.Debug("Initial Delay: " + SCP575.delayTime + "s.");
            Timing.Run(Functions.EnableTimer(SCP575.delayTime));
            if (SCP575.toggle)
            {
                SCP575.timed_override = true;
                SCP575.Timed = false;
                Timing.Run(Functions.RunBlackout(0));
            }
        }

        // public void OnUpdate()
        // {
        //     float ttl = 0f;
        //     ttl += Time.deltaTime;
        //     if (SCP575.Timed || SCP575.toggle && (ttl > 11))
        //     {
        //         Functions.RunBlackout();
        //         ttl = ttl - 11;
        //     }
        //     else if (SCP575.Timed && !SCP575.firstTimer && ttl > 11)
        //     {
        //         Timing.Run(Functions.EnableTimer(SCP575.waitTime));
        //     }
        //     else if (SCP575.firstTimer && SCP575.Timed && ttl > 11)
        //     {
        //         Timing.Run(Functions.EnableFirstTimer(SCP575.delayTime));
        //     }
        // }

        public void OnPlayerTriggerTesla(PlayerTriggerTeslaEvent ev)
        {
            if (!SCP575.tesla)
            {
                ev.Triggerable = false;
            }
        }
    }
}