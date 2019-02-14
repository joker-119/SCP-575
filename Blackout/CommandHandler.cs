using System.Linq;
using scp4aiur;
using Smod2;
using Smod2.API;
using Smod2.Commands;

namespace Blackout
{
    public class CommandHandler : ICommandHandler
    {
        public string[] OnCall(ICommandSender sender, string[] args)
        {
            bool valid = sender is Server;

            Player player = sender as Player;
            if (!valid && player != null)
            {
                valid = Plugin.validRanks.Contains(player.GetRankName());
            }

            if (valid)
            {
                if (args[0].ToLower() == "disable")
                {
                    Plugin.DisableBlackouts();
                    return new string[] { "Timed Blackouts disabled!" };
                }
                else if (args[0].ToLower() == "enable")
                {
                    Plugin.EnableBlackouts();
                    return new string[] { "Timed Blackouts enabled!" };
                }
                else if (args[0].ToLower() == "announce off")
                {
                    Plugin.DisableAnnouncements();
                    return new string[] { "Announcements for the lights are now disabled." };
                }
                else if (args[0].ToLower() == "announce on")
                {
                    Plugin.EnableAnnouncements();
                    return new string[] { "Announcements for the lights are now enabled." };
                }
                else if (args[0].ToLower() == "toggle")
                {
                    Plugin.ToggleBlackout();
                }
            }
            return new[]
                { GetUsage() };
        }
        public string GetUsage()
        {
            return "blackout";
        }

        public string GetCommandDescription()
        {
            return "Causes all the lights to flicker.";
        }
    }
}
