using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using GalacticWaez;

namespace GalacticWaezTests
{
    [TestClass]
    public class AstarPathfinderTests
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void FindPath_Throw_StartNull()
        {
            new AstarPathfinder().FindPath(null, new GalaxyMap.Node(default), 30);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void FindPath_Throw_GoalNull()
        {
            new AstarPathfinder().FindPath(new GalaxyMap.Node(default), null, 30);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void FindPath_Throw_RangeNegative()
        {
            new AstarPathfinder().FindPath(new GalaxyMap.Node(default), new GalaxyMap.Node(default), -30);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void FindPath_Throw_RangeZero()
        {
            new AstarPathfinder().FindPath(new GalaxyMap.Node(default), new GalaxyMap.Node(default), 0);
        }
    }

    // TODO: add tests that verify returned paths
}
