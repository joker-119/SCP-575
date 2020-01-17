using System.Collections.Generic;
using EXILED;
using MEC;
using UnityEngine;

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
			if (plugin.Gen.Next(100) > plugin.SpawnChance)
				Coroutines.Add(Timing.RunCoroutine(plugin.RunBlackoutTimer()));
		}

		public void OnRoundEnd()
		{
			foreach (CoroutineHandle handle in Coroutines)
				Timing.KillCoroutines(handle);
		}

		public void OnWaitingForPlayers()
		{
			TeslasDisabled = false;
		}

		public void OnTriggerTesla(ref TriggerTeslaEvent ev)
		{
			if (TeslasDisabled)
				ev.Triggerable = false;
		}
	}
}