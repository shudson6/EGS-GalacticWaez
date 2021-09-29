using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Eleon.Modding;
using GalacticWaez;

namespace GalacticWaezTests.Fakes
{
    class FakeDataSource : IGalaxyDataSource
    {
        private readonly string filename;

        public FakeDataSource(string filename) { this.filename = filename; }


        public IEnumerable<VectorInt3> GetGalaxyData()
        {
            return GalaxyTestData.LoadPositions(filename);
        }
    }

    class NotFoundDataSource : IGalaxyDataSource
    {
        public IEnumerable<VectorInt3> GetGalaxyData() => null;
    }

    class EmptyCollectionDataSource : IGalaxyDataSource
    {
        public IEnumerable<VectorInt3> GetGalaxyData() => new List<VectorInt3>();
    }
}
