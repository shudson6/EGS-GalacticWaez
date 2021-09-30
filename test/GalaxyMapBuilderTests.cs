using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System;
using GalacticWaez;

namespace GalacticWaezTests
{
    [TestClass]
    [DeploymentItem("Dependencies\\stardata-test-vanilla.csv")]
    [DeploymentItem("Dependencies\\stardata-test-small.csv")]

    public class GalaxyMapBuilderTests
    {
        [TestMethod]
        public void SmallGalaxyMap_HasAllExpectedWarpLines()
        {
            const float Range = 30;
            const string filename = "stardata-test-small.csv";
            VerifyGalaxyMap(filename,
                new GalaxyMapBuilder((_) => { })
                    .BuildGalaxyMap(new Fakes.FileDataSource(filename), Range),
                Range);
        }

        [TestMethod]
        public void LargeGalaxyMap_HasAllExpectedWarpLines()
        {
            const float Range = 30;
            const string filename = "stardata-test-vanilla.csv";
            VerifyGalaxyMap(filename,
                new GalaxyMapBuilder((_) => { })
                    .BuildGalaxyMap(new Fakes.FileDataSource(filename), Range),
                Range);
        }

        [TestMethod]
        public void DataNotFound_ReturnsNull()
        {
            var shouldBeNull = new GalaxyMapBuilder((_) => { })
                .BuildGalaxyMap(new Fakes.NullDataSource(), 30);
            Assert.IsNull(shouldBeNull);
        }

        [TestMethod]
        public void DataEmpty_ReturnsNull()
        {
            var shouldBeNull = new GalaxyMapBuilder((_) => { })
                .BuildGalaxyMap(new Fakes.EmptyDataSource(), 30);
            Assert.IsNull(shouldBeNull);
        }

        private void VerifyGalaxyMap(string filename, GalaxyMap testGalaxy, float testRange)
        {
            var positions = GalaxyTestData.LoadPositions(filename);
            Assert.AreEqual(positions.Count, testGalaxy.Stars);
            int i = 0;
            int warpLines = 0;
            foreach (var n in testGalaxy.Nodes)
            {
                Assert.IsTrue(positions.Contains(n.Position.ToSectorCoordinates()));
                foreach (var p in testGalaxy.Nodes.Take(i))
                {
                    float dist = Distance(n, p);
                    if (dist <= testRange)
                    {
                        // lines are directional; checking 2 at a time
                        warpLines += 2;
                        Assert.IsTrue(p.Neighbors.TryGetValue(n, out float test));
                        Assert.AreEqual(dist, test);
                        Assert.IsTrue(n.Neighbors.TryGetValue(p, out test));
                        Assert.AreEqual(dist, test);
                    }
                    else
                    {
                        Assert.IsFalse(p.Neighbors.ContainsKey(n));
                        Assert.IsFalse(n.Neighbors.ContainsKey(p));
                    }
                }
                i++;
            }
            Assert.AreEqual(warpLines, testGalaxy.WarpLines);
        }

        private float Distance(GalaxyMap.Node a, GalaxyMap.Node b)
        {
            float dx = a.Position.x - b.Position.x;
            float dy = a.Position.y - b.Position.y;
            float dz = a.Position.z - b.Position.z;
            return (float)Math.Sqrt(dx * dx + dy * dy + dz * dz);
        }
    }
}
