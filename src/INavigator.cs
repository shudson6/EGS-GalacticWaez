using System.Collections.Generic;
using Eleon.Modding;

namespace GalacticWaez
{
    public delegate void NavigatorCallback(IEnumerable<VectorInt3> path, string message);

    public interface INavigator
    { 
        void Navigate(IPlayerInfo player, string destination, float playerRange, IResponder response);
    }
}