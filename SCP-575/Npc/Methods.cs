namespace SCP_575.Npc
{
	using System.Collections.Generic;
	using Exiled.API.Enums;
	using Exiled.API.Features.Items;
	using Exiled.Events.Handlers;
	using Exiled.Loader;
	using MEC;
	using Respawning;
	using Map = Exiled.API.Features.Map;

	public class Methods
    {
        private readonly Plugin _plugin;
        public Methods(Plugin plugin) => _plugin = plugin;

        public void Init()
        {
	        Server.RoundEnded += _plugin.Npc.EventHandlers.OnRoundEnd;
	        Server.RoundStarted += _plugin.Npc.EventHandlers.OnRoundStart;
	        Player.TriggeringTesla += _plugin.Npc.EventHandlers.OnTriggerTesla;
	        _plugin.Npc.EventHandlers.OnWaitingForPlayers();
        }

        public void Disable()
        {
	        Server.RoundEnded -= _plugin.Npc.EventHandlers.OnRoundEnd;
	        Server.RoundStarted -= _plugin.Npc.EventHandlers.OnRoundStart;
	        Player.TriggeringTesla -= _plugin.Npc.EventHandlers.OnTriggerTesla;
        }

        public IEnumerator<float> RunBlackoutTimer()
        		{
        			yield return Timing.WaitForSeconds(_plugin.Config.NpcConfig.InitialDelay);
        
        			for (;;)
        			{
        				RespawnEffectsController.PlayCassieAnnouncement(_plugin.Config.NpcConfig.CassieMessageStart, false, true);
        
        				if (_plugin.Config.NpcConfig.DisableTeslas)
        					_plugin.EventHandlers.TeslasDisabled = true;
        				yield return Timing.WaitForSeconds(8.7f);
        			
        				float blackoutDur = _plugin.Config.NpcConfig.DurationMax;
        				if (_plugin.Config.NpcConfig.RandomEvents)
        					blackoutDur = (float)Loader.Random.NextDouble() * (_plugin.Config.NpcConfig.DurationMax - _plugin.Config.NpcConfig.DurationMin) + _plugin.Config.NpcConfig.DurationMin;
        				if (_plugin.Config.NpcConfig.EnableKeter)
        					_plugin.EventHandlers.Coroutines.Add(Timing.RunCoroutine(Keter(blackoutDur), "keter"));
        
        				Map.TurnOffAllLights(blackoutDur, _plugin.Config.NpcConfig.OnlyHeavy ? ZoneType.HeavyContainment : ZoneType.Unspecified);
        				if (_plugin.Config.NpcConfig.Voice)
        					RespawnEffectsController.PlayCassieAnnouncement(_plugin.Config.NpcConfig.CassieKeter, false, false);
        				yield return Timing.WaitForSeconds(blackoutDur - 8.7f);
        				RespawnEffectsController.PlayCassieAnnouncement(_plugin.Config.NpcConfig.CassieMessageEnd, false, true);
        				yield return Timing.WaitForSeconds(8.7f);
        				Timing.KillCoroutines("keter");
        				_plugin.EventHandlers.TeslasDisabled = false;
        				if (_plugin.Config.NpcConfig.RandomEvents)
        					yield return Timing.WaitForSeconds(Loader.Random.Next(_plugin.Config.NpcConfig.DelayMin, _plugin.Config.NpcConfig.DelayMax));
        				else
        					yield return Timing.WaitForSeconds(_plugin.Config.NpcConfig.InitialDelay);
        			}
        		}
        
        		public IEnumerator<float> Keter(float dur)
        		{
        			do
        			{
        				foreach (Exiled.API.Features.Player player in Exiled.API.Features.Player.List)
        				{
        					if (player.CurrentRoom.LightsOff && player.IsHuman && !player.HasFlashlightModuleEnabled && (!(player.CurrentItem is Flashlight flashlight) || !flashlight.Active))
        					{
        						player.Hurt(_plugin.Config.NpcConfig.KilledBy, _plugin.Config.NpcConfig.KeterDamage);
        						player.Broadcast(_plugin.Config.NpcConfig.KeterBroadcast);
        					}
        
        					yield return Timing.WaitForSeconds(5f);
        				}
        			} while ((dur -= 5f) > 5f);
        		}
    }
}