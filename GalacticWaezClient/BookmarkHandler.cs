using System;

namespace GalacticWaez
{
    public class BookmarkHandler : ICommandHandler
    {
        private readonly LoggingDelegate Log;
        private readonly IBookmarkManager Bookmarks;

        public BookmarkHandler(IBookmarkManager bmgr, LoggingDelegate log)
        {
            Bookmarks = bmgr ?? throw new ArgumentNullException("BookmarkHandler: bmgr");
            Log = log ?? delegate { };
        }

        public bool HandleCommand(string cmdToken, string args, IPlayerInfo player, IResponder responder)
        {
            if (cmdToken == "clear")
            { 
                if (args == null || args == "")
                {
                    cmdToken = "bookmarks";
                    args = "clear";
                }
                else
                {
                    responder.Send("'clear' does not take options.");
                    return true;
                }
            }

            if (cmdToken != "bookmarks")
                return false;

            if (args == null || args == "")
            {
                responder.Send("Required: [show|hide|clear]");
                return true;
            }

            if (args != "show" && args != "hide" && args != "clear")
            {
                responder.Send("Unrecognized option: " + args
                    + "\nuse [show|hide|clear]");
                return true;
            }

            string message = "Modified "
                + Bookmarks.ModifyPathMarkers(player.Id, args)
                + " map markers.";
            responder?.Send(message);
            if (responder == null)
                Log(message);

            return true;
        }
    }
}
