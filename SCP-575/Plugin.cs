using System;
using System.Collections.Generic;
using Exiled.API.Enums;
using Exiled.API.Features;
using MEC;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = System.Random;
using Handlers = Exiled.Events.Handlers;

namespace SCP_575
{
	public class Plugin : Exiled.API.Features.Plugin<Config>
	{
		public Random Gen = new Random();
		
		public EventHandlers EventHandlers;
		public MTFRespawn Respawn;
		public static bool TimerOn;
		public override PluginPriority Priority { get; } = PluginPriority.Default;

		public override void OnEnabled()
		{
			try
			{
				Log.Info("loaded.");
				Log.Info("Configs loaded.");
				
				EventHandlers = new EventHandlers(this);

				Handlers.Server.RoundStarted += EventHandlers.OnRoundStart;
				Handlers.Server.RoundEnded += EventHandlers.OnRoundEnd;
				Handlers.Server.WaitingForPlayers += EventHandlers.OnWaitingForPlayers;
				Handlers.Player.TriggeringTesla += EventHandlers.OnTriggerTesla;
			}
			catch (Exception e)
			{
				Log.Error($"OnEnable Error: {e}");
			}
		}

		public override void OnDisabled()
		{
			foreach (CoroutineHandle handle in EventHandlers.Coroutines)
				Timing.KillCoroutines(handle);
			Handlers.Server.RoundStarted -= EventHandlers.OnRoundStart;
			Handlers.Server.RoundEnded -= EventHandlers.OnRoundEnd;
			Handlers.Server.WaitingForPlayers -= EventHandlers.OnWaitingForPlayers;
			Handlers.Player.TriggeringTesla -= EventHandlers.OnTriggerTesla;
			EventHandlers = null;
		}

		public override void OnReloaded()
		{
			
		}

		public IEnumerator<float> RunBlackoutTimer()
		{
			if (Respawn == null)
				Respawn = PlayerManager.localPlayer.GetComponent<MTFRespawn>();
			yield return Timing.WaitForSeconds(Config.InitialDelay);

			for (;;)
			{
				Respawn.RpcPlayCustomAnnouncement("facility power system failure in 3 . 2 . 1 .", false, true);

				if (Config.DisableTeslas)
					EventHandlers.TeslasDisabled = true;
				TimerOn = true;
				yield return Timing.WaitForSeconds(8.7f);
			
				float blackoutDur = Config.DurationMax;
				if (Config.RandomEvents)
					blackoutDur = (float)Gen.NextDouble() * (Config.DurationMax - Config.DurationMin) + Config.DurationMin;
				if (Config.EnableKeter)
					EventHandlers.Coroutines.Add(Timing.RunCoroutine(Keter(blackoutDur), "keter"));

				Generator079.Generators[0].RpcCustomOverchargeForOurBeautifulModCreators(blackoutDur, Config.OnlyHeavy);
				if (Config.Voice)
					Respawn.RpcPlayCustomAnnouncement("pitch_0.15 .g7", false, false);
				yield return Timing.WaitForSeconds(blackoutDur - 8.7f);
				Respawn.RpcPlayCustomAnnouncement("facility power system now operational", false, true);
				yield return Timing.WaitForSeconds(8.7f);
				Timing.KillCoroutines("keter");
				EventHandlers.TeslasDisabled = false;
				TimerOn = false;
				if (Config.RandomEvents)
					yield return Timing.WaitForSeconds(Gen.Next(Config.DelayMin, Config.DelayMax));
				else
					yield return Timing.WaitForSeconds(Config.InitialDelay);
			}
		}

		public IEnumerator<float> Keter(float dur)
		{
			do
			{
				foreach (Player player in Player.List)
				{
					bool damaged = false;
					foreach (FlickerableLight light in Object.FindObjectsOfType<FlickerableLight>())
						if (Vector3.Distance(light.transform.position, player.Position) < 10f && !damaged)
							if (player.ReferenceHub.characterClassManager.IsHuman() &&
							    player.Role != RoleType.Spectator && !player.ReferenceHub.HasLightSource())
							{
								damaged = true;
								player.ReferenceHub.playerStats.HurtPlayer(new PlayerStats.HitInfo(Config.KeterDamage, "SCP-575", DamageTypes.Wall, 0), player.GameObject);
								player.Broadcast(5, "You were damaged by SCP-575! Equip a flashlight!", Broadcast.BroadcastFlags.Normal);
							}

					yield return Timing.WaitForSeconds(5f);
				}
			} while ((dur -= 5f) > 5f);
		}
	}
}