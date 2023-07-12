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
            Player.ActivatingWarheadPanel += _plugin.Npc.EventHandlers.onTriggerNukeStart;
            _plugin.Npc.EventHandlers.OnWaitingForPlayers();
        }

        public void Disable()
        {
            Server.RoundEnded -= _plugin.Npc.EventHandlers.OnRoundEnd;
            Server.RoundStarted -= _plugin.Npc.EventHandlers.OnRoundStart;
            Player.TriggeringTesla -= _plugin.Npc.EventHandlers.OnTriggerTesla;
            Player.ActivatingWarheadPanel -= _plugin.Npc.EventHandlers.onTriggerNukeStart;
        }

        public IEnumerator<float> RunBlackoutTimer()
        {
            yield return Timing.WaitForSeconds(_plugin.Config.NpcConfig.InitialDelay);
            if (_plugin.Config.NpcConfig.RandomEvents) yield return Timing.WaitForSeconds(Loader.Random.Next(_plugin.Config.NpcConfig.DelayMin, _plugin.Config.NpcConfig.DelayMax));
            for (; ; )
            {
                bool isBlackout = false;

                Exiled.API.Features.Cassie.GlitchyMessage(_plugin.Config.NpcConfig.CassieMessageStart, _plugin.Config.NpcConfig.GlitchChance/100, _plugin.Config.NpcConfig.JamChance/100);

                if (_plugin.Config.NpcConfig.FlickerLights)
                {
                    Map.TurnOffAllLights(_plugin.Config.NpcConfig.FlickerLightsDuration, ZoneType.Entrance);
                    Map.TurnOffAllLights(_plugin.Config.NpcConfig.FlickerLightsDuration, ZoneType.Surface);
                    Map.TurnOffAllLights(_plugin.Config.NpcConfig.FlickerLightsDuration, ZoneType.LightContainment);
                    Map.TurnOffAllLights(_plugin.Config.NpcConfig.FlickerLightsDuration, ZoneType.HeavyContainment);
                }

                yield return Timing.WaitForSeconds(_plugin.Config.NpcConfig.TimeBetweenSentenceAndStart);
                float blackoutDur = _plugin.Config.NpcConfig.DurationMax;
                if (_plugin.Config.NpcConfig.RandomEvents) blackoutDur = (float)Loader.Random.NextDouble() * (_plugin.Config.NpcConfig.DurationMax - _plugin.Config.NpcConfig.DurationMin) + _plugin.Config.NpcConfig.DurationMin;
                if (_plugin.Config.NpcConfig.EnableKeter) _plugin.EventHandlers.Coroutines.Add(Timing.RunCoroutine(Keter(blackoutDur), "keter"));

                Exiled.API.Features.Cassie.GlitchyMessage(_plugin.Config.NpcConfig.CassiePostMessage, _plugin.Config.NpcConfig.GlitchChance/100, _plugin.Config.NpcConfig.JamChance/100);
                //Per Zone
                if (_plugin.Config.NpcConfig.UsePerRoomChances == false)
                {
                    List<ZoneType> zones = new List<ZoneType>();

                    bool isOtherMessage = false;
                    if (((float)Loader.Random.NextDouble() * 100) < _plugin.Config.NpcConfig.ChanceLight)
                    {
                        Map.TurnOffAllLights(blackoutDur, ZoneType.LightContainment);
                        isBlackout = true;
                        Exiled.API.Features.Cassie.Message(_plugin.Config.NpcConfig.CassieMessageLight, false, false);
                    }
                    if (((float)Loader.Random.NextDouble() * 100) < _plugin.Config.NpcConfig.ChanceHeavy)
                    {
                        if (_plugin.Config.NpcConfig.DisableTeslas) _plugin.EventHandlers.TeslasDisabled = true;
                        if (_plugin.Config.NpcConfig.DisableNuke)
                        {
                            _plugin.EventHandlers.NukeDisabled = true;
                            if (Exiled.API.Features.Warhead.Controller.isActiveAndEnabled) Exiled.API.Features.Warhead.Controller.CancelDetonation();
                        }
                        Map.TurnOffAllLights(blackoutDur, ZoneType.HeavyContainment);
                        isBlackout = true;
                        Exiled.API.Features.Cassie.Message(_plugin.Config.NpcConfig.CassieMessageHeavy, false, false);

                    }
                    if (((float)Loader.Random.NextDouble() * 100) < _plugin.Config.NpcConfig.ChanceEntrance)
                    {
                        Map.TurnOffAllLights(blackoutDur, ZoneType.Entrance);
                        isBlackout = true;
                        Exiled.API.Features.Cassie.Message(_plugin.Config.NpcConfig.CassieMessageEntrance, false, false);
                    }
                    if (((float)Loader.Random.NextDouble() * 100) < _plugin.Config.NpcConfig.ChanceSurface)
                    {
                        Map.TurnOffAllLights(blackoutDur, ZoneType.Surface);
                        isBlackout = true;
                        Exiled.API.Features.Cassie.Message(_plugin.Config.NpcConfig.CassieMessageSurface, false, false);
                    }
                    if (((float)Loader.Random.NextDouble() * 100) < _plugin.Config.NpcConfig.ChanceOther)
                    {
                        Map.TurnOffAllLights(blackoutDur, ZoneType.Other);
                        isBlackout = true;
                        if (!isOtherMessage) Exiled.API.Features.Cassie.Message(_plugin.Config.NpcConfig.CassieMessageOther, false, false);
                        isOtherMessage = true;
                    }
                    if (((float)Loader.Random.NextDouble() * 100) < _plugin.Config.NpcConfig.ChanceUnspecified)
                    {
                        Map.TurnOffAllLights(blackoutDur, ZoneType.Unspecified);
                        isBlackout = true;
                        if (!isOtherMessage) Exiled.API.Features.Cassie.Message(_plugin.Config.NpcConfig.CassieMessageOther, false, false);
                        isOtherMessage = true;
                    }
                    if (!isBlackout && _plugin.Config.NpcConfig.EnableFacilityBlackout)
                    {
                        isBlackout = true;
                        if (_plugin.Config.NpcConfig.DisableTeslas) _plugin.EventHandlers.TeslasDisabled = true;
                        if (_plugin.Config.NpcConfig.DisableNuke)
                        {
                            _plugin.EventHandlers.NukeDisabled = true;
                            if (Exiled.API.Features.Warhead.Controller.isActiveAndEnabled) Exiled.API.Features.Warhead.Controller.CancelDetonation();
                        }
                        Map.TurnOffAllLights(blackoutDur, ZoneType.Entrance);
                        Map.TurnOffAllLights(blackoutDur, ZoneType.Surface);
                        Map.TurnOffAllLights(blackoutDur, ZoneType.LightContainment);
                        Map.TurnOffAllLights(blackoutDur, ZoneType.HeavyContainment);
                        Exiled.API.Features.Cassie.Message(_plugin.Config.NpcConfig.CassieMessageFacility, false, false);
                        

                    }

                }
                // Per Room 
                else
                {

                    foreach (Exiled.API.Features.Room r in Exiled.API.Features.Room.List)
                    {
                        //Heavy 
                        if (r.Type.ToString().Contains("Hcz"))
                        {
                            if (((float)Loader.Random.NextDouble() * 100) < _plugin.Config.NpcConfig.ChanceHeavy)
                            {
                                if (_plugin.Config.NpcConfig.DisableNuke && r.Type.ToString().ToLower().Contains("nuke"))
                                {
                                    _plugin.EventHandlers.NukeDisabled = true;
                                    if (Exiled.API.Features.Warhead.Controller.isActiveAndEnabled) Exiled.API.Features.Warhead.Controller.CancelDetonation();
                                }
                                if (_plugin.Config.NpcConfig.DisableTeslas && r.Type.ToString().ToLower().Contains("tesla"))
                                {
                                    _plugin.EventHandlers.TeslasDisabled = true;
                                }
                                r.TurnOffLights(blackoutDur);
                                isBlackout = true;

                            }
                        }
                        //Light
                        else if (r.Type.ToString().Contains("Lcz"))
                        {
                            if (((float)Loader.Random.NextDouble() * 100) < _plugin.Config.NpcConfig.ChanceLight)
                            {
                                r.TurnOffLights(blackoutDur); isBlackout = true;
                            }
                        }
                        //Entrance 
                        else if (r.Type.ToString().Contains("Ez"))
                        {
                            if (((float)Loader.Random.NextDouble() * 100) < _plugin.Config.NpcConfig.ChanceEntrance)
                            {
                                r.TurnOffLights(blackoutDur); isBlackout = true;
                            }
                        }
                        //Surface 
                        else if (r.Type.ToString().Contains("Surface"))
                        {
                            if (((float)Loader.Random.NextDouble() * 100) < _plugin.Config.NpcConfig.ChanceSurface)
                            {
                                r.TurnOffLights(blackoutDur); isBlackout = true;
                            }
                        }
                        //Misc
                        else
                        {
                            if (((float)Loader.Random.NextDouble() * 100) < _plugin.Config.NpcConfig.ChanceOther)
                            {
                                r.TurnOffLights(blackoutDur); isBlackout = true;
                            }
                        }

                    }
                    if (!isBlackout)
                    {
                        if (_plugin.Config.NpcConfig.EnableFacilityBlackout)
                        {
                            isBlackout = true;
                            if (_plugin.Config.NpcConfig.DisableTeslas) _plugin.EventHandlers.TeslasDisabled = true;
                            if (_plugin.Config.NpcConfig.DisableNuke)
                            {
                                _plugin.EventHandlers.NukeDisabled = true;
                                if (Exiled.API.Features.Warhead.Controller.isActiveAndEnabled) Exiled.API.Features.Warhead.Controller.CancelDetonation();
                            }
                            Map.TurnOffAllLights(blackoutDur, ZoneType.Entrance);
                            Map.TurnOffAllLights(blackoutDur, ZoneType.Surface);
                            Map.TurnOffAllLights(blackoutDur, ZoneType.LightContainment);
                            Map.TurnOffAllLights(blackoutDur, ZoneType.HeavyContainment);
                            Exiled.API.Features.Cassie.Message(_plugin.Config.NpcConfig.CassieMessageFacility, false, false);
                        }
                    }
                    else Exiled.API.Features.Cassie.Message(_plugin.Config.NpcConfig.CassieMessageOther, false, false);

                }

                // End Event
                if (isBlackout)
                {
                    if (_plugin.Config.NpcConfig.Voice) Exiled.API.Features.Cassie.Message(_plugin.Config.NpcConfig.CassieKeter, false, false);
                    yield return Timing.WaitForSeconds(blackoutDur - _plugin.Config.NpcConfig.TimeBetweenSentenceAndStart);
                    Exiled.API.Features.Cassie.Message(_plugin.Config.NpcConfig.CassieMessageEnd, false, false);
                    yield return Timing.WaitForSeconds(8.0f);
                }
                else Exiled.API.Features.Cassie.Message(_plugin.Config.NpcConfig.CassieMessageWrong, false, false);

                Timing.KillCoroutines("keter");
                _plugin.EventHandlers.TeslasDisabled = false;
                _plugin.EventHandlers.NukeDisabled = false;
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
                    if (player.CurrentRoom.AreLightsOff && player.IsHuman && !player.HasFlashlightModuleEnabled && (!(player.CurrentItem is Flashlight flashlight) || !flashlight.Active))
                    {
                        player.Hurt(_plugin.Config.NpcConfig.KeterDamage, _plugin.Config.NpcConfig.KilledBy);
                        player.Broadcast(_plugin.Config.NpcConfig.KeterBroadcast);
                    }

                    yield return Timing.WaitForSeconds(_plugin.Config.NpcConfig.KeterDamageDelay);
                }
            } while ((dur -= _plugin.Config.NpcConfig.KeterDamageDelay) > _plugin.Config.NpcConfig.KeterDamageDelay);
        }
    }
}