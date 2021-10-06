using System;
using System.Threading.Tasks;
using static GalacticWaez.GalacticWaez;

namespace GalacticWaez
{
    public class NavigationHandler : ICommandHandler
    {
        private readonly INavigator Navigator;

        public NavigationHandler(INavigator nav)
            => Navigator = nav ?? throw new ArgumentNullException("NavigationHandler: nav");

        public bool HandleCommand(string cmdToken, string args, IPlayerInfo player, IResponder responder)
        {
            if (cmdToken != "to")
                return false;

            if (args == null)
                args = "";

            float range = player.WarpRange;

            // check for options
            // once options, if any, are chopped off the front, whatever is left is the destination name
            while (args.StartsWith("--"))
            {
                var temp = args.Split(new[] { ' ' }, 2);
                string option = temp[0];
                args = (temp.Length == 2) ? temp[1] : "";
                if (option.StartsWith("--range="))
                {
                    if (!float.TryParse(option.Substring("--range=".Length), out range))
                    {
                        responder.Send("Invalid range argument.");
                        return true;
                    }
                    if (range <= 0)
                    {
                        responder.Send("Range must be a positive number.");
                        return true;
                    }
                    // user range is interpreted as LY, but sectors needed for Navigator
                    range *= SectorsPerLY;
                }
            }

            // fire-and-forget, let the TaskScheduler handle threading
            Task.Factory.StartNew(() => Navigator.Navigate(player, args, range, responder));
            return true;
        }
    }
}
