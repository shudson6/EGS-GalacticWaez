using Eleon;

namespace GalacticWaez
{
    public interface ICommandHandler
    {
        bool HandleCommand(string cmdToken, string args, IPlayerInfo player, IResponder responder);
    }
}