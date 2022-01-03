namespace SCP_575
{
	using System;
	using System.Collections.Generic;
	using Exiled.API.Features;
	using MEC;
	using Server = Exiled.Events.Handlers.Server;

	public class Plugin : Plugin<Config>
	{
		public static Plugin Singleton;

		public override string Author { get; } = "Joker119";
		public override string Name { get; } = "SCP-575";
		public override string Prefix { get; } = "575";
		public override Version Version { get; } = new Version(4, 0, 0);
		public override Version RequiredExiledVersion { get; } = new Version(4, 2, 0);

		public EventHandlers EventHandlers { get; private set; }
		public NestingObjects.Npc Npc { get; private set; }
		public NestingObjects.Playable Playable { get; private set; }
		public List<Player> StopRagdollList { get; } = new List<Player>();

		public override void OnEnabled()
		{
			Singleton = this;
			Config.PlayableConfig.Scp575.TryRegister();
				EventHandlers = new EventHandlers(this);
				Npc = new NestingObjects.Npc(this);
				Playable = new NestingObjects.Playable(this);

				Server.WaitingForPlayers += EventHandlers.OnWaitingForPlayers;
				Exiled.Events.Handlers.Player.SpawningRagdoll += EventHandlers.OnSpawningRagdoll;

				base.OnEnabled();
		}

		public override void OnDisabled()
		{
			Config.PlayableConfig.Scp575.TryUnregister();
			foreach (CoroutineHandle handle in EventHandlers.Coroutines)
				Timing.KillCoroutines(handle);
			EventHandlers.Coroutines.Clear();
			Server.WaitingForPlayers -= EventHandlers.OnWaitingForPlayers;
			Exiled.Events.Handlers.Player.SpawningRagdoll -= EventHandlers.OnSpawningRagdoll;

			EventHandlers = null;
			Npc = null;
			Playable = null;
			
			base.OnDisabled();
		}
	}
}