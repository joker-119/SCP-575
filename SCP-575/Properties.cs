using System;
using System.Collections.Generic;
using Smod2.API;
using MEC;

namespace SCP575
{
	public class Properties
	{
		private readonly SCP575 plugin;
		public Properties(SCP575 plugin) => this.plugin = plugin;


		public string[] ValidRanks { get; internal set; }

		public List<Room> BlackoutRoom = new List<Room>();
		public List<Room> Rooms = new List<Room>();

		public float WaitTime { get; internal set; }
		public float DurTime { get; internal set; }
		public float DelayTime { get; internal set; }


		public bool TimedEvents { get; set; }
		public bool Announce { get; set; }
		public bool Toggled { get; set; }
		public bool ToggledLcz { get; internal set; }
		public bool TimedLcz { get; internal set; }
		public bool TimerOn { get; set; } = false;
		public bool TimedOverride { get; set; } = false;
		public bool ToggleTeslaDisable { get; internal set; }
		public bool TimedTeslaDisable { get; internal set; }
		public bool Keter { get; internal set; }
		public bool TriggerKill { get; set; }
		public bool ToggleKeter { get; internal set; }
		public bool RandomEvents { get; internal set; }
		public bool KeterKill { get; internal set; }

		public int KeterDamage { get; internal set; }
		public int RandomMin { get; internal set; }
		public int RandomMax { get; internal set; }
		public int RandomDurMin { get; internal set; }
		public int RandomDurMax { get; internal set; }
		public int KeterKillNum { get; internal set; }
	}
}