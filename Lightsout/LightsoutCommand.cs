using Smod2.Commands;

namespace Lightsout
{
    public class LightsoutCommand : ICommandHandler
    {
        public string GetCommandDescription()
        {
            return "";
        }

        public string GetUsage()
        {
            return "Lightsout Commands \n"+
            "[lightsout / lo] HELP \n"+
            "lightsout TOGGLE \n"+
            "lightsout ENABLE \n"+
            "lightsout DISABLE \n"+
            "lightsout ANNOUNCE ON \n"+
            "lightsout ANNOUNCE OFF \n";
        }

        public string[] OnCall(ICommandSender sender, string[] args)
        {
            if (args.Length > 0)
            {
                if (args[0].ToLower() == "help")
                {
                    return new string[]
                    {
                        "Lightsout Command List \n"+
                        "lightsout toggle - Toggles a manual blackout on/off. \n"+
                        "lightsout enable - Enables timed Lightout events. \n"+
                        "lightsout disable - Disables timed Lightout events. \n"+
                        "lightsout announce on - Enables CASSIE announcements for events. \n"+
                        "lightsout announce off - Disables CASSIE announcements for events. \n"
                    };
                }
                else if (args[0].ToLower() == "toggle")
                {
                    Functions.ToggleBlackout();
                    return new string[] { "Manual Lightout togled." };
                }
                else if (args[0].ToLower() == "enable")
                {
                    Functions.EnableBlackouts();
                    return new string[] { "Timed Lightout events enabled." };
                }
                else if (args[0].ToLower() == "disable")
                {
                    Functions.DisableBlackouts();
                    return new string[] { "Timed Lightout events disabled." };
                }
                else if (args[0].ToLower() == "announce on")
                {
                    Functions.EnableAnnounce();
                    return new string[] { "CASSIE announcements enabled." };
                }
                else if (args[0].ToLower() == "announce off")
                {
                    Functions.DisableAnnounce();
                    return new string[] { "CASSIE announcements disabled." };
                }
                else
                {
                    return new string[] { GetUsage() };
                }
            }
            else
            {
                return new string[] { GetUsage() };
            }
        }
    }
}