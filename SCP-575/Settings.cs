using System.Collections.Generic;
using Smod2.API;
using Smod2.Config;

namespace SCP575
{
	public class Settings
	{
		public List<Room> BlackoutRoom = new List<Room>();
		
		public bool TimerOn { get; internal set; }
		public bool DisableTeslas { get; internal set; }
		public bool Toggled { get; internal set; }
		public bool TriggerKill { get; internal set; }
		public bool TimedOverride { get; internal set; }
		
		public int GenCount { get; internal set; }
		public bool WarheadCounting { get; internal set; }
	}
}