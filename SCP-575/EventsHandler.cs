using Smod2.EventHandlers;
using Smod2.Events;
using MEC;

namespace SCP575
{
	public class EventsHandler : IEventHandlerRoundStart, IEventHandlerPlayerTriggerTesla, IEventHandlerWaitingForPlayers, IEventHandlerRoundEnd, IEventHandlerRoundRestart
	{
		private readonly SCP575 plugin;

		public EventsHandler(SCP575 plugin)
		{
			this.plugin = plugin;
		}

		public void OnWaitingForPlayers(WaitingForPlayersEvent ev)
		{
			plugin.RefreshConfig();
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
				plugin.coroutines.Add(Timing.RunCoroutine(plugin.Functions.TimedBlackout(0)));
			}
			else if (plugin.Vars.TimedEvents)
			{
				plugin.coroutines.Add(Timing.RunCoroutine(plugin.Functions.TimedBlackout(plugin.Vars.DelayTime)));
			}
		}

		public void OnRoundRestart(RoundRestartEvent ev)
		{
			foreach (CoroutineHandle handle in plugin.coroutines) Timing.KillCoroutines(handle);
			plugin.coroutines.Clear();
			plugin.Vars.TimerOn = false;
			plugin.Vars.TriggerKill = false;
		}

		public void OnRoundEnd(RoundEndEvent ev)
		{
			foreach (CoroutineHandle handle in plugin.coroutines) Timing.KillCoroutines(handle);
			plugin.coroutines.Clear();
			plugin.Vars.TriggerKill = false;
			plugin.Vars.TimerOn = false;
		}

		public void OnPlayerTriggerTesla(PlayerTriggerTeslaEvent ev)
		{
			if ((plugin.Vars.TimerOn && plugin.Vars.TimedTeslaDisable) || (plugin.Vars.Toggled && plugin.Vars.ToggleTeslaDisable))
			{
				ev.Triggerable = false;
			}
		}
	}
}