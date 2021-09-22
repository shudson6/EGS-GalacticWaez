using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using GalacticWaez;
using SectorCoordinates = Eleon.Modding.VectorInt3;
using Eleon.Modding;

namespace GalacticWaezTests
{
    [TestClass]
    public class StarFinderTests
    {
        private static IEnumerable<VectorInt3> positions;

        [ClassInitialize]
        [DeploymentItem("Dependencies\\stardata-test-large.csv")]
        public static void SetupClass(TestContext _tc)
        {
            positions = GalaxyDataPrep.LoadPositions(_tc.DeploymentDirectory + "\\stardata-test-large.csv");
        }

        [TestMethod]
        public void DoesItWork()
        {
            var loaded = new List<StarFinder.StarData>(positions.Count());
            int i = 0;
            foreach (var p in positions)
            {
                loaded.Add(new StarFinder.StarData(0, -1, p.x, p.y, p.z, i));
                i++;
            }
            var found = new StarFinder().Search(new SectorCoordinates(
                loaded[0].x, loaded[0].y, loaded[0].z));
            i = 0;
            foreach (var p in positions)
            {
                Assert.AreEqual(p, found[i]);
                i++;
            }
        }
    }
}
