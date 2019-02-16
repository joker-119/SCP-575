using Smod2.Commands;

namespace SCP575
{
    public class SCP575Command : ICommandHandler
    {
        public string GetCommandDescription()
        {
            return "";
        }

        public string GetUsage()
        {
            return "SCP575 Commands \n"+
            "[SCP575 / 575] HELP \n"+
            "SCP575 TOGGLE \n"+
            "SCP575 ENABLE \n"+
            "SCP575 DISABLE \n"+
            "SCP575 ANNOUNCE ON \n"+
            "SCP575 ANNOUNCE OFF \n";
        }

        public string[] OnCall(ICommandSender sender, string[] args)
        {
            if (args.Length > 0)
            {
                if (args[0].ToLower() == "help")
                {
                    return new string[]
                    {
                        "SCP575 Command List \n"+
                        "SCP575 toggle - Toggles a manual blackout on/off. \n"+
                        "SCP575 enable - Enables timed Lightout events. \n"+
                        "SCP575 disable - Disables timed Lightout events. \n"+
                        "SCP575 announce on - Enables CASSIE announcements for events. \n"+
                        "SCP575 announce off - Disables CASSIE announcements for events. \n"
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