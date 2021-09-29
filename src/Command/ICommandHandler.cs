using Eleon;

namespace GalacticWaez.Command
{
    public interface ICommandHandler
    {
        void HandleChatCommand(MessageData messageData);
    }
}