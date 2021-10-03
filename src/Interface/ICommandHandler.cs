using Eleon;

namespace GalacticWaez
{
    public interface ICommandHandler
    {
        bool HandleCommand(string commandText, IPlayerInfo player, IResponder responder);
    }
}