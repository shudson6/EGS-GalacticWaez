using System.Collections.Generic;

namespace GalacticWaez.Navigation
{
    public delegate void NavigatorCallback(IEnumerable<LYCoordinates> path);

    public interface INavigator
    {
        void HandlePathRequest(string request, 
            IPlayerTracker player, 
            PathfinderDelegate findPath,
            NavigatorCallback doneCallback
            );
    }
}