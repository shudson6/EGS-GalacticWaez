using System.Collections.Generic;
using System.Threading;
using Eleon.Modding;

namespace GalacticWaez
{
    public interface IPathfinder
    {
        /// <summary>
        /// Find a path from start to goal, where each jump is no longer than
        /// <c>warpRange</c>. Values of <c>warpRange</c> larger
        /// than the distance used to build the Galaxy have no effect.
        /// <br/>The returned path includes the start and goal vectors.
        /// </summary>
        /// <param name="start">starting point, not <c>null</c></param>
        /// <param name="goal">destination point, not <c>null</c></param>
        /// <param name="warpRange">maximum jump distance. must be greater than zero.</param>
        /// <returns>
        /// an ordered collection of vectors representing the path,
        /// or <c>null</c> if no path found
        /// </returns>
        IEnumerable<VectorInt3> FindPath(IGalaxyNode start, IGalaxyNode goal, float warpRange,
            CancellationToken token = default);
    }
}