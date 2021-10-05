using System.Collections.Generic;
using Eleon.Modding;

namespace GalacticWaez
{
    public interface IPathfinder
    {
        /// <summary>
        /// Finds a path from start to goal. Returned path includes both start and goal vectors.
        /// Returns null if no path is found.
        /// <br/>
        /// <c>warpRange</c> is required to be positive; if it is <= 0, 
        /// <see cref="System.ArgumentOutOfRangeException"/> is thrown.
        /// </summary>
        /// <param name="start">the node to start at</param>
        /// <param name="goal">the node to end at</param>
        /// <param name="warpRange">the maximum allowed jump distance</param>
        /// <returns>
        /// a path from start to goal, or <c>null</c> if no path found.
        /// </returns>
        IEnumerable<VectorInt3> FindPath(GalaxyMap.Node start, GalaxyMap.Node goal, float warpRange);
    }
}