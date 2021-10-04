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
            + "help: get this help message\n";

        public bool HandleCommand(string commandText, IPlayerInfo player, IResponder responder)
        {
            if (commandText == null)
                return false;

            var tokens = commandText.Split(new[] { ' ' }, 2, StringSplitOptions.RemoveEmptyEntries);
            if (tokens[0] != "help")
                return false;

            if (tokens.Length == 1)
            {
                responder.Send(HelpText);
            }
            else
            {
                responder.Send("Unrecognized Command: " + commandText);
            }
            return true;
        }
    }
}
