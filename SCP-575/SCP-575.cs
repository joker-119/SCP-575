using Smod2.Attributes;
using Smod2;
using System.Collections.Generic;
using MEC;
using Smod2.Config;

namespace SCP575
{
	[PluginDetails(
		author = "Joker119",
		name = "SCP-575",
		description = "Adds light blackout command + timed events",
		id = "joker.SCP575",
		version = "2.5.5",
		configPrefix = "575",
		SmodMajor = 3,
		SmodMinor = 5,
		SmodRevision = 0
	)]

	public class Scp575 : Plugin
	{
		public Settings Vars { get; private set; }
		public Methods Functions { get; private set; }
		public List<CoroutineHandle> Coroutines = new List<CoroutineHandle>();
		public readonly System.Random Gen = new System.Random();

		[ConfigOption] public string[] ValidRanks = new string[] { };

		[ConfigOption] public float WaitTime = 180f;
		[ConfigOption] public float DurTime = 90f;
		[ConfigOption] public float DelayTime = 300f;
		[ConfigOption] public float VoicePitch = 0.35f;

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
		[ConfigOption] public bool DoorEvents = true;
		[ConfigOption] public bool Voice = true;

		[ConfigOption] public int KeterDamage = 10;
		[ConfigOption] public int RandomMin = 60;
		[ConfigOption] public int RandomMax = 300;
		[ConfigOption] public int RandomDurMin = 11;
		[ConfigOption] public int RandomDurMax = 90;
		[ConfigOption] public int KeterKillNum = 1;

		public override void OnDisable()
		{
			Debug(Details.name + " v." + Details.version + " has been disabled.");
		}
		public override void OnEnable()
		{
			Debug(Details.name + " v." + Details.version + " has been enabled.");
		}

		public override void Register()
		{
			AddEventHandlers(new EventsHandler(this));

			Vars = new Settings();

			Functions = new Methods(this);

			AddCommands(new string[] { "SCP575", "575" }, new Scp575Command(this));
		}
	}
}