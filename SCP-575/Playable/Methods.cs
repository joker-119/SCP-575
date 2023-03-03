namespace SCP_575.Playable
{
    using System.Linq;
    using Exiled.Events.Handlers;
    using Exiled.Loader;
    using PlayerRoles;
    using Player = Exiled.API.Features.Player;

    public class Methods
    {
        private readonly Plugin _plugin;
        public Methods(Plugin plugin) => _plugin = plugin;

        public void Init()
        {
            Server.RoundStarted += _plugin.Playable.EventHandlers.OnRoundStarted;
        }

        public void Disable()
        {
            Server.RoundStarted -= _plugin.Playable.EventHandlers.OnRoundStarted;
        }

        public void TrySpawn575()
        {
            if (Loader.Random.Next(100) <= _plugin.Config.PlayableConfig.Scp575.SpawnChance)
            {
                Player? player = Player.Get(RoleTypeId.Scp106).FirstOrDefault();
                if (player is null)
                    return;

                _plugin.Config.PlayableConfig.Scp575.AddRole(player);
                Disable();
            }
        }
    }
}