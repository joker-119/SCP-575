namespace SCP_575.ConfigObjects
{
    using System.ComponentModel;
    using Exiled.API.Features;

    public class NpcConfig
    {
        [Description("Whether or not randomly timed events should occur. If false, all events will be at the same interval apart.")]
        public bool RandomEvents { get; private set; } = true;

        [Description("Whether or not tesla gates should be disabled during blackouts.")]
        public bool DisableTeslas { get; private set; } = true;

        [Description("Whether or not nuke should be disabled during blackouts.")]
        public bool DisableNuke { get; private set; } = true;

        [Description("The delay before the first event of each round, in seconds.")]
        public float InitialDelay { get; private set; } = 300f;

        [Description("The minimum number of seconds a blackout event can last.")]
        public float DurationMin { get; private set; } = 30f;

        [Description("The maximum number of seconds a blackout event can last. If RandomEvents is disabled, this will be the duration for every event.")]
        public float DurationMax { get; private set; } = 90f;

        [Description("The minimum amount of seconds between each event.")]
        public int DelayMin { get; private set; } = 180;

        [Description("The maximum amount of seconds between each event. If RandomEvents is disabled, this will be the delay between every event.")]
        public int DelayMax { get; private set; } = 500;

        [Description("The percentage change that SCP-575 events will occur in any particular round.")]
        public int SpawnChance { get; private set; } = 45;

        [Description("Whether or not people in dark rooms should take damage if they have no light source in their hand.")]
        public bool EnableKeter { get; private set; } = true;

        [Description("Broadcast shown when a player is damaged by SCP-575.")]
        public Broadcast KeterBroadcast { get; set; } = new Broadcast("You were damaged by SCP-575! Equip a flashlight!", 5);

        [Description("Whether or not SCP-575's \"roar\" should happen after a blackout starts.")]
        public bool Voice { get; private set; } = true;

        [Description("Base damage per delay inflicted if EnableKeter is set to true.")]
        public float KeterDamage { get; private set; } = 10f;

        [Description("The delay of receiving damage.")]
        public float KeterDamageDelay { get; private set; } = 8f;

        [Description("Name displayed in player's death information.")]
        public string KilledBy { get; set; } = "SCP-575";

        // MESSAGES
        [Description("Message said by Cassie if no blackout occurs")]
        public string CassieMessageWrong { get; set; } = ". I have prevented the system failure . .g5 Sorry for a .g3 . false alert .";

        [Description("Message said by Cassie when a blackout starts - 3 . 2 . 1 announcement")]
        public string CassieMessageStart { get; set; } = "facility power system outage in 3 . 2 . 1 .";

        [Description("The time between the sentence and the 3 . 2 . 1 announcement")]
        public float TimeBetweenSentenceAndStart { get; set; } = 8.6f;

        [Description("Message said by Cassie just after the blackout.")]
        public string CassiePostMessage { get; set; } = "facility power system malfunction has been detected at .";

        [Description("Message said by Cassie after CassiePostMessage if outage gonna occure at whole site.")]
        public string CassieMessageFacility { get; set; } = "The Facility .";

        [Description("Message said by Cassie after CassiePostMessage if outage gonna occure at the Entrance Zone.")]
        public string CassieMessageEntrance { get; set; } = "The Entrance Zone .";

        [Description("Message said by Cassie after CassiePostMessage if outage gonna occure at the Light Containment Zone.")]
        public string CassieMessageLight { get; set; } = "The Light Containment Zone .";

        [Description("Message said by Cassie after CassiePostMessage if outage gonna occure at the Heavy Containment Zone.")]
        public string CassieMessageHeavy { get; set; } = "The Heavy Containment Zone.";

        [Description("Message said by Cassie after CassiePostMessage if outage gonna occure at the entrance zone.")]
        public string CassieMessageSurface { get; set; } = "The Surface .";

        [Description("Message said by Cassie after CassiePostMessage if outage gonna occure at random rooms in facility when UseRoomChances is true or unknown type of zones or unspecified zones.")]
        public string CassieMessageOther { get; set; } = ". pitch_0.35 .g6 pitch_0.95 the malfunction is Unspecified .";

        [Description("The sound CASSIE will make during a blackout.")]
        public string CassieKeter { get; set; } = "pitch_0.15 .g7";

        [Description("The message CASSIE will say when a blackout ends.")]
        public string CassieMessageEnd { get; set; } = "facility power system now operational";

        // Probability 
        [Description("A blackout in the whole facility will occur if none of the zones are selected randomly and EnableFacilityBlackout is set to true.")]
        public bool EnableFacilityBlackout { get; private set; } = true;

        [Description("Percentage chance of an outage at the Heavy Containment Zone during the blackout.")]
        public int ChanceHeavy { get; set; } = 99;

        [Description("Percentage chance of an outage at the Heavy Containment Zone during the blackout.")]
        public int ChanceLight { get; set; } = 45;

        [Description("Percentage chance of an outage at the Entrance Zone during the blackout.")]
        public int ChanceEntrance { get; set; } = 65;

        [Description("Percentage chance of an outage at the Surface Zone during the blackout.")]
        public int ChanceSurface { get; set; } = 25;

        [Description("Percentage chance of an outage at an unknown type of zone during the blackout.")]
        public int ChanceOther { get; set; } = 0;

        [Description("Percentage chance of an outage at an unspecified zone during the blackout.")]
        public int ChanceUnspecified { get; set; } = 0;

        [Description("Change this to true if want to use per room probability settings isntead of per zone settings. The script will check all rooms in the specified zone with its probability.")]
        public bool UsePerRoomChances { get; set; } = false;


    }
}