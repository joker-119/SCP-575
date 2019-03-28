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
        version = "2.3.8",
        SmodMajor = 3,
        SmodMinor = 3,
        SmodRevision = 0
    )]

    public class SCP575 : Plugin
    {
        public string[] validRanks;

        public readonly System.Random gen = new System.Random();

        public List<CoroutineHandle> coroutines = new List<CoroutineHandle>();
        public List<Room> BlackoutRoom = new List<Room>();
        public List<Room> rooms = new List<Room>();

        public float waitTime { get; private set; }
        public float durTime { get; private set; }
        public float delayTime { get; private set; }


        public bool Timed { get; set; }
        public bool announce { get; set; }
        public bool toggle { get; set; }
        public bool toggle_lcz { get; private set; }
        public bool timed_lcz { get; private set; }
        public bool timer { get; set; } = false;
        public bool timed_override { get; set; } = false;
        public bool toggleTesla { get; private set; }
        public bool timedTesla { get; private set; }
        public bool keter { get; private set; }
        public bool triggerkill { get; set; }
        public bool toggleketer { get; private set; }
        public bool random_events { get; private set; }
        public bool keterkill { get; private set; }

        public int KeterDamage { get; private set; }
        public int random_min { get; private set; }
        public int random_max { get; private set; }
        public int random_dur_min { get; private set; }
        public int random_dur_max { get; private set; }
        public int keterkill_num { get; private set; }

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
            this.AddConfig(new ConfigSetting("575_random_events", false, SettingType.BOOL, true, "If 575 events should be randomized."));
            this.AddConfig(new ConfigSetting("575_random_min", 60, SettingType.NUMERIC, true, "The minimum amount of time between random events."));
            this.AddConfig(new ConfigSetting("575_random_max", 300, SettingType.NUMERIC, true, "The maximum wait time between random events."));
            this.AddConfig(new ConfigSetting("575_random_dur_min", 11, SettingType.NUMERIC, true, "The minimum amount of time a random event should last."));
            this.AddConfig(new ConfigSetting("575_random_dur_max", 90, SettingType.NUMERIC, true, "The maximum amount of time a random event should last."));

            this.AddEventHandlers(new EventsHandler(this), Priority.Normal);

            new Functions(this);

            this.AddCommands(new string[] { "SCP575", "575" }, new SCP575Command(this));
        }

        public void RefreshConfig()
        {
            validRanks = GetConfigList("575_ranks");
            delayTime = GetConfigFloat("575_delay");
            waitTime = GetConfigFloat("575_wait");
            durTime = GetConfigFloat("575_duration");
            Timed = GetConfigBool("575_timed");
            announce = GetConfigBool("575_announce");
            toggle_lcz = GetConfigBool("575_toggle_lcz");
            timed_lcz = GetConfigBool("575_timed_lcz");
            timedTesla = GetConfigBool("575_timed_tesla");
            toggleTesla = GetConfigBool("575_toggle_tesla");
            keter = GetConfigBool("575_keter");
            KeterDamage = GetConfigInt("575_keter_damage");
            toggleketer = GetConfigBool("575_keter_toggle");
            keterkill = GetConfigBool("575_keter_kill");
            keterkill_num = GetConfigInt("575_keter_kill_num");
            random_events = GetConfigBool("575_random_events");
            random_min = GetConfigInt("575_random_min");
            random_max = GetConfigInt("575_random_max") + 1;
            random_dur_min = GetConfigInt("575_random_dur_min");
            random_dur_max = GetConfigInt("575_random_dur_max") + 1;
            timer = false;
            triggerkill = false;
        }
    }
}