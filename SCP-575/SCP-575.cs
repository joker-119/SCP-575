using Smod2.Attributes;
using Smod2;
using System.Collections.Generic;
using MEC;

namespace SCP575
{
	[PluginDetails(
		author = "Joker119",
		name = "SCP-575",
		description = "Adds light blackout command + timed events",
		id = "joker.SCP575",
		version = "2.5.0",
		configPrefix = "575",
		SmodMajor = 3,
		SmodMinor = 4,
		SmodRevision = 0
	)]

	public class Scp575 : Plugin
	{
		public Settings Vars { get; private set; }
		public Methods Functions { get; private set; }
		public List<CoroutineHandle> Coroutines = new List<CoroutineHandle>();
		public readonly System.Random Gen = new System.Random();

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