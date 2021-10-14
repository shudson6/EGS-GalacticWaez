using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System;
using System.Threading;
using System.Threading.Tasks;
using GalacticWaez;
using static GalacticWaez.GalacticWaez;

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
            const float Range = 30 * SectorsPerLY;
            const string filename = "stardata-test-small.csv";
            VerifyGalaxyMap(filename,
                new GalaxyMapBuilder((_) => { })
                    .BuildGalaxyMap(new Fakes.FileDataSource(filename), Range),
                Range);
        }

        [TestMethod]
        public void LargeGalaxyMap_HasAllExpectedWarpLines()
        {
            const float Range = 30 * SectorsPerLY;
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
                .BuildGalaxyMap(new Fakes.NullDataSource(), 30 * SectorsPerLY);
            Assert.IsNull(shouldBeNull);
        }

        [TestMethod]
        public void DataEmpty_ReturnsNull()
        {
            var shouldBeNull = new GalaxyMapBuilder((_) => { })
                .BuildGalaxyMap(new Fakes.EmptyDataSource(), 30 * SectorsPerLY);
            Assert.IsNull(shouldBeNull);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Throws_WhenSourceNull()
        {
            new GalaxyMapBuilder(delegate { }).BuildGalaxyMap(null, 30 * SectorsPerLY);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void Throws_WhenWarpNegative()
        {
            new GalaxyMapBuilder(delegate { }).BuildGalaxyMap(new Fakes.EmptyDataSource(), -1);
        }

        [TestMethod]
        public void NoThrow_WhenLoggerNull()
        {
            Assert.IsNotNull(new GalaxyMapBuilder(null));
        }

        [TestMethod]
        public void BuildGalaxyMap_CanBeCanceled()
        {

            const float Range = 30 * SectorsPerLY;
            const string filename = "stardata-test-vanilla.csv";
            var gmb = new GalaxyMapBuilder(delegate { });
            var source = new CancellationTokenSource();
            var token = source.Token;
            var task = Task.Factory.StartNew(() => gmb.BuildGalaxyMap(
                new Fakes.FileDataSource(filename), Range, token), token);
            while (task.Status < TaskStatus.Running) ;
            source.Cancel();
            try
            {
                task.Wait();
            }
            catch (AggregateException ex)
            {
                bool good = false;
                foreach (var e in ex.InnerExceptions)
                    if (e is TaskCanceledException)
                        good = true;
                Assert.IsTrue(good);
                return;
            }
            throw new AssertFailedException();
        }

        private void VerifyGalaxyMap(string filename, IGalaxyMap testGalaxy, float testRange)
        {
            var positions = GalaxyTestData.LoadPositions(filename);
            Assert.IsTrue(testGalaxy.WarpLines > 0);
            Assert.AreEqual(positions.Count, testGalaxy.Stars);
            int i = 0;
            int warpLines = 0;
            foreach (var n in testGalaxy.Nodes)
            {
                Assert.IsTrue(positions.Contains(n.Position));
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
