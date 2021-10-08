using System;

namespace GalacticWaez
{
    public class HelpHandler : ICommandHandler
    {
        public const string HelpText = "Waez commands:\n"
            + "bookmarks [clear|hide|show]: remove Waez_ bookmarks or hide/show them in HUD (requires exit/resume)\n"
            + "clear: remove all map markers that start with Waez_\n"
            + "help: get this help message. provide a command to get specific help.\n"
            + "pinfo: find out what Waez knows about you\n"
            + "restart: reinitialize Waez (for troubleshooting)\n"
            + "status: find out what Waez is up to\n"
            + "store: write star data to save file (for troubleshooting)\n"
            + "to [mapmarker]: plot a course to [mapmarker] and add mapmarkers for each step\n";

        public const string ToHelp = "/waez to [options] [mapmarker]\n"
            + "Plots a course from your current location to [mapmarker] and adds waypoints to the Galaxy Map.\n"
            + "[mapmarker] is the name either of a mapmarker on the Galaxy Map or of a known (visited) star. "
            + "Map markers are searched first.\n"
            + "[options]:\n"
            + "--range=[#] - Use [#] as the jump range when pathfinding. Unit is LY.\n"
            + "Options must come before [mapmarker].";

        public const string StatusHelp = "/waez status\n"
            + "Responds with Waez's current status. Navigation is only possible when status is 'Ready.'";

        public const string ClearHelp = "/waez clear\n"
            + "Removes all 'Waez_#' map markers belonging to the player.";

        public const string BookmarksHelp = "/waez bookmarks [clear|hide|show]\n"
            + "clear: Removes all 'Waez_#' map markers belonging to the player.\n"
            + "hide: Unchecks 'Show On HUD' for all 'Waez_#' map markers belonging to the player.\n"
            + "show: Checks 'Show On HUD' for all 'Waez_#' map markers belonging to the player.\n"
            + "Exit/Resume is required for these to take effect.";

        public const string HelpHelp = "/waez help [command]\n"
            + "Displays helpful information. If [command] is provided, displays specific information.";

        public const string PinfoHelp = "/waez pinfo [playerId]\n"
            + "get info about the player with id [playerId]\n"
            + "if [playerId] is not given, get the local player";

        public const string RestartHelp = "/waez restart [source]\n"
            + "Reacquire star position data and rebuild Waez's Galaxy Map.\n"
            + "[source] must be provided from these options:\n"
            + "file-only - Load data from savegame (if present). No memory scan.\n"
            + "scan-only - Search memory for star data.\n"
            + "normal - Normal init, checks savegame for star data and scans memory if not found.\n";

        public const string StoreHelp = "/waez store [--replace]\n"
            + "Store star position data in the savegame. Under normal operation, this is done automatically.\n"
            + "Use --replace to force overwrite of existing data.";

        public bool HandleCommand(string cmdToken, string args, IPlayerInfo player, IResponder responder)
        {
            if (cmdToken != "help")
                return false;

            string message;
            switch (args?.Trim())
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

                case "pinfo":
                    message = PinfoHelp;
                    break;

                case "restart":
                    message = RestartHelp;
                    break;

                case "store":
                    message = StoreHelp;
                    break;

                default:
                    message = HelpText + "\n\nUnrecognized option: " + args;
                    break;
            }
            responder.Send(message);
            return true;
        }
    }
}
