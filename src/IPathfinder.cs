using System.Collections.Generic;

namespace GalacticWaez
{
    public interface IPathfinder
    {
        IEnumerable<LYCoordinates> FindPath(GalaxyMap.Node start, GalaxyMap.Node goal, float warpRange);
    }
}