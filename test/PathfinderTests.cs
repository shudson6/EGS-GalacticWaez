using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using GalacticWaez;

namespace GalacticWaezTests
{
    [TestClass]
    public class PathfinderTests
    {
        const int RandomTestIterations = 100;
        static Galaxy TestGalaxy;

        [ClassInitialize]
        public static void Setup(TestContext wtf)
        {
            TestGalaxy = Galaxy.CreateNew(GalaxyDataPrep.Locations, Const.BaseWarpRange);
        }
        
        [TestMethod]
        public void Easy_Path()
        {
            var start = new LYCoordinates(134, 25, 126);
            var end = new LYCoordinates(124, 17, 132);
            var path = AstarPathfinder.FindPath(
                TestGalaxy.GetNode(start),
                TestGalaxy.GetNode(end));
            Assert.IsNotNull(path);
            Assert.AreEqual(2, path.Count());
        }

        [TestMethod]
        public void Test_Many_Paths_Hopefully_No_Exceptions()
        {
            var rand = new Random();
            for (int i = RandomTestIterations; i > 0; i--)
            {
                int a = rand.Next(GalaxyDataPrep.Locations.Count);
                int b = rand.Next(GalaxyDataPrep.Locations.Count);
                AstarPathfinder.FindPath(
                    TestGalaxy.GetNode(new LYCoordinates(GalaxyDataPrep.Locations.ElementAt(a))),
                    TestGalaxy.GetNode(new LYCoordinates(GalaxyDataPrep.Locations.ElementAt(b)))
                    );
            }
        }
    }
}
