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

			plugin.Debug("Initial Delay: " + plugin.DelayTime + "s.");
			if (plugin.Toggle)
			{
				plugin.TimedOverride = true;
				plugin.Timed = false;
				plugin.Toggle = true;
				plugin.coroutines.Add(Timing.RunCoroutine(plugin.Functions.TimedBlackout(0)));
			}
			else if (plugin.Timed)
			{
				plugin.coroutines.Add(Timing.RunCoroutine(plugin.Functions.TimedBlackout(plugin.DelayTime)));
			}
		}

		public void OnRoundRestart(RoundRestartEvent ev)
		{
			foreach (CoroutineHandle handle in plugin.coroutines) Timing.KillCoroutines(handle);
			plugin.coroutines.Clear();
			plugin.Timer = false;
			plugin.TriggerKill = false;
		}

		public void OnRoundEnd(RoundEndEvent ev)
		{
			foreach (CoroutineHandle handle in plugin.coroutines) Timing.KillCoroutines(handle);
			plugin.coroutines.Clear();
			plugin.TriggerKill = false;
			plugin.Timer = false;
		}

		public void OnPlayerTriggerTesla(PlayerTriggerTeslaEvent ev)
		{
			if ((plugin.Timer && plugin.TimedTesla) || (plugin.Toggle && plugin.ToggleTesla))
			{
				ev.Triggerable = false;
			}
		}
	}
}