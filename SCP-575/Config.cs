using Exiled.API.Interfaces;
using Exiled.Loader;

namespace SCP_575
{
    public class Config : IConfig
    {
        public bool RandomEvents { get; private set; } = true;
        public bool DisableTeslas { get; private set; } = true;
        public float InitialDelay { get; private set; } = 300f;
        public float DurationMin { get; private set; } = 30f;
        public float DurationMax { get; private set; } = 90f;
        public int DelayMin { get; private set; } = 180;
        public int DelayMax { get; private set; } = 500;
        public int SpawnChance { get; private set; } = 45;
        public bool EnableKeter { get; private set; } = true;
        public bool OnlyHeavy { get; private set; } = false;
        public bool Voice { get; private set; } = true;
        public float KeterDamage { get; private set; } = 10f;
        
        public bool IsEnabled { get; set; }
        public string Prefix { get; } = "575";
    }
}