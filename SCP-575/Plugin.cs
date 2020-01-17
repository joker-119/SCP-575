using System;
using System.Collections.Generic;
using EXILED;
using MEC;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = System.Random;

namespace SCP_575
{
	public class Plugin : EXILED.Plugin
	{
		public Random Gen = new Random();
		
		public EventHandlers EventHandlers;
		public MTFRespawn Respawn;
		public static bool TimerOn;
		
		public bool RandomEvents;
		public bool DisableTeslas;
		public bool EnableKeter;
		public bool OnlyHeavy;
		public bool Voice;
		public float InitialDelay;
		public float DurationMin;
		public float DurationMax;
		public int DelayMax;
		public int DelayMin;
		public int SpawnChance;
		public float KeterDamage;

		public override void OnEnable()
		{
			try
			{
				Info("loaded.");
				ReloadConfig();
				Info("Configs loaded.");
				EventHandlers = new EventHandlers(this);

				Events.RoundStartEvent += EventHandlers.OnRoundStart;
				Events.RoundEndEvent += EventHandlers.OnRoundEnd;
				Events.WaitingForPlayersEvent += EventHandlers.OnWaitingForPlayers;
				Events.TriggerTeslaEvent += EventHandlers.OnTriggerTesla;
			}
			catch (Exception e)
			{
				Error($"OnEnable Error: {e}");
			}
		}

		public override void OnDisable()
		{
			Events.RoundStartEvent -= EventHandlers.OnRoundStart;
			Events.RoundEndEvent -= EventHandlers.OnRoundEnd;
			Events.WaitingForPlayersEvent -= EventHandlers.OnWaitingForPlayers;
			Events.TriggerTeslaEvent -= EventHandlers.OnTriggerTesla;
			EventHandlers = null;
		}

		public override void OnReload()
		{
			
		}

		public override string getName { get; } = "SCP-575";

		public void ReloadConfig()
		{
			Info($"Config Path: {Config.Path}");
			RandomEvents = Config.GetBool("575_random_events", true);
			DisableTeslas = Config.GetBool("575_disable_teslas", true);
			InitialDelay = Config.GetFloat("575_initial_delay", 300f);
			DurationMin = Config.GetFloat("575_dur_min", 30f);
			DurationMax = Config.GetFloat("575_dur_max", 90);
			DelayMin = Config.GetInt("575_delay_min", 180);
			DelayMax = Config.GetInt("575_delay_max", 500);
			SpawnChance = Config.GetInt("575_spawn_chance", 45);
			EnableKeter = Config.GetBool("575_keter", true);
			OnlyHeavy = Config.GetBool("575_only_hcz", false);
			Voice = Config.GetBool("575_voice", true);
			KeterDamage = Config.GetFloat("575_keter_dmg", 10f);
		}

		public IEnumerator<float> RunBlackoutTimer()
		{
			if (Respawn == null)
				Respawn = PlayerManager.localPlayer.GetComponent<MTFRespawn>();
			yield return Timing.WaitForSeconds(InitialDelay);

			for (;;)
			{
				Respawn.RpcPlayCustomAnnouncement("facility power system failure in 3 . 2 . 1 .", false, true);

				if (DisableTeslas)
					EventHandlers.TeslasDisabled = true;
				TimerOn = true;
				yield return Timing.WaitForSeconds(8.7f);
			
				float blackoutDur = DurationMax;
				if (RandomEvents)
					blackoutDur = (float)Gen.NextDouble() * (DurationMax - DurationMin) + DurationMin;
				if (EnableKeter)
					Timing.RunCoroutine(Keter(blackoutDur));

				Generator079.generators[0].RpcCustomOverchargeForOurBeautifulModCreators(blackoutDur, OnlyHeavy);
				if (Voice)
					Respawn.RpcPlayCustomAnnouncement("pitch_0.15 .g7", false, false);
				yield return Timing.WaitForSeconds(blackoutDur - 8.7f);
				Respawn.RpcPlayCustomAnnouncement("facility power system now operational", false, true);
				yield return Timing.WaitForSeconds(8.7f);
				EventHandlers.TeslasDisabled = false;
				TimerOn = false;
				if (RandomEvents)
					yield return Timing.WaitForSeconds(Gen.Next(DelayMin, DelayMax));
				else
					yield return Timing.WaitForSeconds(InitialDelay);
			}
		}

		public IEnumerator<float> Keter(float dur)
		{
			do
			{
				foreach (ReferenceHub hub in GetHubs())
				{
					bool damaged = false;
					foreach (FlickerableLight light in Object.FindObjectsOfType<FlickerableLight>())
						if (Vector3.Distance(light.transform.position, hub.gameObject.transform.position) < 10f && !damaged)
							if (hub.characterClassManager.IsHuman() &&
							    hub.characterClassManager.CurClass != RoleType.Spectator && !hub.HasLightSource())
							{
								hub.playerStats.HurtPlayer(new PlayerStats.HitInfo(KeterDamage, "SCP-575", DamageTypes.Wall, 0), hub.gameObject);
								hub.Broadcast(5, "You were damaged by SCP-575! Equip a flashlight!");
							}

					yield return Timing.WaitForSeconds(5f);
				}
			} while ((dur -= 5f) > 5f);
		}
	}
}