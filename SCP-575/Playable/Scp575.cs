namespace SCP_575.Playable
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using CustomPlayerEffects;
    using Exiled.API.Enums;
    using Exiled.API.Features;
    using Exiled.API.Features.Attributes;
    using Exiled.API.Features.Doors;
    using Exiled.API.Features.Roles;
    using Exiled.API.Features.Spawn;
    using Exiled.CustomRoles.API.Features;
    using Exiled.Events.EventArgs.Map;
    using Exiled.Events.EventArgs.Player;
    using Exiled.Events.EventArgs.Scp106;
    using Exiled.Events.Handlers;
    using Footprinting;
    using MEC;
    using Mirror;
    using PlayerRoles;
    using PlayerRoles.PlayableScps;
    using PlayerStatsSystem;
    using UnityEngine;
    using Cassie = Exiled.API.Features.Cassie;
    using Map = Exiled.Events.Handlers.Map;
    using Player = Exiled.API.Features.Player;
    using Server = Exiled.API.Features.Server;

    [CustomRole(RoleTypeId.Scp106)]
    public class Scp575 : CustomRole
    {
        public readonly Dictionary<Player, int> ConsumptionStacks = new();

        public override uint Id { get; set; } = 12;
        public override RoleTypeId Role { get; set; } = RoleTypeId.Scp106;
        public override int MaxHealth { get; set; } = 550;
        public override string Name { get; set; } = "SCP-575";
        public override string Description { get; set; } = "An entity that appears as a shapeless void, that moves slowly but grows in power the more biological material it consumes. Capable of causing wide-spread power outages.\n\nUse client command \".special\" to trigger a blackout. This can be keyboudn with \"cmdbind KEY .special\"";
        public override string CustomInfo { get; set; } = "SCP-575";

        [Description("The base(minimum) damage his hits will deal.")]
        public float BaseDamage { get; set; } = 30;

        [Description("Damage calculation is base x (consumptionStacks / this value)")]
        public int DamageScalar { get; set; } = 2;

        [Description("The maximum number of consumption stacks 575 can have.")]
        public int MaxConsumption { get; set; } = 10;

        [Description("The amount of consumption stacks he must have to use his ability.")]
        public int AbilityStackRequirement { get; set; } = 5;

        [Description("The base(maximum) damage a flashbang will deal to 575.")]
        public float FlashbangBaseDamage { get; set; } = 450f;

        [Description("The amount of damage removed from the base amount for every 1ft away from the grenade he is.")]
        public float FlashbangFalloffMultiplier { get; set; } = 20f;

        [Description("Whether or not consumption stacks are removed when he is hit with a flashbang.")]
        public bool ResetConsumptionOnFlashed { get; set; } = true;

        [Description("Whether or not 575 is teleported to a random HCZ room when flashbanged.")]
        public bool TeleportOnFlashed { get; set; } = true;

        public override SpawnProperties SpawnProperties { get; set; } = new()
        {
            RoleSpawnPoints = new List<RoleSpawnPoint>
            {
                new()
                {
                    Role = RoleTypeId.Scp173,
                    Chance = 10,
                },
                new()
                {
                    Role = RoleTypeId.Scp079,
                    Chance = 20,
                },
                new()
                {
                    Role = RoleTypeId.Scp049,
                    Chance = 30,
                },
                new()
                {
                    Role = RoleTypeId.Scp106,
                    Chance = 100,
                }
            }
        };

        public override List<CustomAbility> CustomAbilities { get; set; } = new()
        {
            new BlackoutAbility(),
        };

        private List<CoroutineHandle> _coroutines = new();

        protected override void RoleAdded(Player player)
        {
            try
            {
                foreach (CustomAbility ability in CustomAbilities)
                {
                    if (ability is BlackoutAbility blackout)
                    {
                        blackout.CanUseOverride = () => CanUseAbility(player, blackout);
                    }
                }

                if (!ConsumptionStacks.ContainsKey(player))
                    ConsumptionStacks[player] = 1;
                player.InfoArea &= ~PlayerInfoArea.UnitName;
                player.CustomInfo = "<color=red>SCP-575</color>";
                Log.Debug($"{Name} added to {player.Nickname}");
                Cassie.GlitchyMessage("Alert . scp 5 7 5 has breached containment", 0.5f, 0.1f);
                _coroutines.Add(Timing.RunCoroutine(Invisibility(player.Role as FpcRole)));
            }
            catch (Exception e)
            {
                Log.Error($"{Name}: {e}\n{e.StackTrace}");
            }
        }

        protected override void SubscribeEvents()
        {
            Log.Debug($"{Name} loading events.");
            Exiled.Events.Handlers.Player.Dying += OnDying;
            Exiled.Events.Handlers.Player.Hurting += OnHurting;
            Scp106.Teleporting += OnTeleporting;
            Map.ExplodingGrenade += OnExplodingGrenade;
            Map.AnnouncingScpTermination += OnAnnouncingScpTermination;
            Exiled.Events.Handlers.Player.EnteringPocketDimension += OnEnteringPocketDimension;
            base.SubscribeEvents();
        }

        protected override void UnsubscribeEvents()
        {
            Log.Debug($"{Name} unloading events.");
            Exiled.Events.Handlers.Player.Dying -= OnDying;
            Exiled.Events.Handlers.Player.Hurting -= OnHurting;
            Scp106.Teleporting -= OnTeleporting;
            Map.ExplodingGrenade -= OnExplodingGrenade;
            Map.AnnouncingScpTermination -= OnAnnouncingScpTermination;
            Exiled.Events.Handlers.Player.EnteringPocketDimension -= OnEnteringPocketDimension;
            base.UnsubscribeEvents();
        }

        public bool CanUseAbility(Player player, BlackoutAbility ability)
        {
            if (!ConsumptionStacks.ContainsKey(player))
                ConsumptionStacks.Add(player, 1);

            if (ConsumptionStacks[player] < AbilityStackRequirement)
            {
                player.ShowHint($"You are unable to use Blackout until you are power level {AbilityStackRequirement}. You are currently at {ConsumptionStacks[player]}. Gain power levels by killing players.");
                return false;
            }

            DateTime usableTime = ability.LastUsed[player] + TimeSpan.FromSeconds(ability.Cooldown);
            bool flag = DateTime.Now > usableTime;
            if (!flag)
                player.ShowHint($"You must wait another {Math.Round((DateTime.Now - usableTime).TotalSeconds, 2)} more seconds to use this ability.");

            return flag;
        }

        private void OnAnnouncingScpTermination(AnnouncingScpTerminationEventArgs ev)
        {
            if (ev.Role.Type == RoleTypeId.Scp106)
            {
                string message = $"scp 5 7 5 has been successfully terminated . termination cause {ev.TerminationCause}";

                ev.IsAllowed = false;
                Cassie.Message(message);
            }
        }

        protected override void RoleRemoved(Player player)
        {
            foreach (CoroutineHandle handle in _coroutines)
                Timing.KillCoroutines(handle);
            player.DisableEffect<Invisible>();
        }

        private void OnDying(DyingEventArgs ev)
        {
            if (Check(ev.Attacker) && !Check(ev.Player))
                IncreasePower(ev.Attacker);
            else if (Check(ev.Player))
            {
                Log.Warn($"Adding {ev.Player.Nickname} to stop doll list.");
                Plugin.Singleton.StopRagdollList.Add(ev.Player);
                RagdollData info = new(ev.Player.ReferenceHub, ev.DamageHandler.Base, Role, ev.Player.Position,
                    ev.Player.Rotation, ev.Player.Nickname, NetworkTime.time);
                Ragdoll.CreateAndSpawn(info);
            }
        }

        private void OnHurting(HurtingEventArgs ev)
        {
            if (Check(ev.Player))
            {
                if (ev.DamageHandler.Type == DamageType.Scp207)
                    ev.Amount = 0.0f;
                else if (ev.DamageHandler.Type == DamageType.Explosion)
                    ev.Amount *= 0.40f;
                else if (ev.DamageHandler.Type == DamageType.Recontainment)
                {
                    ev.Amount = 0;
                    ev.IsAllowed = false;
                }
                else if (ev.DamageHandler.Type != DamageType.Warhead)
                    ev.Amount *= 0.70f;
            }
        }

        private void OnTeleporting(TeleportingEventArgs ev)
        {
            if (Check(ev.Player))
                ev.IsAllowed = false;
        }

        private void OnExplodingGrenade(ExplodingGrenadeEventArgs ev)
        {
            if (ev.Projectile.ProjectileType == ProjectileType.Flashbang)
            {
                foreach (Player player in TrackedPlayers)
                {
                    float dist = Vector3.Distance(ev.Projectile.Transform.position, player.Position);
                    if (dist <= 20f)
                    {
                        if (Physics.Linecast(ev.Projectile.Transform.position, player.Position))
                            return;
                        float damage = FlashbangBaseDamage - (dist * FlashbangFalloffMultiplier);
                        if (damage < 0)
                            damage = 0f;
                        else if (damage > FlashbangBaseDamage)
                            damage = FlashbangBaseDamage;

                        Log.Debug($"{nameof(OnExplodingGrenade)}: Damage: {damage} - {dist} {player.Nickname}");
                        player.Hurt(new ExplosionDamageHandler(ev.Player?.Footprint ?? new Footprint(Server.Host.ReferenceHub), Vector3.zero, damage, 0));
                        DoFlashEffect(player, dist);
                    }
                }
            }
        }

        private void DoFlashEffect(Player player, float distance)
        {
            if (!ConsumptionStacks.ContainsKey(player))
                ConsumptionStacks.Add(player, 1);

            if (ResetConsumptionOnFlashed)
            {
                if (ConsumptionStacks[player] > 0)
                {
                    if (distance < 6 && ConsumptionStacks[player] > 1)
                        ConsumptionStacks[player]--;
                    else if (distance > 15)
                        ConsumptionStacks[player]++;

                    if ((ConsumptionStacks[player] / 2) >= 3)
                        ConsumptionStacks[player] -= 3;
                    else if ((ConsumptionStacks[player] / 2) >= 2)
                        ConsumptionStacks[player] -= 2;
                    else
                        ConsumptionStacks[player]--;
                }

                player.ShowHint($"You now have {ConsumptionStacks[player]} stacks of Consumption!", 10);
                int newIntensity = ConsumptionStacks[player] / (MaxConsumption / 2);
                player.ChangeEffectIntensity(EffectType.Scp207, (byte)newIntensity);
            }

            if (TeleportOnFlashed)
            {
                player.Position = Vector3.down * 1950f;

                Timing.CallDelayed(15f, () =>
                {
                    foreach (Player ply in Player.Get(Team.SCPs))
                    {
                        if (player == ply)
                            continue;
                        player.Position = ply.Position + Vector3.up;
                        break;
                    }

                    Door hczArmoryDoor = Door.Get("HCZ_ARMORY");
                    Transform transform = hczArmoryDoor.Base.transform;
                    player.Position = transform.position + transform.forward * 2f;
                });
            }
        }

        private void OnEnteringPocketDimension(EnteringPocketDimensionEventArgs ev)
        {
            if (!ConsumptionStacks.ContainsKey(ev.Scp106))
                ConsumptionStacks.Add(ev.Scp106, 1);

            if (Check(ev.Scp106))
            {
                ev.IsAllowed = false;
                ev.Player.Hurt(new ScpDamageHandler(ev.Scp106.ReferenceHub, Mathf.Max(BaseDamage, BaseDamage * (ConsumptionStacks[ev.Scp106] / DamageScalar)), DeathTranslations.PocketDecay));
            }
        }

        public void IncreasePower(Player player)
        {
            if (!ConsumptionStacks.ContainsKey(player))
                ConsumptionStacks.Add(player, 1);

            if (ConsumptionStacks[player] >= MaxConsumption)
                return;

            Log.Debug($"{Name} power increase for {player.Nickname}");
            ConsumptionStacks[player]++;

            int newIntensity = ConsumptionStacks[player] / (MaxConsumption / 2);
            player.ChangeEffectIntensity(EffectType.Scp207, (byte)newIntensity);
            player.ShowHint($"You now have {ConsumptionStacks[player]} stacks of Consumption!");
        }

        private IEnumerator<float> Invisibility(FpcRole? role)
        {
            Log.Debug($"{nameof(Scp575)}: {nameof(Invisibility)}: Starting 268 loop for {role?.Owner.Nickname}");
            for (;;)
            {
                if (role is null)
                {
                    Log.Error($"SCP-575 FPC role is null.");
                    break;
                }

                foreach (Player ply in Player.List)
                {
                    VisionInformation info = VisionInformation.GetVisionInformation(ply.ReferenceHub, ply.CameraTransform, role.Owner.Position, 70f, 60f);
                    if (info.IsLooking && !info.IsInDarkness && info.IsInLineOfSight && role.IsInvisibleFor.Contains(ply))
                    {
                        role.IsInvisibleFor.Remove(ply);
                        continue;
                    }

                    if (!role.IsInvisibleFor.Contains(ply))
                        role.IsInvisibleFor.Add(ply);
                }

                if (!role.Owner.CurrentRoom.AreLightsOff)
                    role.Owner.CurrentRoom.TurnOffLights(10f);

                yield return Timing.WaitForSeconds(0.25f);
            }
        }
    }
}