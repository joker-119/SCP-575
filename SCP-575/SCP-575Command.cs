using Smod2.Commands;
using MEC;

namespace SCP575
{
    public class SCP575Command : ICommandHandler
    {
        private readonly SCP575 plugin;
        public SCP575Command(SCP575 plugin) => this.plugin = plugin;

        public string GetCommandDescription()
        {
            return "";
        }

        public string GetUsage()
        {
            return "SCP575 Commands \n" +
            "[SCP575 / 575] HELP \n" +
            "SCP575 TOGGLE \n" +
            "SCP575 ENABLE \n" +
            "SCP575 DISABLE \n" +
            "SCP575 ANNOUNCE ON \n" +
            "SCP575 ANNOUNCE OFF \n";
        }

        public string[] OnCall(ICommandSender sender, string[] args)
        {
            if (args.Length > 0)
            {
                if (Functions.singleton.IsAllowed(sender))
                {
                    switch (args[0].ToLower())
                    {
                        case "help":
                            return new string[]
                            {
                                "SCP575 Command List \n"+
                                "SCP575 toggle - Toggles a manual SCP-575 on/off. \n"+
                                "SCP575 enable - Enables timed SCP-575 events. \n"+
                                "SCP575 disable - Disables timed SCP-575 events. \n"+
                                "SCP575 announce on - Enables CASSIE announcements for events. \n"+
                                "SCP575 announce off - Disables CASSIE announcements for events. \n"
                            };
                        case "toggle":
                            {
                                Functions.singleton.ToggleBlackout();

                                return new string[]
                                {
                                "Manual SCP-575 event toggled."
                                };
                            }
                        case "enable":
                            {
                                Functions.singleton.EnableBlackouts();

                                return new string[]
                                {
                                "Timed events enabled."
                                };
                            }
                        case "disable":
                            {
                                Functions.singleton.DisableBlackouts();

                                return new string[]
                                {
                                "Timed events disabled."
                                };
                            }
                        case "announce":
                            {
                                if (args.Length > 1)
                                {
                                    switch (args[1].ToLower())
                                    {
                                        case "on":
                                            {
                                                Functions.singleton.EnableAnnounce();

                                                return new string[] { "Announcements enabled." };
                                            }
                                        case "off":
                                            {
                                                Functions.singleton.DisableAnnounce();

                                                return new string[] { "Announcements disabled." };
                                            }
                                        default:
                                            {
                                                return new string[] { "Invalid argument." };
                                            }
                                    }
                                }
                                return new string[] { "No argument supplied." };
                            }
                        case "halt":
                            {
                                foreach (CoroutineHandle handle in plugin.coroutines) Timing.KillCoroutines(handle);
                                plugin.coroutines.Clear();

                                return new string[] { "Halted all active Coroutines." };
                            }
                        default:
                            {
                                return new string[] { GetUsage() };
                            }
                    }
                }
                else
                    return new string[] { "You are not allowed to use this command." };
            }
            else
            {
                return new string[] { GetUsage() };
            }
        }
    }
}