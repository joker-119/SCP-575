using Exiled.API.Interfaces;
using Exiled.Loader;

namespace SCP_575
{
    public class Config : IConfig
    {
        public void Reload()
        {
            RandomEvents = PluginManager.YamlConfig.GetBool($"{Prefix}_random_events", true);
            DisableTeslas = PluginManager.YamlConfig.GetBool($"{Prefix}_disable_teslas", true);
            InitialDelay = PluginManager.YamlConfig.GetFloat($"{Prefix}_initial_delay", 300f);
            DurationMin = PluginManager.YamlConfig.GetFloat($"{Prefix}_dur_min", 30f);
            DurationMax = PluginManager.YamlConfig.GetFloat($"{Prefix}_dur_max", 90);
            DelayMin = PluginManager.YamlConfig.GetInt($"{Prefix}_delay_min", 180);
            DelayMax = PluginManager.YamlConfig.GetInt($"{Prefix}_delay_max", 500);
            SpawnChance = PluginManager.YamlConfig.GetInt($"{Prefix}_spawn_chance", 45);
            EnableKeter = PluginManager.YamlConfig.GetBool($"{Prefix}_keter", true);
            OnlyHeavy = PluginManager.YamlConfig.GetBool($"{Prefix}_only_hcz", false);
            Voice = PluginManager.YamlConfig.GetBool($"{Prefix}_voice", true);
            KeterDamage = PluginManager.YamlConfig.GetFloat($"{Prefix}_keter_dmg", 10f);
        }

        public bool RandomEvents { get; private set; }
        public bool DisableTeslas { get; private set; }
        public float InitialDelay { get; private set; }
        public float DurationMin { get; private set; }
        public float DurationMax { get; private set; }
        public int DelayMin { get; private set; }
        public int DelayMax { get; private set; }
        public int SpawnChance { get; private set; }
        public bool EnableKeter { get; private set; }
        public bool OnlyHeavy { get; private set; }
        public bool Voice { get; private set; }
        public float KeterDamage { get; private set; }
        
        public bool IsEnabled { get; set; }
        public string Prefix { get; } = "575";
    }
}