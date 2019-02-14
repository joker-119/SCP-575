using Smod2.Attributes;
using Smod2.Config;
using Smod2;
using Smod2.API;
using Smod2.Commands;
using scp4aiur;
using System.Collections.Generic;

namespace Blackout
{
    [PluginDetails(
        author = "Joker+4aiur",
        description = "Adds light blackout command.",
        id = "blackout",
        version = "1.5.0",
        SmodMajor = 3,
        SmodMinor = 2,
        SmodRevision = 2)]
    public class Plugin : Smod2.Plugin
    {
        public static Plugin instance;

        public static string[] validRanks;
        public static bool enabled = true;
        public static int delayTime;
        public static int waitTime;
        public static int durTime;
        public static bool giveFlashlights;
        public static bool Timed;
        public static bool announce;
        public static bool toggle;
        public static bool toggle_lcz;
        public static bool timed_lcz;

        public override void Register()
        {
            instance = this;

            AddConfig(new ConfigSetting("blackout_ranks", new string[] { "owner", "admin", "friend", "srmod" }, SettingType.LIST, true, "Valid ranks for the BLACKOUT command."));
            AddConfig(new ConfigSetting("blackout_flashlights", false, SettingType.BOOL, true, "If everyone should get a flashlight on spawn."));
            AddConfig(new ConfigSetting("blackout_timer", true, SettingType.BOOL, true, "Wether or not to use the automated timer or not."));
            AddConfig(new ConfigSetting("blackout_delay", 300000, SettingType.NUMERIC, true, "The amount of time before the first Blackout occurs" ));
            AddConfig(new ConfigSetting("blackout_duration", 90000, SettingType.NUMERIC, true, "The amount of time the blackout will last."));
            AddConfig(new ConfigSetting("blackout_wait", 180000, SettingType.NUMERIC, true, "The amount of time the lights will remain on before the next blackout."));
            AddConfig(new ConfigSetting("blackout_light_announcments", true, SettingType.BOOL, true, "If CASSIE should announce light events."));
            AddConfig(new ConfigSetting("blackout_toggle_lcz", true, SettingType.BOOL, true, "If lights in LCZ should be blacked out during toggled blackouts."));
            AddConfig(new ConfigSetting("blackout_timed_lcz", false, SettingType.BOOL, true, "If LCZ should be blacked out during timed events."));

            AddEventHandlers(new Timing(Info));
            AddEventHandlers(new EventHandlers());

            AddCommand("blackout", new CommandHandler());

        }

        public override void OnEnable()
        {
        }

        public static void ToggleBlackout()
        {
            toggle = !toggle;

            if (toggle)
            {
                if(Plugin.giveFlashlights)
                {
                    foreach (Player p in PluginManager.Manager.Server.GetPlayers())
                    {
                        if (!p.HasItem(ItemType.FLASHLIGHT))
                        {
                            p.GiveItem(ItemType.FLASHLIGHT);
                        }
                    }
                }
                if (toggle_lcz)
                {
                    PlayerManager.localPlayer.GetComponent<MTFRespawn>().CallRpcPlayCustomAnnouncement("FACILITY POWER SYSTEM FAILURE IN 3. . 2. . 1. . ", false);
                }
                else
                {
                    PlayerManager.localPlayer.GetComponent<MTFRespawn>().CallRpcPlayCustomAnnouncement("HEAVY CONTAINMENT POWER SYSTEM FAILURE IN 3. . 2. . 1. . ", false);
                }

                Timing.Timer(TenSecondBlackout, 8.7f);
                if (Plugin.Timed)
                {
                    Plugin.Timed = !Plugin.Timed;
                }
            }
        }

         private static void TenSecondBlackout(float inaccuracy = 0)
        {
            if (toggle_lcz)
            {
                foreach (Room room in PluginManager.Manager.Server.Map.Get079InteractionRooms(Scp079InteractionType.CAMERA))
                {
                    if (room.ZoneType != ZoneType.ENTRANCE && room.ZoneType != ZoneType.HCZ)
                    {
                        room.FlickerLights();
                    }
                }
            }
            Generator079.generators[0].CallRpcOvercharge();

            if (toggle)
            {
                Timing.Timer(TenSecondBlackout, 8 + inaccuracy);
            }
        }           

        
        public static void DisableBlackouts() {
            enabled = false;
        }

        public static void EnableBlackouts() {
            enabled = true;
        }
        public static void DisableAnnouncements()
        {
            announce = false;
        }
        public static void EnableAnnouncements()
        {
            announce = true;
        }

        public override void OnDisable()
        {
        }
    }
}
