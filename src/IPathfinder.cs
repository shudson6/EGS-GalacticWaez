using System.Collections.Generic;
using Eleon.Modding;

namespace GalacticWaez
{
    public interface IPathfinder
    {
        IEnumerable<VectorInt3> FindPath(GalaxyMap.Node start, GalaxyMap.Node goal, float warpRange);
    }
}