using scp4aiur;
using Smod2;
using Smod2.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SCP575
{
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
					keterlist.Add(players[random].Name);
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
						player.PersonalClearBroadcasts();
						player.PersonalBroadcast(15, "You were killed by SCP-575. Having a flashlight out while in an area affected by a blackout will save you from this!", false);
					}
					else
					{
						player.Damage(SCP575.KeterDamage);
						SCP575.plugin.Debug("Damaging " + player.Name + ".");
						player.PersonalBroadcast(5, "You were damaged by SCP-575!", false);
					}
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
			foreach (Room room in SCP575.plugin.Server.Map.Get079InteractionRooms(Scp079InteractionType.CAMERA).Where(p => Vector.Distance(loc, p.Position) <= 10f))
			{
				if (room.ZoneType == ZoneType.HCZ || (SCP575.timer && SCP575.timed_lcz && room.ZoneType == ZoneType.LCZ) || (SCP575.toggle && SCP575.toggle_lcz && room.ZoneType == ZoneType.LCZ))
					return true;
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
