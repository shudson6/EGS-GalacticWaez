using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using GalacticWaez;
using Eleon.Modding;

namespace GalacticWaezTests
{
    [TestClass]
    [DeploymentItem("Dependencies\\stardata-test-small.csv")]
    public partial class NavigatorTests
    {
        private static readonly Func<ulong> TestTicks = () => 7231013;
        private static IGalaxyMap galaxy;
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

        [TestMethod]
        public  void Navigate_BookmarkFound_7StepPath()
        {
            var path = positions.Take(7);
            var pathfinder = new Fakes.FakePathfinder(path);
            var bm = new Fakes.HappyBookmarkManager(path.Last());
            string logged = null;
            var nav = new Navigator(galaxy, pathfinder, bm, new Fakes.FakeStarProvider(), 
                (text) => logged = text, TestTicks);
            var player = new Fakes.NavTestPlayerInfo(1337, 1337, path.First(), 30);
            var rslt = nav.Navigate(player, "foo", 30, null).Result;
            Assert.AreEqual(7, rslt.Count());
            Assert.AreEqual(5, bm.Inserted);
            Assert.AreEqual("Path found; 5/5 waypoints added.", logged);
        }

        [TestMethod]
        public  void Navigate_StarFound_7StepPath()
        {
            var path = positions.Take(7);
            var pathfinder = new Fakes.FakePathfinder(path);
            var bm = new Fakes.NotFoundBookmarkManager();
            string logged = null;
            var nav = new Navigator(galaxy, pathfinder, bm, new Fakes.FakeStarProvider(path.Last()), 
                (text) => logged = text, TestTicks);
            var player = new Fakes.NavTestPlayerInfo(1337, 1337, path.First(), 30);
            var rslt = nav.Navigate(player, "foo", 30, null).Result;
            Assert.AreEqual(7, rslt.Count());
            Assert.AreEqual(6, bm.Inserted);
            Assert.AreEqual("Path found; 6/6 waypoints added.", logged);
        }

        [TestMethod]
        public  void Navigate_DestinationNotFound()
        {
            var pathfinder = new Fakes.FakePathfinder();
            var bm = new Fakes.NotFoundBookmarkManager();
            var stars = new Fakes.NotFoundStarProvider();
            string logged = null;
            var nav = new Navigator(galaxy, pathfinder, bm, stars,
                (text) => logged = text, TestTicks);
            var player = new Fakes.NavTestPlayerInfo(1337, 1337, default, 30);
            var response = new Fakes.TestResponder();
            Assert.IsNull(nav.Navigate(player, "foo", 30, response).Result);
            Assert.AreEqual(1, response.Messages.Count);
            Assert.IsTrue(response.Messages[0].StartsWith("No bookmark or known star"));
        }

        [TestMethod]
        public  void Navigate_DestinationEqualStartBookmark()
        {
            var vector = positions.First();
            var pathfinder = new Fakes.FakePathfinder();
            var bm = new Fakes.HappyBookmarkManager(vector);
            var stars = new Fakes.NotFoundStarProvider();
            string logged = null;
            var nav = new Navigator(galaxy, pathfinder, bm, stars,
                (text) => logged = text, TestTicks);
            var player = new Fakes.NavTestPlayerInfo(1337, 1337, vector, 30);
            var response = new Fakes.TestResponder();
            Assert.IsNull(nav.Navigate(player, "foo", 30, response).Result);
            Assert.AreEqual(1, response.Messages.Count);
            Assert.AreEqual("It appears you are already there.", response.Messages[0]);
        }

        [TestMethod]
        public  void Navigate_DestinationEqualStartStar()
        {
            var vector = positions.First();
            var pathfinder = new Fakes.FakePathfinder();
            var bm = new Fakes.NotFoundBookmarkManager();
            var stars = new Fakes.FakeStarProvider(vector);
            string logged = null;
            var nav = new Navigator(galaxy, pathfinder, bm, stars,
                (text) => logged = text, TestTicks);
            var player = new Fakes.NavTestPlayerInfo(1337, 1337, vector, 30);
            var response = new Fakes.TestResponder();
            Assert.IsNull(nav.Navigate(player, "foo", 30, response).Result);
            Assert.AreEqual(1, response.Messages.Count);
            Assert.AreEqual("It appears you are already there.", response.Messages[0]);
        }

