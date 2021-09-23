using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.Threading;
using GalacticWaez;
using Eleon.Modding;
using System.Collections.Generic;

namespace GalacticWaezTests
{
    [TestClass]
    [DeploymentItem("Dependencies\\stardata-test-small.csv")]
    public class InitializerTests
    {
        private static TestContext tc;

        [ClassInitialize]
        public static void SetupClass(TestContext _tc)
        {
            tc = _tc;
        }

        [TestMethod]
        public void FileInit_FileNotFound()
        {
            var fakeApp = new FakeApplication(null);
            var modApi = new FakeModApi(fakeApp);
            var init = new Initializer(modApi, new NotFoundStorage(), null, null);
            bool done = false;
            init.Initialize(Initializer.Source.File,
                (galaxy, ex) =>
                {
                    Assert.IsNull(galaxy);
                    Assert.IsTrue(modApi.LogContains("Stored star data not found."));
                    done = true;
                });
            while (!done)
            {
                Thread.Sleep(20);
                fakeApp.FireUpdate();
            }
        }

        [TestMethod]
        public void FileInit_FoundButBad()
        {
            var fakeApp = new FakeApplication(null);
            var modApi = new FakeModApi(fakeApp);
            var init = new Initializer(modApi, new CorruptedStorage(), null, null);
            bool done = false;
            init.Initialize(Initializer.Source.Scanner,
                (galaxy, ex) =>
                {
                    Assert.IsNull(galaxy);
                    Assert.IsTrue(modApi.LogContains("Failed to locate star position data. ",
                        FakeModApi.LogType.Warning
                        ));
                    done = true;
                });
            while (!done)
            {
                Thread.Sleep(20);
                fakeApp.FireUpdate();
            }
        }

        [TestMethod]
        public void ScannerInit_NotFound()
        {
            var fakeApp = new FakeApplication(null);
            var modApi = new FakeModApi(fakeApp);
            var init = new Initializer(modApi, null, new FakeStarFinder(null),
                new InitializationDB(default));
            bool done = false;
            init.Initialize(Initializer.Source.Scanner,
                (galaxy, ex) =>
                {
                    Assert.IsNull(galaxy);
                    Assert.IsTrue(modApi.LogContains("Failed to locate star position data. ",
                        FakeModApi.LogType.Warning
                        ));
                    done = true;
                });
            while (!done)
            {
                Thread.Sleep(20);
                fakeApp.FireUpdate();
            }
        }

        [TestMethod]
        public void NormalInit_BothFail()
        {
            var fakeApp = new FakeApplication(null);
            var modApi = new FakeModApi(fakeApp);
            var init = new Initializer(modApi, new CorruptedStorage(), new FakeStarFinder(null),
                new InitializationDB(default));
            bool done = false;
            init.Initialize(Initializer.Source.Normal,
                (galaxy, ex) =>
                {
                    Assert.IsNull(galaxy);
                    Assert.IsTrue(modApi.LogContains("Stored star data not found.")
                        || modApi.LogContains("Failed to load"));
                    Assert.IsTrue(modApi.LogContains("Failed to locate star position data. ",
                        FakeModApi.LogType.Warning
                        ));
                    done = true;
                });
            while (!done)
            {
                Thread.Sleep(20);
                fakeApp.FireUpdate();
            }
        }

        [TestMethod]
        public void ScannerInit_Found_NoSave()
        {
            var data = GalaxyTestData.LoadPositions(
                tc.DeploymentDirectory + "\\stardata-test-small.csv")
                .ToArray();
            var fakeApp = new FakeApplication(tc.DeploymentDirectory);
            var modApi = new FakeModApi(fakeApp);
            var init = new Initializer(modApi, null, new FakeStarFinder(data), 
                new InitializationDB(data.First()));
            bool done = false;
            init.Initialize(Initializer.Source.Scanner,
                (galaxy, ex) =>
                {
                    Assert.IsNull(ex);
                    Assert.IsNotNull(galaxy);
                    Assert.IsTrue(modApi.LogContains($"Located {data.Length} stars"));
                    done = true;
                });
            while (!done)
            {
                Thread.Sleep(20);
                fakeApp.FireUpdate();
            }
        }
    }

    public class FakeStarFinder : IStarFinder
    {
        private readonly VectorInt3[] data;

        public FakeStarFinder(VectorInt3[] starData) => data = starData;

        public VectorInt3[] Search(VectorInt3 knownPosition) => data;
    }

    public class SuccessfulStorage : IStarDataStorage
    {
        private readonly string path;

        public SuccessfulStorage(string path)
        {
            this.path = path;
        }

        public bool Exists() => true;

        public IEnumerable<VectorInt3> Load()
            => GalaxyTestData.LoadPositions(path);

        public bool Store(IEnumerable<VectorInt3> _) => true;
    }

    public class NotFoundStorage : IStarDataStorage
    {
        public bool Exists() => false;

        public IEnumerable<VectorInt3> Load() => null;

        public bool Store(IEnumerable<VectorInt3> _)
            => throw new NotImplementedException();
    }

    public class CorruptedStorage : IStarDataStorage
    {
        public bool Exists() => true;

        public IEnumerable<VectorInt3> Load() => null;

        public bool Store(IEnumerable<VectorInt3> _)
            => throw new NotImplementedException();
    }

    public class InitializationDB : ISaveGameDB
    {
        private readonly VectorInt3 first;

        public InitializationDB(VectorInt3 vector) => first = vector;

        public int ClearPathMarkers(int playerId)
            => throw new NotImplementedException();

        public bool GetBookmarkVector(string bookmarkName, out VectorInt3 coordinates)
            => throw new NotImplementedException();

        public VectorInt3 GetFirstKnownStarPosition() => first;

        public float GetLocalPlayerWarpRange()
            => throw new NotImplementedException();

        public float GetPlayerWarpRange(int playerId)
            => throw new NotImplementedException();

        public bool GetSolarSystemCoordinates(string starName, out VectorInt3 coordinates)
            => throw new NotImplementedException();

        public int InsertBookmarks(IEnumerable<VectorInt3> positions, IPlayer player)
            => throw new NotImplementedException();
    }
}
