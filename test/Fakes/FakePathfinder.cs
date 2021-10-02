using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalacticWaez;

namespace GalacticWaezTests.Fakes
{
    public class FakePathfinder : IPathfinder
    {
        public IEnumerable<LYCoordinates> FindPath(GalaxyMap.Node start, GalaxyMap.Node goal, float warpRange)
        {
            throw new NotImplementedException();
        }
    }
}
