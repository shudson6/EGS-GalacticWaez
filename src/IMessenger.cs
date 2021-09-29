using Eleon;

namespace GalacticWaez
{
    public interface IMessenger
    {
        void SendMessage(string text, MessageData request);
    }
}