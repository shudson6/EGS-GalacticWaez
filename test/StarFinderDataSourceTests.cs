using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using GalacticWaez;
using Eleon.Modding;
using System.Collections.Generic;

namespace GalacticWaezTests
{
    [TestClass]
    [DeploymentItem("Dependencies\\stardata-test-vanilla.csv")]
    public class StarFinderDataSourceTests
    {
        [TestMethod]
        public void ReturnsNull_WhenNotFound()
        {
            // need to use a vector here that isn't going to be found by any other test
            Assert.IsNull(new StarFinderDataSource(new KnownVectorDB(default)).GetGalaxyData());
        }

        [TestMethod]
        public void Finds_Expected_VanillaData()
        {
            var gameData = GalaxyTestData.LoadGameStyleStarArray("stardata-test-vanilla.csv");
            var expected = GalaxyTestData.LoadPositions("stardata-test-vanilla.csv");
            var actual = new StarFinderDataSource(new KnownVectorDB(expected[0])).GetGalaxyData();
            Assert.IsTrue(gameData.Count > 0);
            Assert.IsTrue(expected.Count > 0);
            Assert.AreEqual(expected.Count, actual.Count());
            Assert.IsFalse(expected.Except(actual).Any());
        }
    }

    class KnownVectorDB : ISaveGameDB
    {
        private readonly VectorInt3 knownVector;
        public KnownVectorDB(VectorInt3 known) { knownVector = known; }

        public int ClearPathMarkers(int playerId)
        {
            throw new NotImplementedException();
        }

        public bool GetBookmarkVector(string bookmarkName, out VectorInt3 coordinates)
        {
            throw new NotImplementedException();
        }

        public VectorInt3 GetFirstKnownStarPosition() => knownVector;

        public bool GetSolarSystemCoordinates(string starName, out VectorInt3 coordinates)
        {
            throw new NotImplementedException();
        }

        public int InsertBookmarks(IEnumerable<VectorInt3> positions, int playerId)
        {
            throw new NotImplementedException();
        }
    }
}
