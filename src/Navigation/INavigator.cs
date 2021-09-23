using Eleon.Modding;

namespace GalacticWaez.Navigation
{
    public delegate void NavigatorCallback(string message);

    public interface INavigator
    {
        void HandlePathRequest(string request, IPlayer player, NavigatorCallback doneCallback);
    }
}