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
            if (cmdToken == "clear" && (args == null || args == ""))
            {
                cmdToken = "bookmarks";
                args = "clear";
            }

            if (cmdToken != "bookmarks")
                return false;

            if (args != "show" && args != "hide" && args != "clear")
                return false;

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
