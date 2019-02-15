using Smod2.Attributes;
using Smod2.Config;
using Smod2;
using Smod2.Events;
using scp4aiur;

namespace Lightsout
{
    [PluginDetails(
        author = "Joker119",
        description = "Adds light blackout command + timed events",
        id = "joker.lightsout",
        version = "2.0",
        SmodMajor = 3,
        SmodMinor = 2,
        SmodRevision = 2
    )]

    public class Lightsout : Plugin
    {
        internal static Lightsout plugin;
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
        public static bool timed = false;

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
            this.AddConfig(new ConfigSetting("lo_ranks", new string[] { "owner", "admin" }, SettingType.LIST, true, "The list of ranks able to use the LO commands."));
            this.AddConfig(new ConfigSetting("lo_timed", true, SettingType.BOOL, true, "If timed events should be active."));
            this.AddConfig(new ConfigSetting("lo_delay", 300, SettingType.NUMERIC, true, "The amount of seconds before the first timed blackout occurs."));
            this.AddConfig(new ConfigSetting("lo_duration", 90, SettingType.NUMERIC, true, "The amount of time that Lightout events last."));
            this.AddConfig(new ConfigSetting("lo_wait", 180, SettingType.NUMERIC, true, "The amoutn of time to wait between subsequent timed events."));
            this.AddConfig(new ConfigSetting("lo_announce", true, SettingType.BOOL, true, "If CASSIE should announce events."));
            this.AddConfig(new ConfigSetting("lo_toggle_lcz", true, SettingType.BOOL, true, "If toggled events should affect LCZ."));
            this.AddConfig(new ConfigSetting("lo_timed_lcz", false, SettingType.BOOL, true, "If timed events should affect LCZ."));
            this.AddEventHandlers(new Timing(Info));
            this.AddEventHandlers(new EventsHandler(this), Priority.Normal);
            this.AddCommands(new string[] { "lightsout", "lo" }, new LightsoutCommand());
        }
    }

    public class Functions
    {
        public static void Blackout(float inaccuracy = 0)
        {
            Generator079.generators[0].CallRpcOvercharge();

            if (Lightsout.timed || Lightsout.toggle)
            {
                Timing.Timer(Blackout, 11 + inaccuracy);
            }
        }
        public static void LightsOn(float inaccuracy = 0)
        {
            if (Lightsout.announce)
            {
                PlayerManager.localPlayer.GetComponent<MTFRespawn>().CallRpcPlayCustomAnnouncement("HEAVY CONTAINMENT POWER SYSTEM NOW OPERATIONAL", false);
            }
            EventsHandler.tesla = !EventsHandler.tesla;
        }

        public static void ToggleBlackout()
        {
            Lightsout.toggle = !Lightsout.toggle;
            if (Lightsout.timed && !Lightsout.timed_override)
            {
                Lightsout.timed_override = true;
                Lightsout.timed = !Lightsout.timed;
            }
            else if (Lightsout.timed_override && !Lightsout.timed)
            {
                Lightsout.timed_override = false;
                Lightsout.timed = !Lightsout.timed;
            }
            if (Lightsout.toggle)
            {
                if (Lightsout.announce)
                {
                    PlayerManager.localPlayer.GetComponent<MTFRespawn>().CallRpcPlayCustomAnnouncement("HEAVY CONTAINMENT POWER SYSTEM FAILURE IN 3. . 2. . 1. . ", false);
                }
            }

            Timing.Timer(Blackout, 8.7f);
        }

        // public static void LightBlackout(float inaccuracy = 0)
        // {
        //     foreach (Room room in PluginManager.Manager.Server.Map.Get079InteractionRooms(Scp079InteractionType.CAMERA))
        //     {
        //         if (room.ZoneType == ZoneType.LCZ)
        //         {
        //             room.FlickerLights();
        //         }
        //     }
        // }

        public static void EnableBlackouts()
        {
            Lightsout.enabled = true;
        }
        public static void DisableBlackouts()
        {
            Lightsout.enabled = false;
        }
        public static void EnableAnnounce()
        {
            Lightsout.announce = true;
        }
        public static void DisableAnnounce()
        {
            Lightsout.announce = false;
        }
    }
}