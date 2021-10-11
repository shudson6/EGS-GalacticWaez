namespace GalacticWaez
{
    public class PreInitCommandHandler : ICommandHandler
    {
        public bool HandleCommand(string cmdToken, string args, IPlayerInfo player, IResponder responder)
        {
            if (cmdToken == "to" || cmdToken == "clear" || cmdToken == "bookmarks")
            {
                responder.Send("Please wait until Waez is ready.");
                return true;
            }
            return false;
        }
    }
}
