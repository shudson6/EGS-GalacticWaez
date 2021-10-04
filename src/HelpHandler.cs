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

        public bool HandleCommand(string cmdToken, string args, IPlayerInfo player, IResponder responder)
        {
            if (cmdToken != "help")
                return false;

            switch (args)
            {
                case null:
                case "":
                    responder.Send(HelpText);
                    return true;

                default:
                    return false;
            }
        }
    }
}
