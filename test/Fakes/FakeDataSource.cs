using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Eleon.Modding;
using GalacticWaez;

namespace GalacticWaezTests.Fakes
{
    class FakeDataSource : IFileDataSource
    {
        public string PathToFile { get; }

        public FakeDataSource(string filename) { PathToFile = filename; }


        public IEnumerable<VectorInt3> GetGalaxyData()
        {
            return GalaxyTestData.LoadPositions(PathToFile);
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
