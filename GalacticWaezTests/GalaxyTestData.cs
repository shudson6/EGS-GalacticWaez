using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using Eleon.Modding;
using GalacticWaez;

namespace GalacticWaezTests
{
    public class GalaxyTestData
    {
        public const string LoremIpsum = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, "
            + "sed do eiusmod tempor incididunt ut labore et dolore magna aliqua.Ut enim ad minim veniam, "
            + "quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. "
            + "Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat "
            + "nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia "
            + "deserunt mollit anim id est laborum";

        public static List<VectorInt3> LoadPositions(string starDataFile)
        {
            var reader = new StreamReader(new FileStream(
                starDataFile, FileMode.Open, FileAccess.Read
                ));
            var data = new List<VectorInt3>(int.Parse(reader.ReadLine()));
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                var coords = line.Split(',');
                data.Add(new VectorInt3(
                    int.Parse(coords[0]),
                    int.Parse(coords[1]),
                    int.Parse(coords[2])
                    ));
            }
            return data;
        }

        public static List<StarFinder.StarData> LoadGameStyleStarArray(string starDataFile)
        {
            var pos = LoadPositions(starDataFile);
            int i = 0;
            var data = new List<StarFinder.StarData>(pos.Count);
            foreach (var p in pos)
            {
                data.Add(new StarFinder.StarData(0, -1, p.x, p.y, p.z, i));
                i++;
            }
            return data;
        }

        public static GalaxyMap BuildTestGalaxy(string starDataFile, float range)
            => new GalaxyMapBuilder(null).BuildGalaxyMap(new TestGalaxySource(starDataFile), range);

        private class TestGalaxySource : IGalaxyDataSource
        {
            private readonly string file;
            public TestGalaxySource(string starDataFile) => file = starDataFile;

            public IEnumerable<VectorInt3> GetGalaxyData() => LoadPositions(file);
        }
    }
}
