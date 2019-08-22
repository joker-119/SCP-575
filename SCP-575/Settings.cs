using System.Collections.Generic;
using Smod2.API;
using Smod2.Config;

namespace SCP575
{
	public class Settings
	{
		[ConfigOption] public string[] ValidRanks = new string[]{};

		[ConfigOption] public float WaitTime = 180f;
		[ConfigOption] public float DurTime = 90f;
		[ConfigOption] public float DelayTime = 300f;

		[ConfigOption] public bool TimedEvents = true;
		[ConfigOption] public bool Announce = true;
		[ConfigOption] public bool ToggledLcz = true;
		[ConfigOption] public bool TimedLcz = true;
		[ConfigOption] public bool ToggleTeslaDisable = true;
		[ConfigOption] public bool TimedTeslaDisable = true;
		[ConfigOption] public bool Keter = true;
		[ConfigOption] public bool ToggleKeter = false;
		[ConfigOption] public bool RandomEvents = true;
		[ConfigOption] public bool KeterKill = false;

		[ConfigOption] public int KeterDamage = 10;
		[ConfigOption] public int RandomMin = 60;
		[ConfigOption] public int RandomMax = 300;
		[ConfigOption] public int RandomDurMin = 11;
		[ConfigOption] public int RandomDurMax = 90;
		[ConfigOption] public int KeterKillNum = 1;

		public List<Room> BlackoutRoom = new List<Room>();
		
		public bool TimerOn { get; internal set; }
		public bool Toggled { get; internal set; }
		public bool TriggerKill { get; internal set; }
		public bool TimedOverride { get; internal set; }
		
		public int GenCount { get; internal set; }
		public bool WarheadCounting { get; internal set; }
	}
}