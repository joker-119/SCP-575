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

    public class Functions
    {
        public static void RunBlackout()
        {
            SCP575.plugin.Debug("Blackout Function has started");
            if ((SCP575.timer && SCP575.timed_lcz) || (SCP575.toggle && SCP575.toggle_lcz))
            {
                foreach (Room room in SCP575.BlackoutRoom)
                {
                    room.FlickerLights();
                }
            }
            Generator079.generators[0].CallRpcOvercharge();

            if ((SCP575.timer && SCP575.keter) || (SCP575.toggle && SCP575.toggleketer))
            {
                Keter();
            }
        }
        public static IEnumerable<float> ToggledBlackout(float delay)
        {
            yield return delay;
            while (SCP575.toggle)
            {
                RunBlackout();
                yield return 11;
            }
        }
        public static IEnumerable<float> TimedBlackout(float delay)
        {
            SCP575.plugin.Debug("Being Delayed");
            yield return delay;
            while (SCP575.Timed)
            {
                SCP575.plugin.Debug("Announcing");
                if (SCP575.announce && SCP575.timed_lcz && SCP575.Timed)
                {
                    PlayerManager.localPlayer.GetComponent<MTFRespawn>().CallRpcPlayCustomAnnouncement("FACILITY POWER SYSTEM FAILURE IN 3 . 2 . 1 .", false);
                }
                else
                {
                    PlayerManager.localPlayer.GetComponent<MTFRespawn>().CallRpcPlayCustomAnnouncement("HEAVY CONTAINMENT POWER SYSTEM FAILURE IN 3 . 2 . 1 .", false);
                }
                yield return 8.7f;
                float blackout_dur = SCP575.durTime;
                SCP575.plugin.Debug("Flipping Bools1");
                SCP575.timer = true;
                SCP575.triggerkill = true;
                SCP575.plugin.Debug(SCP575.timer.ToString() + SCP575.triggerkill.ToString());
                do 
                {
                    SCP575.plugin.Debug("Running Blackout");
                    RunBlackout();
                    yield return 11;
                } while ((blackout_dur -= 11) > 0);
                SCP575.plugin.Debug("Announcing Disabled.");
                if (SCP575.announce && SCP575.timed_lcz && SCP575.Timed)
                {
                    PlayerManager.localPlayer.GetComponent<MTFRespawn>().CallRpcPlayCustomAnnouncement("FACILITY POWER SYSTEM NOW OPERATIONAL", false);
                }
                else
                {
                    PlayerManager.localPlayer.GetComponent<MTFRespawn>().CallRpcPlayCustomAnnouncement("HEAVY CONTAINMENT POWER SYSTEM NOW OPERATIONAL", false);
                }
                yield return 8.7f;
                SCP575.plugin.Debug("Flipping bools2");
                SCP575.timer = false;
                SCP575.triggerkill = false;
                SCP575.plugin.Debug("Timer: " + SCP575.timer);

                SCP575.plugin.Debug("Waiting to re-execute..");
                yield return SCP575.waitTime;
            }
        }
        public static void Keter()
        {
            SCP575.plugin.Debug("Keter function started.");
            List<Player> players = SCP575.plugin.Server.GetPlayers();
            List<String> keterlist = new List<String>();
            for (int i = 0; i < SCP575.keterkill_num; i++)
            {
                int random = new System.Random().Next(players.Count);
                string name = players[random].Name;
                if (players[random].TeamRole.Team != Smod2.API.Team.SCP && players[random].TeamRole.Team != Smod2.API.Team.SPECTATOR)
                {
                    keterlist.Add(name);
                }
            }

            foreach (Player player in players)
            {
                if (Functions.IsInDangerZone(player) && !Functions.HasFlashlight(player) && player.TeamRole.Team != Smod2.API.Team.SPECTATOR && player.TeamRole.Team != Smod2.API.Team.SCP)
                {
                    if (keterlist.Contains(player.Name) && SCP575.keterkill)
                    {
                        player.Kill();
                        SCP575.plugin.Debug("Killing " + player.Name + ".");
                        keterlist.Remove(player.Name);
                    }
                    else
                    {
                        player.Damage(SCP575.KeterDamage);
                        SCP575.plugin.Debug("Damaging " + player.Name + ".");
                    }
                }
                else
                {
                    break;
                }
            }
        }
        public static bool HasFlashlight(Player player)
        {
            GameObject ply = player.GetGameObject() as GameObject;
            WeaponManager manager = ply.GetComponent<WeaponManager>();
            if (manager.NetworksyncFlash || player.GetCurrentItem().ItemType == ItemType.FLASHLIGHT)
            {
                return true;
            }
            return false;
        }
        public static bool IsInDangerZone(Player player)
        {
            Vector loc = player.GetPosition();
            foreach (Room room in SCP575.plugin.Server.Map.Get079InteractionRooms(Scp079InteractionType.CAMERA))
            {
                if (room.ZoneType == ZoneType.HCZ || (SCP575.timer && SCP575.timed_lcz && room.ZoneType == ZoneType.LCZ) || (SCP575.toggle && SCP575.toggle_lcz && room.ZoneType == ZoneType.LCZ))
                {
                    float x = Math.Abs(loc.x - room.Position.x),
                        y = Math.Abs(loc.y - room.Position.y),
                        z = Math.Abs(loc.z - room.Position.z);
                    if (x < 10 && y < 10 && z < 10)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        public static void Get079Rooms()
        {
            foreach (Room room in PluginManager.Manager.Server.Map.Get079InteractionRooms(Scp079InteractionType.CAMERA))
            {
                if (room.ZoneType == ZoneType.LCZ)
                {
                    SCP575.BlackoutRoom.Add(room);
                }
            }
        }
        public static void ToggleBlackout()
        {
            SCP575.toggle = !SCP575.toggle;
            if (SCP575.Timed)
            {
                SCP575.timed_override = true;
                SCP575.Timed = false;
            }
            else if (SCP575.timed_override)
            {
                SCP575.timed_override = false;
                SCP575.Timed = true;
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
                Timing.Run(ToggledBlackout(8.7f));
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