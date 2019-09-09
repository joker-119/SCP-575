using Smod2.EventHandlers;
using Smod2.Events;
using MEC;

namespace SCP575
{
	public class EventsHandler : IEventHandlerRoundStart, IEventHandlerPlayerTriggerTesla,
		IEventHandlerWaitingForPlayers, IEventHandlerRoundEnd, IEventHandlerGeneratorFinish, IEventHandlerWarheadStartCountdown, IEventHandlerWarheadStopCountdown
	{
		private readonly Scp575 plugin;

		public EventsHandler(Scp575 plugin) => this.plugin = plugin;

		public void OnWaitingForPlayers(WaitingForPlayersEvent ev)
		{
			plugin.Vars.TimerOn = false;
			plugin.Vars.TriggerKill = false;
			plugin.Vars.GenCount = 0;
			plugin.Vars.BlackoutRoom.Clear();
		}

		public void OnRoundStart(RoundStartEvent ev)
		{
			plugin.Info("Getting 079 rooms.");
			plugin.Functions.Get079Rooms();

			plugin.Info("Initial Delay: " + plugin.DelayTime + "s.");
			if (plugin.Vars.Toggled)
			{
				if (plugin.TimedEvents)
					plugin.Vars.TimedOverride = true;

				plugin.TimedEvents = false;
				plugin.Vars.Toggled = true;
				plugin.Coroutines.Add(Timing.RunCoroutine(plugin.Functions.TimedBlackout(0)));
			}
			else if (plugin.TimedEvents) plugin.Coroutines.Add(Timing.RunCoroutine(plugin.Functions.TimedBlackout(plugin.DelayTime)));
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
			if (plugin.Vars.DisableTeslas && plugin.TimedTeslaDisable || plugin.Vars.DisableTeslas && plugin.ToggleTeslaDisable)
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

		public void OnStartCountdown(WarheadStartEvent ev)
		{
			plugin.Vars.WarheadCounting = true;
		}

		public void OnStopCountdown(WarheadStopEvent ev)
		{
			plugin.Vars.WarheadCounting = false;
		}
	}
}