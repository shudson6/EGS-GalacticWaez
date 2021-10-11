using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using GalacticWaez;
using Eleon.Modding;

namespace GalacticWaezTests
{
    [TestClass]
    [DeploymentItem("Dependencies\\stardata-test-vanilla.csv")]
    public class StarFinderTests
    {
        private static string baseDir;

        [ClassInitialize]
        public static void SetupClass(TestContext _tc)
        {
            baseDir = _tc.DeploymentDirectory;
        }

        [TestMethod]
        public void FindBigData()
        {
            var positions = GalaxyTestData.LoadPositions(
                baseDir + "\\stardata-test-vanilla.csv");
            var loaded = new List<StarFinder.StarData>(positions.Count);
            int i = 0;
            foreach (var p in positions)
            {
                loaded.Add(new StarFinder.StarData(0, -1, p.x, p.y, p.z, i));
                i++;
            }
            var found = new StarFinder().Search(new VectorInt3(
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
