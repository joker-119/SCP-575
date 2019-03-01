using System.Reflection;
using System;
using Smod2.Attributes;
using Smod2.Config;
using Smod2;
using Smod2.API;
using Smod2.Events;
using System.Collections.Generic;
using UnityEngine;
using scp4aiur;

namespace SCP575
{
	[PluginDetails(
		author = "Joker119",
		name = "SCP-575",
		description = "Adds light blackout command + timed events",
		id = "joker.SCP575",
		version = "2.3.5",
		SmodMajor = 3,
		SmodMinor = 3,
		SmodRevision = 0
	)]

	public class SCP575 : Plugin
	{
		internal static SCP575 plugin;
		public static string[] validRanks;
		public static bool enabled = true;
		public static float delayTime;
		public static float waitTime;
		public static float durTime;
		public static bool 
			Timed,
			announce,
			toggle,
			toggle_lcz,
			timed_lcz,
			timer = false,
			timed_override = false,
			toggleTesla,
			timedTesla,
			keter,
			triggerkill,
			toggleketer,
			keterkill;
		public static int 
			KeterDamage,
			keterkill_num;
		public static List<Room> BlackoutRoom = new List<Room>();

		public override void OnDisable()
		{
			plugin.Debug(plugin.Details.name + " v." + plugin.Details.version + " has been disabled.");
		}
		public override void OnEnable()
		{
			plugin = this;
			plugin.Debug(plugin.Details.name + " v." + plugin.Details.version + " has been enabled.");
		}

		public override void Register()
		{
			this.AddConfig(new ConfigSetting("575_ranks", new string[] { "owner", "admin" }, SettingType.LIST, true, "The list of ranks able to use the LO commands."));
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
			this.AddEventHandlers(new EventsHandler(this), Priority.Normal);
			Timing.Init(this);
			this.AddCommands(new string[] { "SCP575", "575" }, new SCP575Command());
		}
	}
}