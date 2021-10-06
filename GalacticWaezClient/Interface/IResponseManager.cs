using Eleon;

namespace GalacticWaez
{
    public interface IResponseManager
    {
        IResponder CreateResponder(MessageData msg);
    }
}