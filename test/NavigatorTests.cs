using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using GalacticWaez;
using Eleon.Modding;

namespace GalacticWaezTests
{
    [TestClass]
    [DeploymentItem("Dependencies\\stardata-test-small.csv")]
    public class NavigatorTests
    {
        private static Func<ulong> TestTicks = () => 7231013;
        private static GalaxyMap galaxy;
        private static IEnumerable<VectorInt3> positions;

        [ClassInitialize]
        public static void SetupClass(TestContext tc)
        {
            string file = tc.DeploymentDirectory + "\\stardata-test-small.csv";
            positions = GalaxyTestData.LoadPositions(file);
            galaxy = GalaxyTestData.BuildTestGalaxy(file, 30);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Throw_NullGalaxy()
        {
            new Navigator(null, new Fakes.FakePathfinder(), new Fakes.FakeBookmarkManager(), 
                new Fakes.FakeStarProvider(), delegate { }, TestTicks);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Throw_NullPathfinder()
        {
            new Navigator(galaxy, null, new Fakes.FakeBookmarkManager(), 
                new Fakes.FakeStarProvider(), delegate { }, TestTicks);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Throw_NullBookmarkManager()
        {
            new Navigator(galaxy, new Fakes.FakePathfinder(), null,
                new Fakes.FakeStarProvider(), delegate { }, TestTicks);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Throw_NullStarProvider()
        {
            new Navigator(galaxy, new Fakes.FakePathfinder(), new Fakes.FakeBookmarkManager(),
                null, delegate { }, TestTicks);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Throw_NullTicks()
        {
            new Navigator(galaxy, new Fakes.FakePathfinder(), new Fakes.FakeBookmarkManager(),
                new Fakes.FakeStarProvider(), delegate { }, null);
        }

        [TestMethod]
        public void NoLogger_NoProblem()
        {
            Assert.IsNotNull(new Navigator(galaxy, new Fakes.FakePathfinder(), new Fakes.FakeBookmarkManager(),
                new Fakes.FakeStarProvider(), null, TestTicks));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Navigate_Throw_NullPlayer()
        {
            new Navigator(galaxy, new Fakes.FakePathfinder(), new Fakes.FakeBookmarkManager(),
                new Fakes.FakeStarProvider(), delegate { }, TestTicks)
                .Navigate(null, "foo", 30, new Fakes.FakeResponder());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Navigate_Throw_NullDestination()
        {
            new Navigator(galaxy, new Fakes.FakePathfinder(), new Fakes.FakeBookmarkManager(),
                new Fakes.FakeStarProvider(), delegate { }, TestTicks)
                .Navigate(new Fakes.FakePlayerInfo(), null, 30, new Fakes.FakeResponder());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void Navigate_Throw_NegativeRange()
        {
            new Navigator(galaxy, new Fakes.FakePathfinder(), new Fakes.FakeBookmarkManager(),
                new Fakes.FakeStarProvider(), delegate { }, TestTicks)
                .Navigate(new Fakes.FakePlayerInfo(), "foo", -30, new Fakes.FakeResponder());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void Navigate_Throw_ZeroRange()
        {
            new Navigator(galaxy, new Fakes.FakePathfinder(), new Fakes.FakeBookmarkManager(),
                new Fakes.FakeStarProvider(), delegate { }, TestTicks)
                .Navigate(new Fakes.FakePlayerInfo(), "foo", 0, new Fakes.FakeResponder());
        }
    }
}
