using System.Collections.Generic;

namespace GalacticWaez.Navigation
{
    public interface IPathfinder
    {
        IEnumerable<LYCoordinates> FindPath(Galaxy.Node start, Galaxy.Node goal, float warpRange);
    }
}