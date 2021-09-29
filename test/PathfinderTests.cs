using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using GalacticWaez;
using GalacticWaez.Navigation;
using Eleon.Modding;

namespace GalacticWaezTests
{
    [TestClass]
    public class PathfinderTests
    {
        const int RandomTestIterations = 100;
        static GalaxyMap TestGalaxy;
        private static IEnumerable<VectorInt3> positions;

        [ClassInitialize]
        [DeploymentItem("Dependencies\\stardata-test-large.csv")]
        public static void Setup(TestContext _tc)
        {
            positions = GalaxyTestData.LoadPositions(_tc.DeploymentDirectory + "\\stardata-test-large.csv");
            TestGalaxy = GalaxyMap.CreateNew(positions, Const.BaseWarpRangeLY);
        }
        
        [TestMethod]
        public void Easy_Path()
        {
            var start = new LYCoordinates(134, 25, 126);
            var end = new LYCoordinates(124, 17, 132);
            var path = AstarPathfinder.FindPath(
                TestGalaxy.GetNode(start),
                TestGalaxy.GetNode(end),
                Const.BaseWarpRangeLY
                );
            Assert.IsNotNull(path);
            Assert.AreEqual(2, path.Count());
        }

        [TestMethod]
        public void Test_Many_Paths_Hopefully_No_Exceptions()
        {
            var rand = new Random();
            for (int i = RandomTestIterations; i > 0; i--)
            {
                int a = rand.Next(positions.Count());
                int b = rand.Next(positions.Count());
                AstarPathfinder.FindPath(
                    TestGalaxy.GetNode(new LYCoordinates(positions.ElementAt(a))),
                    TestGalaxy.GetNode(new LYCoordinates(positions.ElementAt(b))),
                    Const.BaseWarpRangeLY
                    );
            }
        }
    }
}
