using System.Collections.Generic;
using Exiled.Events.EventArgs;
using MEC;

namespace SCP_575
{
	public class EventHandlers
	{
		public Plugin plugin;
		public EventHandlers(Plugin plugin) => this.plugin = plugin;

		public bool TeslasDisabled = false;
		public List<CoroutineHandle> Coroutines = new List<CoroutineHandle>();

		public void OnRoundStart()
		{
			foreach (CoroutineHandle handle in Coroutines)
				Timing.KillCoroutines(handle);
			TeslasDisabled = false;
			if (plugin.Gen.Next(100) < plugin.Config.SpawnChance)
				Coroutines.Add(Timing.RunCoroutine(plugin.RunBlackoutTimer()));
		}

		public void OnRoundEnd(RoundEndedEventArgs ev)
		{
			foreach (CoroutineHandle handle in Coroutines)
				Timing.KillCoroutines(handle);
		}

		public void OnWaitingForPlayers()
		{
			foreach (CoroutineHandle handle in Coroutines)
				Timing.KillCoroutines(handle);
			TeslasDisabled = false;
		}

		public void OnTriggerTesla(TriggeringTeslaEventArgs ev)
		{
			if (TeslasDisabled)
				ev.IsTriggerable = false;
		}
	}
}
