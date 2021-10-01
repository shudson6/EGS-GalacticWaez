using System.Collections.Generic;
using Eleon.Modding;

namespace GalacticWaez.Navigation
{
    public delegate void NavigatorCallback(IEnumerable<LYCoordinates> path, string message);

    public interface INavigator
    { 
        void Navigate(IPlayerInfo player, string destination, float playerRange, IResponder response);
    }
}