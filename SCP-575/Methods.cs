using System.Collections.Generic;
using System.Linq;
using MEC;
using Smod2;
using Smod2.API;
using Smod2.Commands;
using UnityEngine;
using UnityEngine.Networking;

namespace SCP575
{
	public class Methods
	{
		private readonly Scp575 plugin;

		public Methods(Scp575 plugin) => this.plugin = plugin;

		private void RunBlackout(float duration)
		{
			plugin.Info("Blackout Function has started");

			Timing.RunCoroutine(LczBlackout(duration));
			Timing.RunCoroutine(HczBlackout(duration));

			List<Smod2.API.Door> doors = plugin.Server.Map.GetDoors();
			List<Smod2.API.Door> toClose = new List<Smod2.API.Door>();
			int count = plugin.Gen.Next(doors.Count);
			for (int i = 0; i < count; i++)
				toClose.Add(doors[plugin.Gen.Next(doors.Count)]);
			if (!plugin.DoorEvents)
				return;
			foreach (Smod2.API.Door door in toClose)
				Timing.RunCoroutine(LockDoor(door));
		}

		private IEnumerator<float> LczBlackout(float duration)
		{
			do
			{
				if (!plugin.Vars.TimerOn)
					yield break;
				foreach (Room room in plugin.Server.Map.Get079InteractionRooms(Scp079InteractionType.CAMERA).Where(rm => rm.ZoneType == ZoneType.LCZ))
					room.FlickerLights();
				plugin.Info("LCZ Loop");
				if (plugin.Keter && !plugin.Vars.Toggled || plugin.Vars.Toggled && plugin.ToggleKeter)
					plugin.Coroutines.Add(Timing.RunCoroutine(Keter()));
				yield return Timing.WaitForSeconds(8f);
			} while ((duration -= 8) > 0);
		}

		private IEnumerator<float> HczBlackout(float duration)
		{
			plugin.Vars.DisableTeslas = true;
			foreach (Room room in plugin.Vars.BlackoutRoom.Where(rm => rm.ZoneType == ZoneType.HCZ))
				do
				{
					if (!plugin.Vars.TimerOn)
						yield break;
					Generator079.generators[0].CallRpcOvercharge();
					plugin.Info("HCZ Loop");
					room.FlickerLights();
					if (plugin.Keter && !plugin.Vars.Toggled || plugin.Vars.Toggled && plugin.ToggleKeter)
						plugin.Coroutines.Add(Timing.RunCoroutine(Keter()));
					yield return Timing.WaitForSeconds(11f);
				} while ((duration -= 11) > 0);

			plugin.Vars.DisableTeslas = false;
		}

		private IEnumerator<float> ToggledBlackout(float delay)
		{
			yield return Timing.WaitForSeconds(delay);

			while (plugin.Vars.Toggled)
			{
				RunBlackout(11f);
				yield return Timing.WaitForSeconds(11f);
			}
		}

		private IEnumerator<float> LockDoor(Smod2.API.Door door)
		{
			door.Open = false;

			yield return Timing.WaitForSeconds(RandomTime());

			door.Open = true;

			yield return Timing.WaitForSeconds(RandomTime());

			door.Open = false;
			door.Locked = true;

			yield return Timing.WaitForSeconds(RandomTime());

			door.Locked = false;
			if (plugin.Vars.WarheadCounting)
				door.Open = true;
		}

		private float RandomTime() => plugin.Gen.Next(1, 5);

		public void Scp575Voice(float pitch)
		{
			plugin.Info("Voice.");
			PlayerManager.localPlayer.GetComponent<MTFRespawn>().CallRpcPlayCustomAnnouncement($"pitch_{pitch} No Containment", true);
		}

		public IEnumerator<float> TimedBlackout(float delay)
		{
			plugin.Info("Being Delayed");

			yield return Timing.WaitForSeconds(delay);

			while (plugin.TimedEvents)
			{
				plugin.Info("Announcing");
				if (plugin.Announce && plugin.TimedLcz)
					PlayerManager.localPlayer.GetComponent<MTFRespawn>().CallRpcPlayCustomAnnouncement("FACILITY POWER SYSTEM FAILURE IN 3 . 2 . 1 .", false);
				else if (plugin.Announce)
					PlayerManager.localPlayer.GetComponent<MTFRespawn>().CallRpcPlayCustomAnnouncement("HEAVY CONTAINMENT POWER SYSTEM FAILURE IN 3 . 2 . 1 .", false);
				yield return Timing.WaitForSeconds(8.7f);

				float blackoutDur;
				if (plugin.RandomEvents)
					blackoutDur = plugin.Gen.Next(plugin.RandomDurMin, plugin.RandomDurMax);
				else
					blackoutDur = plugin.DurTime;

				plugin.Info("Flipping Bool 1");
				plugin.Vars.TimerOn = true;
				plugin.Vars.TriggerKill = true;

				RunBlackout(blackoutDur);
				if (plugin.Voice)
					Scp575Voice(plugin.VoicePitch);
				yield return Timing.WaitForSeconds(blackoutDur - 8.7f);

				plugin.Info("Announcing Disabled.");
				if (plugin.Announce && plugin.TimedLcz)
					PlayerManager.localPlayer.GetComponent<MTFRespawn>().CallRpcPlayCustomAnnouncement("FACILITY POWER SYSTEM NOW OPERATIONAL", false);
				else if (plugin.Announce)
					PlayerManager.localPlayer.GetComponent<MTFRespawn>().CallRpcPlayCustomAnnouncement("HEAVY CONTAINMENT POWER SYSTEM NOW OPERATIONAL", false);
				yield return Timing.WaitForSeconds(8.7f);

				plugin.Info("Flipping Bool 2");
				plugin.Vars.TriggerKill = false;
				plugin.Vars.TimerOn = false;
				plugin.Info("Timer: " + plugin.Vars.TimerOn);
				plugin.Info("Waiting to re-execute..");

				if (plugin.RandomEvents)
					yield return Timing.WaitForSeconds(plugin.Gen.Next(plugin.RandomMin, plugin.RandomMax));
				else
					yield return Timing.WaitForSeconds(plugin.WaitTime);
			}
		}

