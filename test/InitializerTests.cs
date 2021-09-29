using Eleon.Modding;
using GalacticWaez;
using GalacticWaez.Navigation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace GalacticWaezTests
{
    [TestClass]
    [DeploymentItem("Dependencies\\stardata-test-small.csv")]
    public class InitializerTests
    {
        private static TestContext tc;
        private static string DataFilePath;

        [ClassInitialize]
        public static void SetupClass(TestContext _tc)
        {
            tc = _tc;
            DataFilePath = tc.DeploymentDirectory + "\\stardata-test-small.csv";
        }

        [TestMethod]
        public void FileInit_FileNotFound()
        {
            var fakeApp = new FakeApplication(null);
            var modApi = new FakeModApi(fakeApp);
            var init = new ClientInitializer(modApi, new NotFoundStorage(), null, null);
            bool done = false;
            init.Initialize(StarDataSource.File,
                (galaxy, ex) =>
                {
                    Assert.IsNull(galaxy);
                    Assert.IsTrue(modApi.LogContains("Stored star data not found.",
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
        public void FileInit_FoundCorrupt()
        {
            var fakeApp = new FakeApplication(null);
            var modApi = new FakeModApi(fakeApp);
            var init = new ClientInitializer(modApi, new CorruptedStorage(), null, null);
            bool done = false;
            init.Initialize(StarDataSource.File,
                (galaxy, ex) =>
                {
                    Assert.IsNull(galaxy);
                    Assert.IsTrue(modApi.LogContains("Failed to load star data",
                        FakeModApi.LogType.Error
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
            var init = new ClientInitializer(modApi, null, new FakeStarFinder(null),
                new InitializationDB(default));
            bool done = false;
            init.Initialize(StarDataSource.Scanner,
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
        public void ScannerInit_Found_DontSave()
        {
            var data = GalaxyTestData.LoadPositions(DataFilePath).ToArray();
            var fakeApp = new FakeApplication(tc.DeploymentDirectory);
            var modApi = new FakeModApi(fakeApp);
            var init = new ClientInitializer(modApi, null, new FakeStarFinder(data), 
                new InitializationDB(data.First()));
            bool done = false;
            init.Initialize(StarDataSource.Scanner,
                (galaxy, ex) =>
                {
                    Assert.IsNull(ex);
                    Assert.IsNotNull(galaxy);
                    Assert.IsTrue(modApi.LogContains($"Located {data.Length} stars"));

                    // ensure no attempt was made to save
                    Assert.IsFalse(modApi.LogContains("Saved star positions to file."));
                    Assert.IsFalse(modApi.LogContains("Could not save",
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
        public void NormalInit_FileCorrupt()
        {
            var fakeApp = new FakeApplication(null);
            var modApi = new FakeModApi(fakeApp);
            var init = new ClientInitializer(modApi, new CorruptedStorage(), new FakeStarFinder(null),
                new InitializationDB(default));
            bool done = false;
            init.Initialize(StarDataSource.Normal,
                (galaxy, ex) =>
                {
                    Assert.IsNull(galaxy);
                    Assert.IsTrue(modApi.LogContains("Failed to load star data",
                        FakeModApi.LogType.Error
                        ));

                    // should not scan if file was found
                    Assert.IsFalse(modApi.LogContains("Failed to locate star position data. ",
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
        public void NormalInit_NoFile_ScanFail()
        {
            var fakeApp = new FakeApplication(null);
            var modApi = new FakeModApi(fakeApp);
            var init = new ClientInitializer(modApi, new NotFoundStorage(), new FakeStarFinder(null),
                new InitializationDB(default));
            bool done = false;
            init.Initialize(StarDataSource.Normal,
                (galaxy, ex) =>
                {
                    Assert.IsNull(galaxy);
                    Assert.IsTrue(modApi.LogContains("No saved star positions."));

                    // should not scan if file was found
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

        private void CheckNeighborDistances(Galaxy.Node node, HashSet<Galaxy.Node> checkedNodes, float MaxDistance)
        {
            foreach (var n in node.Neighbors)
            {
                if (checkedNodes.Add(n.Key))
                {
                    Assert.AreEqual(Distance(node.Position, n.Key.Position), n.Value);
                    Assert.IsTrue(n.Value <= MaxDistance, "{0} -> {1} = {2} > {3}", 
                        node.Position, n.Key.Position, n.Value, MaxDistance);
                    CheckNeighborDistances(n.Key, checkedNodes, MaxDistance);
                }
            }
        }
        private float Distance(LYCoordinates a, LYCoordinates b)
        {
            float dx = a.x - b.x;
            float dy = a.y - b.y;
            float dz = a.z - b.z;
            return (float)Math.Sqrt(dx * dx + dy * dy + dz * dz);
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
        private readonly IEnumerable<VectorInt3> data;
        private readonly bool saveSuccessful;

        public SuccessfulStorage(IEnumerable<VectorInt3> data, bool willSucceed = true)
        {
            this.data = data;
            saveSuccessful = willSucceed;
        }

        public bool Exists() => true;

        public IEnumerable<VectorInt3> Load() => data;

        public bool Store(IEnumerable<VectorInt3> _) => saveSuccessful;
    }

    public class NotFoundStorage : IStarDataStorage
    {
        private readonly bool saveSuccessful;

        public NotFoundStorage(bool willSucceed = false)
        {
            saveSuccessful = willSucceed;
        }

        public bool Exists() => false;

        public IEnumerable<VectorInt3> Load() => null;

        public bool Store(IEnumerable<VectorInt3> _) => saveSuccessful;
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

        public bool GetSolarSystemCoordinates(string starName, out VectorInt3 coordinates)
            => throw new NotImplementedException();

        public int InsertBookmarks(IEnumerable<VectorInt3> positions, int playerId)
            => throw new NotImplementedException();
    }
}
