using System.Collections.Generic;
using Eleon.Modding;
using GalacticWaez;

namespace GalacticWaezTests.Fakes
{
    class FileDataSource : IFileDataSource
    {
        public string PathToFile { get; }

        public FileDataSource(string filename) { PathToFile = filename; }


        public IEnumerable<VectorInt3> GetGalaxyData()
        {
            return GalaxyTestData.LoadPositions(PathToFile);
        }
    }

    class NullDataSource : IGalaxyDataSource
    {
        public IEnumerable<VectorInt3> GetGalaxyData() => null;
    }

    class NullFileDataSource : NullDataSource, IFileDataSource
    {
        public string PathToFile => throw new System.NotImplementedException();
    }

    class EmptyDataSource : IGalaxyDataSource
    {
        public IEnumerable<VectorInt3> GetGalaxyData() => new List<VectorInt3>();
    }

    class EmptyFileDataSource : EmptyDataSource, IFileDataSource
    {
        public string PathToFile => throw new System.NotImplementedException();
    }
}
