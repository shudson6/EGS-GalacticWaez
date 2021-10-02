﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
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

        [TestMethod]
        public void Navigate_BookmarkFound_7StepPath()
        {
            var path = positions.Take(7);
            var pathfinder = new Fakes.FakePathfinder(path);
            var bm = new Fakes.HappyBookmarkManager(path.Last());
            string logged = null;
            var nav = new Navigator(galaxy, pathfinder, bm, new Fakes.FakeStarProvider(), 
                (text) => logged = text, TestTicks);
            var player = new Fakes.NavTestPlayerInfo(new Fakes.FakePlayer(1337), path.First(), 30);
            nav.Navigate(player, "foo", 30, null);
            Assert.AreEqual(5, bm.Inserted);
            Assert.AreEqual("Path found; 5/5 waypoints added.", logged);
        }

        [TestMethod]
        public void Navigate_StarFound_7StepPath()
        {
            var path = positions.Take(7);
            var pathfinder = new Fakes.FakePathfinder(path);
            var bm = new Fakes.NotFoundBookmarkManager();
            string logged = null;
            var nav = new Navigator(galaxy, pathfinder, bm, new Fakes.FakeStarProvider(path.Last()), 
                (text) => logged = text, TestTicks);
            var player = new Fakes.NavTestPlayerInfo(new Fakes.FakePlayer(1337), path.First(), 30);
            nav.Navigate(player, "foo", 30, null);
            Assert.AreEqual(6, bm.Inserted);
            Assert.AreEqual("Path found; 6/6 waypoints added.", logged);
        }

        [TestMethod]
        public void Navigate_DestinationNotFound()
        {
            var pathfinder = new Fakes.FakePathfinder();
            var bm = new Fakes.NotFoundBookmarkManager();
            var stars = new Fakes.NotFoundStarProvider();
            string logged = null;
            var nav = new Navigator(galaxy, pathfinder, bm, stars,
                (text) => logged = text, TestTicks);
            var player = new Fakes.NavTestPlayerInfo(new Fakes.FakePlayer(1337), default, 30);
            var response = new Fakes.TestResponder();
            nav.Navigate(player, "foo", 30, response);
            Assert.AreEqual(1, response.Messages.Count);
            Assert.IsTrue(response.Messages[0].StartsWith("No bookmark or known star"));
        }
    }
}
