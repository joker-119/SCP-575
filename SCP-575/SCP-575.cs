using Smod2.Attributes;
using Smod2.Config;
using Smod2;
using Smod2.API;
using Smod2.Events;
using scp4aiur;

namespace SCP575
{
    [PluginDetails(
        author = "Joker119",
        description = "Adds light blackout command + timed events",
        id = "joker.SCP575",
        version = "2.0",
        SmodMajor = 3,
        SmodMinor = 2,
        SmodRevision = 2
    )]

    public class SCP575 : Plugin
    {
        internal static SCP575 plugin;
        public static string[] validRanks;
        public static bool enabled = true;
        public static int delayTime;
        public static int waitTime;
        public static int durTime;
        public static bool Timed;
        public static bool announce;
        public static bool toggle;
        public static bool toggle_lcz;
        public static bool timed_lcz;
        public static bool timed_override = false;
        public static bool timer = false;

        public override void OnDisable()
        {
            plugin.Info(plugin.Details.name + " v." + plugin.Details.version + " has been disabled.");
        }
        public override void OnEnable()
        {
            plugin = this;
            plugin.Info(plugin.Details.name + " v." + plugin.Details.version + " has been enabled.");
        }

        public override void Register()
        {
            this.AddConfig(new ConfigSetting("575_ranks", new string[] { "owner", "admin" }, SettingType.LIST, true, "The list of ranks able to use the LO commands."));
            this.AddConfig(new ConfigSetting("575_timed", true, SettingType.BOOL, true, "If timed events should be active."));
            this.AddConfig(new ConfigSetting("575_delay", 300, SettingType.NUMERIC, true, "The amount of seconds before the first timed blackout occurs."));
            this.AddConfig(new ConfigSetting("575_duration", 90, SettingType.NUMERIC, true, "The amount of time that Lightout events last."));
            this.AddConfig(new ConfigSetting("575_wait", 180, SettingType.NUMERIC, true, "The amoutn of time to wait between subsequent timed events."));
            this.AddConfig(new ConfigSetting("575_announce", true, SettingType.BOOL, true, "If CASSIE should announce events."));
            this.AddConfig(new ConfigSetting("575_toggle_lcz", true, SettingType.BOOL, true, "If toggled events should affect LCZ."));
            this.AddConfig(new ConfigSetting("575_timed_lcz", false, SettingType.BOOL, true, "If timed events should affect LCZ."));
            this.AddEventHandlers(new Timing(Info));
            this.AddEventHandlers(new EventsHandler(this), Priority.Normal);
            this.AddCommands(new string[] { "SCP575", "575" }, new SCP575Command());
        }
    }

    public class Functions
    {
        public static void Blackout(float inaccuracy = 0)
        {
            Generator079.generators[0].CallRpcOvercharge();

            if (SCP575.timer)
            {
                if (SCP575.timed_lcz)
                {
                    LightBlackout();
                    Timing.Timer(Blackout, 9 + inaccuracy);
                }
                else
                {
                    Timing.Timer(Blackout, 11 + inaccuracy);
                }
            }
            else if (SCP575.toggle)
            {
                if (SCP575.toggle_lcz)
                {
                    LightBlackout();
                    Timing.Timer(Blackout, 9 + inaccuracy);
                }
                else
                {
                    Timing.Timer(Blackout, 11 + inaccuracy);
                }
            }

        }
        public static void LightsOn(float inaccuracy = 0)
        {
            if (SCP575.announce)
            {
                PlayerManager.localPlayer.GetComponent<MTFRespawn>().CallRpcPlayCustomAnnouncement("HEAVY CONTAINMENT POWER SYSTEM NOW OPERATIONAL", false);
            }
            EventsHandler.tesla = !EventsHandler.tesla;
        }

        public static void ToggleBlackout()
        {
            SCP575.toggle = !SCP575.toggle;
            if (SCP575.Timed && !SCP575.timed_override)
            {
                SCP575.timed_override = true;
                SCP575.Timed = !SCP575.Timed;
            }
            else if (SCP575.timed_override && !SCP575.Timed)
            {
                SCP575.timed_override = false;
                SCP575.Timed = !SCP575.Timed;
            }
            if (SCP575.toggle)
            {
                if (SCP575.announce)
                {
                    if (!SCP575.toggle_lcz)
                    {
                        PlayerManager.localPlayer.GetComponent<MTFRespawn>().CallRpcPlayCustomAnnouncement("HEAVY CONTAINMENT POWER SYSTEM FAILURE IN 3. . 2. . 1. . ", false);
                    }
                    else if (SCP575.toggle_lcz)
                    {
                        PlayerManager.localPlayer.GetComponent<MTFRespawn>().CallRpcPlayCustomAnnouncement("FACILITY POWER SYSTEM FAILURE IN 3. . 2. . 1. . ", false);
                    }
                }
            }

            Timing.Timer(Blackout, 8.7f);
        }

        public static void LightBlackout()
        {
            foreach (Room room in PluginManager.Manager.Server.Map.Get079InteractionRooms(Scp079InteractionType.CAMERA))
            {
                if (room.ZoneType == ZoneType.LCZ)
                {
                    room.FlickerLights();
                }
            }
        }

        public static void EnableBlackouts()
        {
            SCP575.Timed = true;
        }
        public static void DisableBlackouts()
        {
            SCP575.Timed = false;
        }
        public static void EnableAnnounce()
        {
            SCP575.announce = true;
        }
        public static void DisableAnnounce()
        {
            SCP575.announce = false;
        }
    }
}