		private IEnumerator<float> Keter()
		{
			if (!plugin.Keter)
				yield break;

			List<Player> players = plugin.Server.GetPlayers();
			List<Player> keterList = new List<Player>();

			foreach (Player player in players)
			{
				if (player.TeamRole.Team == Smod2.API.Team.SPECTATOR || player.TeamRole.Team == Smod2.API.Team.SCP) 
					continue;
				yield return Timing.WaitForSeconds(0.1f);
				if (HasFlashlight(player)) 
					continue;
				yield return Timing.WaitForSeconds(0.1f);
				if (!IsInDangerZone(player)) 
					continue;

				keterList.Add(player);
			}

			if (plugin.KeterKill && keterList.Count > 0 && plugin.Vars.TriggerKill)
			{
				for (int i = 0; i < plugin.KeterKillNum; i++)
				{
					if (keterList.Count == 0) break;

					Player ply = players[plugin.Gen.Next(keterList.Count)];
					if (ply.GetGodmode()) continue;

					ply.Kill();
					keterList.Remove(ply);
					ply.PersonalClearBroadcasts();
					ply.PersonalBroadcast(10, "You were killed by SCP-575!", false);
				}

				plugin.Vars.TriggerKill = false;
			}
			else if (!plugin.KeterKill && keterList.Count > 0)
				foreach (Player player in keterList)
				{
					if (player.GetGodmode()) continue;

					player.Damage(plugin.KeterDamage);
					player.PersonalClearBroadcasts();
					player.PersonalBroadcast(5, "You were damaged by SCP-575!", false);
					ShakeScreen((GameObject)player.GetGameObject());
				}
		}

		private static void ShakeScreen(GameObject ply)
		{
			int rpcId = -737840022;
			NetworkWriter writer = new NetworkWriter();
			writer.Write((short)0);
			writer.Write((short)2);
			writer.WritePackedUInt32((uint)rpcId);
			writer.Write(ply.GetComponent<NetworkIdentity>().netId);
			writer.FinishMessage();
			ply.GetComponent<CharacterClassManager>().connectionToClient.SendWriter(writer, 0);
		}

		private static bool HasFlashlight(Player player) =>
			((GameObject)player.GetGameObject()).GetComponent<Scp173PlayerScript>().SMHasLightSource();

		private bool IsInDangerZone(Player player)
		{
			Vector loc = player.GetPosition();

			return plugin.Vars.BlackoutRoom.Where(p => Vector.Distance(loc, p.Position) <= 12f).Any(room =>
				(room.ZoneType == ZoneType.HCZ ||
				plugin.Vars.TimerOn && plugin.TimedLcz && room.ZoneType == ZoneType.LCZ ||
				plugin.Vars.Toggled && plugin.ToggledLcz && room.ZoneType == ZoneType.LCZ) &&
				room.RoomType != RoomType.CHECKPOINT_A && room.RoomType != RoomType.CHECKPOINT_B &&
				room.RoomType != RoomType.ENTRANCE_CHECKPOINT);
		}

		public void Get079Rooms()
		{
			Room[] rooms = plugin.Server.Map.Get079InteractionRooms(Scp079InteractionType.CAMERA);

			foreach (Room room in rooms)
				plugin.Vars.BlackoutRoom.Add(room);
		}

		public void ToggleBlackout()
		{
			plugin.Vars.Toggled = !plugin.Vars.Toggled;
			
			if (plugin.TimedEvents)
			{
				plugin.Vars.TimedOverride = true;
				plugin.TimedEvents = false;
			}
			else if (plugin.Vars.TimedOverride)
			{
				plugin.Vars.TimedOverride = false;
				plugin.TimedEvents = true;
			}

			if (!plugin.Vars.Toggled) 
				return;

			if (plugin.Announce)
				PlayerManager.localPlayer.GetComponent<MTFRespawn>().CallRpcPlayCustomAnnouncement(
					!plugin.ToggledLcz
						? "HEAVY CONTAINMENT POWER SYSTEM FAILURE IN 3. . 2. . 1. . "
						: "FACILITY POWER SYSTEM FAILURE IN 3. . 2. . 1. . ", false);

			plugin.Coroutines.Add(Timing.RunCoroutine(ToggledBlackout(8.7f)));
		}

		public bool IsAllowed(ICommandSender sender)
		{
			if (!(sender is Player player)) return true;

			List<string> roleList = plugin.ValidRanks != null && plugin.ValidRanks.Length > 0
				? plugin.ValidRanks.Select(role => role.ToUpper()).ToList()
				: new List<string>();

			if (roleList.Count > 0 && (roleList.Contains(player.GetUserGroup().Name.ToUpper()) || roleList.Contains(player.GetRankName().ToUpper())))
				return true;
			return roleList.Count == 0;
		}

		public void EnableBlackouts() => plugin.TimedEvents = true;

		public void DisableBlackouts() => plugin.TimedEvents = false;

		public void EnableAnnounce() => plugin.Announce = true;

		public void DisableAnnounce() => plugin.Announce = false;
	}
}
