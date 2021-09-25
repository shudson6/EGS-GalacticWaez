using System.Collections.Generic;

namespace GalacticWaez.Navigation
{
    public delegate void NavigatorCallback(IEnumerable<LYCoordinates> path, string message);

    public interface INavigator
    {
        void HandlePathRequest(string request, 
            IPlayerTracker player, 
            Pathfinder findPath,
            NavigatorCallback doneCallback
            );
    }
}