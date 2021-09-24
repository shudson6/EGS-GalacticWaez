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
    public class NavigatorTests
    {
        [TestMethod]
        public void DestinationNotFound()
        {
            var app = new FakeApplication(null);
            var modApi = new FakeModApi(app);
            var nav = new Navigator(modApi, null, new NavigatorTestDB(false, false));
            bool done = false;
            nav.HandlePathRequest("nonexistent",
                new FakePlayerTracker(1, 30, default),
                AstarPathfinder.FindPath,
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

        [TestMethod]
        public void Bookmark_AlreadyThere()
        {
            var app = new FakeApplication(null);
            var modApi = new FakeModApi(app);
            var galaxy = Galaxy.CreateNew(new[] { default(VectorInt3) }, Const.BaseWarpRange);
            var nav = new Navigator(modApi, galaxy, new NavigatorTestDB(true, false));
            bool done = false;
            nav.HandlePathRequest("nonexistent",
                new FakePlayerTracker(1, 30, default),
                AstarPathfinder.FindPath,
                (path) =>
                {
                    Assert.IsNull(path);
                    Assert.IsTrue(modApi.LogContains("It appears you are already there."));
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
