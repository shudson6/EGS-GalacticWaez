using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalacticWaez;
using Eleon.Modding;

namespace GalacticWaezTests.Fakes
{
    public class FakePathfinder : IPathfinder
    {
        private readonly IEnumerable<VectorInt3> path;

        public FakePathfinder(IEnumerable<VectorInt3> path = null) => this.path = path;

        public IEnumerable<VectorInt3> FindPath(GalaxyMap.Node start, GalaxyMap.Node goal, float warpRange)
            => path;
    }
}
