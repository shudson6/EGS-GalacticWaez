using System;

namespace GalacticWaez
{
    public class HelpHandler : ICommandHandler
    {
        private const string HelpText = "Waez commands:\n"
            + "to [mapmarker]: plot a course to [mapmarker] and add mapmarkers for each step\n"
            + "status: find out what Waez is up to\n"
            + "clear: remove all map markers that start with Waez_\n"
            + "bookmarks [clear|hide|show]: remove Waez_ bookmarks or hide/show them in HUD (requires exit/resume)\n"
            + "help: get this help message. provide a command to get specific help.\n";

        private const string ToHelp = "/waez to [options] [mapmarker]\n"
            + "Plots a course from your current location to [mapmarker] and adds waypoints to the Galaxy Map.\n"
            + "[mapmarker] is the name either of a mapmarker on the Galaxy Map or of a known (visited) star. "
            + "Map markers are searched first.\n"
            + "[options]:\n"
            + "--range=[#] - Use [#] as the jump range when pathfinding. Unit is LY.\n"
            + "Options must come before [mapmarker].";

        private const string StatusHelp = "/waez status\n"
            + "Responds with Waez's current status. Navigation is only possible when status is 'Ready.'";

        private const string ClearHelp = "/waez clear\n"
            + "Removes all 'Waez_#' map markers belonging to the player.";

        private const string BookmarksHelp = "/waez bookmarks [clear|hide|show]\n"
            + "clear: Removes all 'Waez_#' map markers belonging to the player.\n"
            + "hide: Unchecks 'Show On HUD' for all 'Waez_#' map markers belonging to the player.\n"
            + "show: Checks 'Show On HUD' for all 'Waez_#' map markers belonging to the player.\n"
            + "Exit/Resume is required for these to take effect.";

        private const string HelpHelp = "/waez help [command]\n"
            + "Displays helpful information. If [command] is provided, displays specific information.";

        public bool HandleCommand(string cmdToken, string args, IPlayerInfo player, IResponder responder)
        {
            if (cmdToken != "help")
                return false;

            string message;
            switch (args)
            {
                case null:
                case "":
                    message = HelpText;
                    break;

                case "to":
                    message = ToHelp;
                    break;

                case "status":
                    message = StatusHelp;
                    break;

                case "clear":
                    message = ClearHelp;
                    break;

                case "bookmarks":
                    message = BookmarksHelp;
                    break;

                case "help":
                    message = HelpHelp;
                    break;

                default:
                    return false;
            }
            responder.Send(message);
            return true;
        }
    }
}
