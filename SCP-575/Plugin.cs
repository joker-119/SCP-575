using System;
using System.Collections.Generic;
using Exiled.API.Features;
using Exiled.API.Interfaces;
using Exiled.Events;
using MEC;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = System.Random;
using Handlers = Exiled.Events.Handlers;

namespace SCP_575
{
	public class Plugin : Exiled.API.Features.Plugin
	{
		public Random Gen = new Random();
		
		public EventHandlers EventHandlers;
		public MTFRespawn Respawn;
		public static bool TimerOn;
		
		public Config cfg;

		public override void OnEnabled()
		{
			try
			{
				Log.Info("loaded.");
				cfg = (Config) Config;
				cfg.Reload();
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

		public override IConfig Config { get; } = new Config();

		public IEnumerator<float> RunBlackoutTimer()
		{
			if (Respawn == null)
				Respawn = PlayerManager.localPlayer.GetComponent<MTFRespawn>();
			yield return Timing.WaitForSeconds(cfg.InitialDelay);

			for (;;)
			{
				Respawn.RpcPlayCustomAnnouncement("facility power system failure in 3 . 2 . 1 .", false, true);

				if (cfg.DisableTeslas)
					EventHandlers.TeslasDisabled = true;
				TimerOn = true;
				yield return Timing.WaitForSeconds(8.7f);
			
				float blackoutDur = cfg.DurationMax;
				if (cfg.RandomEvents)
					blackoutDur = (float)Gen.NextDouble() * (cfg.DurationMax - cfg.DurationMin) + cfg.DurationMin;
				if (cfg.EnableKeter)
					EventHandlers.Coroutines.Add(Timing.RunCoroutine(Keter(blackoutDur), "keter"));

				Generator079.generators[0].RpcCustomOverchargeForOurBeautifulModCreators(blackoutDur, cfg.OnlyHeavy);
				if (cfg.Voice)
					Respawn.RpcPlayCustomAnnouncement("pitch_0.15 .g7", false, false);
				yield return Timing.WaitForSeconds(blackoutDur - 8.7f);
				Respawn.RpcPlayCustomAnnouncement("facility power system now operational", false, true);
				yield return Timing.WaitForSeconds(8.7f);
				Timing.KillCoroutines("keter");
				EventHandlers.TeslasDisabled = false;
				TimerOn = false;
				if (cfg.RandomEvents)
					yield return Timing.WaitForSeconds(Gen.Next(cfg.DelayMin, cfg.DelayMax));
				else
					yield return Timing.WaitForSeconds(cfg.InitialDelay);
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
								player.ReferenceHub.playerStats.HurtPlayer(new PlayerStats.HitInfo(cfg.KeterDamage, "SCP-575", DamageTypes.Wall, 0), player.GameObject);
								player.Broadcast(5, "You were damaged by SCP-575! Equip a flashlight!", Broadcast.BroadcastFlags.Normal);
							}

					yield return Timing.WaitForSeconds(5f);
				}
			} while ((dur -= 5f) > 5f);
		}
	}
}