using System.Collections.Generic;
using System.Threading;
using GalacticWaez;
using Eleon.Modding;

namespace GalacticWaezTests.Fakes
{
    public class FakePathfinder : IPathfinder
    {
        private readonly IEnumerable<VectorInt3> path;

        public FakePathfinder(IEnumerable<VectorInt3> path = null) => this.path = path;

        public IEnumerable<VectorInt3> FindPath(IGalaxyNode start, IGalaxyNode goal, float warpRange,
            CancellationToken token = default) => path;
    }

    /// <summary>
    /// When FindPath is called, blocks until token is canceled.
    /// _ALWAYS_ use a TimeoutAttribute on tests that use this.
    /// </summary>
    public class TimeoutPathfinder : IPathfinder
    {
        public IEnumerable<VectorInt3> FindPath(IGalaxyNode start, IGalaxyNode goal, float warpRange, 
            CancellationToken token = default)
        {
            while (!token.IsCancellationRequested) ;
            token.ThrowIfCancellationRequested();
            return null;
        }
    }
}