        [TestMethod]
        public  void Navigate_NoPathBookmark()
        {
            var pathfinder = new Fakes.FakePathfinder();
            var bm = new Fakes.HappyBookmarkManager(positions.First());
            var stars = new Fakes.NotFoundStarProvider();
            string logged = null;
            var nav = new Navigator(galaxy, pathfinder, bm, stars,
                (text) => logged = text, TestTicks);
            var player = new Fakes.NavTestPlayerInfo(1337, 1337, positions.Last(), 30);
            var response = new Fakes.TestResponder();
            Assert.IsNull(nav.Navigate(player, "foo", 30, response).Result);
            Assert.AreEqual(1, response.Messages.Count);
            Assert.AreEqual("No path found.", response.Messages[0]);
        }

        [TestMethod]
        public  void Navigate_NoPathStar()
        {
            var pathfinder = new Fakes.FakePathfinder();
            var bm = new Fakes.NotFoundBookmarkManager();
            var stars = new Fakes.FakeStarProvider(positions.First());
            string logged = null;
            var nav = new Navigator(galaxy, pathfinder, bm, stars,
                (text) => logged = text, TestTicks);
            var player = new Fakes.NavTestPlayerInfo(1337, 1337, positions.Last(), 30);
            var response = new Fakes.TestResponder();
            Assert.IsNull(nav.Navigate(player, "foo", 30, response).Result);
            Assert.AreEqual(1, response.Messages.Count);
            Assert.AreEqual("No path found.", response.Messages[0]);
        }

        [TestMethod]
        public  void Navigate_Impossible_OneNodePath()
        {
            var path = new[] { positions.First() };
            var pathfinder = new Fakes.FakePathfinder(path);
            var bm = new Fakes.NotFoundBookmarkManager();
            var stars = new Fakes.FakeStarProvider(positions.First());
            string logged = null;
            var nav = new Navigator(galaxy, pathfinder, bm, stars,
                (text) => logged = text, TestTicks);
            var player = new Fakes.NavTestPlayerInfo(1337, 1337, positions.Last(), 30);
            var response = new Fakes.TestResponder();
            Assert.IsNull(nav.Navigate(player, "foo", 30, response).Result);
            Assert.AreEqual(1, response.Messages.Count);
            Assert.AreEqual("Are you already there?", response.Messages[0]);
        }

        [TestMethod]
        public  void Navigate_InRangeOfBookmark_ShouldNotAdd()
        {
            // using a 2-node path (1 jump) so Navigator thinks we're already in range
            var path = positions.Take(2);
            var pathfinder = new Fakes.FakePathfinder(path);
            var bm = new Fakes.HappyBookmarkManager(path.Last());
            var stars = new Fakes.NotFoundStarProvider();
            string logged = null;
            var nav = new Navigator(galaxy, pathfinder, bm, stars,
                (text) => logged = text, TestTicks);
            var player = new Fakes.NavTestPlayerInfo(1337, 1337, path.First(), 30);
            var response = new Fakes.TestResponder();
            Assert.IsNull(nav.Navigate(player, "foo", 30, response).Result);
            Assert.AreEqual(1, response.Messages.Count);
            Assert.AreEqual("It appears you are already in warp range.", response.Messages[0]);
            Assert.AreEqual(0, bm.Inserted);
        }

        [TestMethod]
        public  void Navigate_InRangeOfStar_ShouldAdd()
        {
            // using a 2-node path (1 jump) so Navigator thinks we're already in range
            var path = positions.Take(2);
            var pathfinder = new Fakes.FakePathfinder(path);
            var bm = new Fakes.NotFoundBookmarkManager();
            var stars = new Fakes.FakeStarProvider(path.Last());
            string logged = null;
            var nav = new Navigator(galaxy, pathfinder, bm, stars,
                (text) => logged = text, TestTicks);
            var player = new Fakes.NavTestPlayerInfo(1337, 1337, path.First(), 30);
            var response = new Fakes.TestResponder();
            var rslt = nav.Navigate(player, "foo", 30, response).Result;
            Assert.AreEqual(2, rslt.Count());
            Assert.AreEqual(1, response.Messages.Count);
            Assert.AreEqual("Path found; 1/1 waypoints added.", response.Messages[0]);
            Assert.AreEqual(1, bm.Inserted);
        }
    }
}
