namespace SCP_575.Playable
{
    using System.Collections.Generic;
    using System.ComponentModel;
    using Exiled.API.Enums;
    using Exiled.API.Features;
    using Exiled.API.Features.Items;
    using Exiled.CustomRoles.API.Features;
    using InventorySystem.Items.Firearms;
    using InventorySystem.Items.Firearms.BasicMessages;
    using MEC;
    using Firearm = Exiled.API.Features.Items.Firearm;

    public class BlackoutAbility : ActiveAbility
    {
        private readonly List<CoroutineHandle> _coroutines = new List<CoroutineHandle>();
        public override string Name { get; set; } = "Blackout";

        public override string Description { get; set; } =
            "Causes all rooms in the facility to lose lighting. Damages players caught in the dark.";

        public override float Duration { get; set; } = 120f;

        public override float Cooldown { get; set; } = 180f;

        [Description("How much damage human players will take, at 5-second intervals, while in a blackedout room without a light.")]
        public float KeterDamage { get; set; } = 5;

        [Description("The message sent to players who are damaged by the blackout.")]
        public string KeterHint { get; set; } = "You have been damaged by SCP-575!";

        protected override void AbilityUsed(Player player)
        {
            DoBlackout(player);
        }

        protected override void AbilityRemoved(Player player)
        {
            foreach (var handle in _coroutines)
                Timing.KillCoroutines(handle);
            _coroutines.Clear();
        }

        private void DoBlackout(Player player)
        {
            Cassie.Message("pitch_0.15 .g7");
            foreach (var room in Map.Rooms)
                if (room.Zone != ZoneType.Surface)
                    room.TurnOffLights(Duration);

            _coroutines.Add(Timing.RunCoroutine(Keter(player, Duration)));
        }

        private IEnumerator<float> Keter(Player ply, float dur)
        {
            do
            {
                foreach (var player in Player.List)
                    if (player.CurrentRoom.LightsOff && !HasLightSource(player) && player.IsHuman)
                    {
                        player.Hurt(DamageType.Bleeding.ToString(), KeterDamage);
                        player.ShowHint(KeterHint, 5f);
                    }

                yield return Timing.WaitForSeconds(5f);
            } while ((dur -= 5f) > 5f);
        }

        private static bool HasLightSource(Player player)
        {
            return player.CurrentItem is Flashlight flashlight && flashlight.Active ||
                   player.HasFlashlightModuleEnabled;
        }
    }
}