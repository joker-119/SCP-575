using System;
using System.Collections.Generic;
using Exiled.API.Enums;
using Exiled.API.Features;
using MEC;
using Respawning;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = System.Random;
using Handlers = Exiled.Events.Handlers;
using Exiled.API.Features.Items;

namespace SCP_575
{
	public class Plugin : Exiled.API.Features.Plugin<Config>
	{
		public override string Author { get; } = "Galaxy119";
		public override string Name { get; } = "SCP-575";
		public override string Prefix { get; } = "575";
		public override Version Version { get; } = new Version(3, 7, 0);
		public override Version RequiredExiledVersion { get; } = new Version(3, 0, 0);
		
		public Random Gen = new Random();
		
		public EventHandlers EventHandlers;
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
			yield return Timing.WaitForSeconds(Config.InitialDelay);

			for (;;)
			{
				RespawnEffectsController.PlayCassieAnnouncement(Config.CassieMessageStart, false, true);

				if (Config.DisableTeslas)
					EventHandlers.TeslasDisabled = true;
				TimerOn = true;
				yield return Timing.WaitForSeconds(8.7f);
			
				float blackoutDur = Config.DurationMax;
				if (Config.RandomEvents)
					blackoutDur = (float)Gen.NextDouble() * (Config.DurationMax - Config.DurationMin) + Config.DurationMin;
				if (Config.EnableKeter)
					EventHandlers.Coroutines.Add(Timing.RunCoroutine(Keter(blackoutDur), "keter"));

				List<ZoneType> affectedZones = new List<ZoneType>();

				if (Config.OnlyHeavy) affectedZones.Add(ZoneType.HeavyContainment);
				else
					affectedZones.AddRange(new List<ZoneType> { ZoneType.Surface, ZoneType.Entrance, ZoneType.HeavyContainment, ZoneType.LightContainment });

				foreach (ZoneType type in affectedZones)
					Map.TurnOffAllLights(blackoutDur, type);

				if (Config.Voice)
					RespawnEffectsController.PlayCassieAnnouncement(Config.CassieKeter, false, false);
				yield return Timing.WaitForSeconds(blackoutDur - 8.7f);
				RespawnEffectsController.PlayCassieAnnouncement(Config.CassieMessageEnd, false, true);
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
					if (player.CurrentRoom.LightsOff && !player.HasFlashlightModuleEnabled && !(player.CurrentItem is Flashlight flashlight && flashlight.Active) && player.ReferenceHub.characterClassManager.IsHuman())
						player.Hurt(Config.KeterDamage, DamageTypes.Bleeding, Config.KilledBy);

					yield return Timing.WaitForSeconds(5f);
				}
			} while ((dur -= 5f) > 5f);
		}
	}
}