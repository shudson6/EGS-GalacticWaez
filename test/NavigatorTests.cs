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
        public void BookmarkGoal_YoureAlreadyThere()
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

        [TestMethod]
        public void BookmarkGoal_YoureAlreadyInRange()
        {
            var pos = new[] { new VectorInt3(100000, 200000, 300000),
                              new VectorInt3(400000, 500000, 600000)
            };
            var app = new FakeApplication(null);
            var modApi = new FakeModApi(app);
            var galaxy = Galaxy.CreateNew(pos, Const.BaseWarpRange);
            var nav = new Navigator(modApi, galaxy, new NavigatorTestDB(true, false, pos[1]));
            bool done = false;
            nav.HandlePathRequest("nonexistent",
                new FakePlayerTracker(1, 30, pos[0]),
                // provide a pathfinder delegate that returns a path with exactly 2 nodes in it
                (start, end, _) => new[] { start.Position, end.Position },
                (path) =>
                {
                    Assert.IsNull(path);
                    Assert.IsTrue(modApi.LogContains("It appears you are already in warp range."));
                    done = true;
                });
            while (!done)
            {
                Thread.Sleep(20);
                app.FireUpdate();
            }
        }

        [TestMethod]
        public void BookmarkGoal_NoPath()
        {
            var pos = new[] { new VectorInt3(100000, 200000, 300000),
                              new VectorInt3(400000, 500000, 600000)
            };
            var app = new FakeApplication(null);
            var modApi = new FakeModApi(app);
            var galaxy = Galaxy.CreateNew(pos, Const.BaseWarpRange);
            var nav = new Navigator(modApi, galaxy, new NavigatorTestDB(true, false, pos[1]));
            bool done = false;
            nav.HandlePathRequest("nonexistent",
                new FakePlayerTracker(1, 30, pos[0]),
                (start, end, _) => null,
                (path) =>
                {
                    Assert.IsNull(path);
                    Assert.IsTrue(modApi.LogContains("No path found."));
                    done = true;
                });
            while (!done)
            {
                Thread.Sleep(20);
                app.FireUpdate();
            }
        }

        [TestMethod]
        public void ThisSituationShouldBeImpossible()
        {
            // it should be impossible to get a path of length 1, because at least the start and end
            // nodes must be present
            // even so, let's make sure it wouldn't break anything if it happened
            var pos = new[] { new VectorInt3(100000, 200000, 300000),
                              new VectorInt3(400000, 500000, 600000)
            };
            var app = new FakeApplication(null);
            var modApi = new FakeModApi(app);
            var galaxy = Galaxy.CreateNew(pos, Const.BaseWarpRange);
            var nav = new Navigator(modApi, galaxy, new NavigatorTestDB(true, false, pos[1]));
            bool done = false;
            nav.HandlePathRequest("nonexistent",
                new FakePlayerTracker(1, 30, pos[0]),
                // provide a pathfinder delegate that returns a path with exactly 1 node in it
                (start, end, _) => new[] { default(LYCoordinates) },
                (path) =>
                {
                    Assert.IsNull(path);
                    Assert.IsTrue(modApi.LogContains("Are you already there?"));
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
        private readonly VectorInt3 bookmarkCoordinates;
        private readonly VectorInt3 solarSystemCoordinates;

        public NavigatorTestDB(bool findBookmark, bool findSolarSystem,
            VectorInt3 bookmark = default, VectorInt3 system = default)
        {
            this.findBookmark = findBookmark;
            this.findSolarSystem = findSolarSystem;
            bookmarkCoordinates = bookmark;
            solarSystemCoordinates = system;
        }

        public int ClearPathMarkers(int playerId) => throw new NotImplementedException();

        public bool GetBookmarkVector(string bookmarkName, out VectorInt3 coordinates)
        {
            coordinates = bookmarkCoordinates;
            return findBookmark;
        }

        public VectorInt3 GetFirstKnownStarPosition()
            => throw new NotImplementedException();

        public bool GetSolarSystemCoordinates(string starName, out VectorInt3 coordinates)
        {
            coordinates = solarSystemCoordinates;
            return findSolarSystem;
        }

        public int InsertBookmarks(IEnumerable<VectorInt3> positions, int playerId)
            => positions.Count();
    }

    public class FakePlayerTracker : IPlayerTracker
    {
        private readonly int playerId;
        private readonly float warpRange;
        private readonly VectorInt3 coords;

        public FakePlayerTracker(int id, float range, VectorInt3 pos)
        {
            playerId = id;
            warpRange = range;
            coords = pos;
        }

        public VectorInt3 GetCurrentStarCoordinates() => coords;

        public int GetPlayerId() => playerId;

        public float GetWarpRange() => warpRange;
    }
}
