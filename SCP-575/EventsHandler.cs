using Smod2.EventHandlers;
using Smod2.Events;
using MEC;

namespace SCP575
{
	public class EventsHandler : IEventHandlerRoundStart, IEventHandlerPlayerTriggerTesla, IEventHandlerWaitingForPlayers, IEventHandlerRoundEnd, IEventHandlerGeneratorFinish
	{
		private readonly Scp575 plugin;

		public EventsHandler(Scp575 plugin) => this.plugin = plugin;

		public void OnWaitingForPlayers(WaitingForPlayersEvent ev)
		{
			plugin.Vars.TimerOn = false;
			plugin.Vars.TriggerKill = false;
		}

		public void OnRoundStart(RoundStartEvent ev)
		{
			plugin.Debug("Getting 079 rooms.");
			plugin.Functions.Get079Rooms();

			plugin.Debug("Initial Delay: " + plugin.Vars.DelayTime + "s.");
			if (plugin.Vars.Toggled)
			{
				if (plugin.Vars.TimedEvents)
					plugin.Vars.TimedOverride = true;

				plugin.Vars.TimedEvents = false;
				plugin.Vars.Toggled = true;
				plugin.Coroutines.Add(Timing.RunCoroutine(plugin.Functions.TimedBlackout(0)));
			}
			else if (plugin.Vars.TimedEvents) plugin.Coroutines.Add(Timing.RunCoroutine(plugin.Functions.TimedBlackout(plugin.Vars.DelayTime)));
		}

		public void OnRoundEnd(RoundEndEvent ev)
		{
			foreach (CoroutineHandle handle in plugin.Coroutines) Timing.KillCoroutines(handle);
			plugin.Coroutines.Clear();
			plugin.Vars.TriggerKill = false;
			plugin.Vars.TimerOn = false;
		}

		public void OnPlayerTriggerTesla(PlayerTriggerTeslaEvent ev)
		{
			if (plugin.Vars.TimerOn && plugin.Vars.TimedTeslaDisable || plugin.Vars.Toggled && plugin.Vars.ToggleTeslaDisable) 
				ev.Triggerable = false;
		}

		public void OnGeneratorFinish(GeneratorFinishEvent ev)
		{
			plugin.Vars.GenCount++;
			plugin.Info(plugin.Vars.GenCount.ToString());

			plugin.Vars.BlackoutRoom.Remove(ev.Generator.Room);

			if (plugin.Vars.GenCount != 5) return;
			
			foreach (CoroutineHandle handle in plugin.Coroutines)
				Timing.KillCoroutines(handle);

			plugin.Server.Map.AnnounceScpKill("575");
		}
	}
}