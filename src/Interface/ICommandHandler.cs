using Eleon;

namespace GalacticWaez
{
    public interface ICommandHandler
    {
        void HandleChatCommand(MessageData messageData);
    }
}