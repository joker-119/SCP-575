using Smod2.Attributes;
using Smod2.Config;
using Smod2;
using Smod2.API;
using Smod2.Events;
using System.Collections.Generic;
using MEC;

namespace SCP575
{
	[PluginDetails(
		author = "Joker119",
		name = "SCP-575",
		description = "Adds light blackout command + timed events",
		id = "joker.SCP575",
		version = "2.4.0",
		SmodMajor = 3,
		SmodMinor = 3,
		SmodRevision = 0
	)]

	public class SCP575 : Plugin
	{
		public Settings Vars { get; private set; }
		public Methods Functions { get; private set; }
		public List<CoroutineHandle> coroutines = new List<CoroutineHandle>();
		public readonly System.Random Gen = new System.Random();

		public override void OnDisable()
		{
			this.Debug(this.Details.name + " v." + this.Details.version + " has been disabled.");
		}
		public override void OnEnable()
		{
			this.Debug(this.Details.name + " v." + this.Details.version + " has been enabled.");
		}

		public override void Register()
		{
			this.AddConfig(new ConfigSetting("575_ranks", new string[] { }, SettingType.LIST, true, "The list of ranks able to use the LO commands."));
			this.AddConfig(new ConfigSetting("575_timed", true, SettingType.BOOL, true, "If timed events should be active."));
			this.AddConfig(new ConfigSetting("575_delay", 300f, SettingType.FLOAT, true, "The amount of seconds before the first timed blackout occurs."));
			this.AddConfig(new ConfigSetting("575_duration", 90f, SettingType.FLOAT, true, "The amount of time that Lightout events last."));
			this.AddConfig(new ConfigSetting("575_wait", 180f, SettingType.FLOAT, true, "The amoutn of time to wait between subsequent timed events."));
			this.AddConfig(new ConfigSetting("575_announce", true, SettingType.BOOL, true, "If CASSIE should announce events."));
			this.AddConfig(new ConfigSetting("575_toggle_lcz", true, SettingType.BOOL, true, "If toggled events should affect LCZ."));
			this.AddConfig(new ConfigSetting("575_timed_lcz", true, SettingType.BOOL, true, "If timed events should affect LCZ."));
			this.AddConfig(new ConfigSetting("575_toggle_tesla", true, SettingType.BOOL, true, "If teslas should be disabled during toggled events."));
			this.AddConfig(new ConfigSetting("575_timed_tesla", true, SettingType.BOOL, true, "If teslas should be disabled during timed events."));
			this.AddConfig(new ConfigSetting("575_keter_damage", 10, SettingType.NUMERIC, true, "How much damage per 5 seconds people in affected areas take."));
			this.AddConfig(new ConfigSetting("575_keter", true, SettingType.BOOL, true, "If SCP-575 should make use of it's Keter status."));
			this.AddConfig(new ConfigSetting("575_keter_toggle", false, SettingType.BOOL, true, "If SCP-575 keter should be enabled for toggled events."));
			this.AddConfig(new ConfigSetting("575_keter_kill", false, SettingType.BOOL, true, "If SCP-575's keter event should kill players instead of damage them."));
			this.AddConfig(new ConfigSetting("575_keter_kill_num", 1, SettingType.NUMERIC, true, "The number of players killed during timed Keter events.));"));
			this.AddConfig(new ConfigSetting("575_random_events", false, SettingType.BOOL, true, "If 575 events should be randomized."));
			this.AddConfig(new ConfigSetting("575_random_min", 60, SettingType.NUMERIC, true, "The minimum amount of time between random events."));
			this.AddConfig(new ConfigSetting("575_random_max", 300, SettingType.NUMERIC, true, "The maximum wait time between random events."));
			this.AddConfig(new ConfigSetting("575_random_dur_min", 11, SettingType.NUMERIC, true, "The minimum amount of time a random event should last."));
			this.AddConfig(new ConfigSetting("575_random_dur_max", 90, SettingType.NUMERIC, true, "The maximum amount of time a random event should last."));

			this.AddEventHandlers(new EventsHandler(this), Priority.Normal);

			Vars = new Settings(this);

			Functions = new Methods(this);

			this.AddCommands(new string[] { "SCP575", "575" }, new SCP575Command(this));
		}

		public void RefreshConfig()
		{
			Vars.ValidRanks = GetConfigList("575_ranks");
			Vars.DelayTime = GetConfigFloat("575_delay");
			Vars.WaitTime = GetConfigFloat("575_wait");
			Vars.DurTime = GetConfigFloat("575_duration");
			Vars.TimedEvents = GetConfigBool("575_timed");
			Vars.Announce = GetConfigBool("575_announce");
			Vars.ToggledLcz = GetConfigBool("575_toggle_lcz");
			Vars.TimedLcz = GetConfigBool("575_timed_lcz");
			Vars.TimedTeslaDisable = GetConfigBool("575_timed_tesla");
			Vars.ToggleTeslaDisable = GetConfigBool("575_toggle_tesla");
			Vars.Keter = GetConfigBool("575_keter");
			Vars.KeterDamage = GetConfigInt("575_keter_damage");
			Vars.ToggleKeter = GetConfigBool("575_keter_toggle");
			Vars.KeterKill = GetConfigBool("575_keter_kill");
			Vars.KeterKillNum = GetConfigInt("575_keter_kill_num");
			Vars.RandomEvents = GetConfigBool("575_random_events");
			Vars.RandomMin = GetConfigInt("575_random_min");
			Vars.RandomMax = GetConfigInt("575_random_max") + 1;
			Vars.RandomDurMin = GetConfigInt("575_random_dur_min");
			Vars.RandomDurMax = GetConfigInt("575_random_dur_max") + 1;
			Vars.TimerOn = false;
			Vars.TriggerKill = false;
		}
	}
}