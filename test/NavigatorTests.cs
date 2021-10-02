using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using GalacticWaez;

namespace GalacticWaezTests
{
    [TestClass]
    [DeploymentItem("Dependencies\\stardata-test-small.csv")]
    public class NavigatorTests
    {
        private static Func<ulong> TestTicks = () => 7231013;
        private static GalaxyMap galaxy;

        [ClassInitialize]
        public static void SetupClass(TestContext tc)
        {
            galaxy = GalaxyTestData.BuildTestGalaxy(tc.DeploymentDirectory 
                + "\\stardata-test-small.csv", 30);
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
    }
}
