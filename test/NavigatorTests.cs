using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using GalacticWaez;
using Eleon.Modding;
using GalacticWaez.Navigation;

namespace GalacticWaezTests
{
    [TestClass]
    [DeploymentItem("Dependencies\\stardata-test-small.csv")]
    public class NavigatorTests
    {
        private static Galaxy galaxy;
        [ClassInitialize]
        public static void SetupClass(TestContext tc)
        {
            var pos = GalaxyTestData.LoadPositions(tc.DeploymentDirectory + "\\stardata-test-small.csv");
            galaxy = Galaxy.CreateNew(pos, Const.BaseWarpRange);
        }

        [TestMethod]
        public void DestinationNotFound()
        {
            var app = new FakeApplication(null);
            var modApi = new FakeModApi(app);
            var nav = new Navigator(modApi, galaxy, new NavigatorTestDB(false, false));
            bool done = false;
            nav.HandlePathRequest("nonexistent",
                new FakePlayerTracker(1, 30, default),
                new AstarPathfinder(), // passing a real one b/c it will cause problems if it gets invoked
                (path) =>
                {
                    Assert.IsNull(path);
                    Assert.IsTrue(modApi.LogContains("No bookmark or", FakeModApi.LogType.Warning));
                    done = true;
                });
            while (!done)
            {
                Thread.Sleep(20);
                app.FireUpdate();
            }
        }
    }

    public class NavigatorTestDB : ISaveGameDB
    {
        private readonly bool findBookmark;
        private readonly bool findSolarSystem;

        public NavigatorTestDB(bool findBookmark, bool findSolarSystem)
        {
            this.findBookmark = findBookmark;
            this.findSolarSystem = findSolarSystem;
        }

        public int ClearPathMarkers(int playerId) => throw new NotImplementedException();

        public bool GetBookmarkVector(string bookmarkName, out VectorInt3 coordinates)
        {
            coordinates = default;
            return findBookmark;
        }

        public VectorInt3 GetFirstKnownStarPosition()
            => throw new NotImplementedException();

        public bool GetSolarSystemCoordinates(string starName, out VectorInt3 coordinates)
        {
            coordinates = default;
            return findSolarSystem;
        }

        public int InsertBookmarks(IEnumerable<VectorInt3> positions, int playerId)
            => positions.Count();
    }

    public class FakePlayerTracker : IPlayerTracker
    {
        private readonly int playerId;
        private readonly float warpRange;
        private readonly LYCoordinates coords;

        public FakePlayerTracker(int id, float range, LYCoordinates pos)
        {
            playerId = id;
            warpRange = range;
            coords = pos;
        }

        public LYCoordinates GetCurrentStarCoordinates() => coords;

        public int GetPlayerId() => playerId;

        public float GetWarpRange() => warpRange;
    }
}
