using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using GalacticWaez;
using Eleon.Modding;

namespace GalacticWaezTests
{
    [TestClass]
    [DeploymentItem("Dependencies\\stardata-test-vanilla.csv")]
    public class StarFinderDataSourceTests
    {
        [TestMethod]
        public void ReturnsNull_WhenNotFound()
        {
            Assert.IsNull(new StarFinderDataSource(new KnownVectorDB(default), (_) => { })
                .GetGalaxyData());
        }

        [TestMethod]
        public void Finds_Expected_VanillaData()
        {
            // running this test in its own domain.
            // because StarFinder will scan app memory, it needs to be isolated from other tests
            // which might contaminate it.
            var testDomain = AppDomain.CreateDomain("Test Domain: ReturnsExpected_VanillaData_LogNull",
                AppDomain.CurrentDomain.Evidence, AppDomain.CurrentDomain.SetupInformation);
            testDomain.DoCallBack(() =>
            {
                var gameData = GalaxyTestData.LoadGameStyleStarArray("stardata-test-vanilla.csv");
                var expected = GalaxyTestData.LoadPositions("stardata-test-vanilla.csv");
                var actual = new StarFinderDataSource(new KnownVectorDB(expected[0]), (_) => { })
                    .GetGalaxyData();
                Assert.IsTrue(gameData.Count > 0);
                Assert.IsTrue(expected.Count > 0);
                Assert.AreEqual(expected.Count, actual.Count());
                Assert.IsFalse(expected.Except(actual).Any());
            });
        }

        [TestMethod]
        public void ReturnsNull_WhenDbNull()
        {
            Assert.IsNull(new StarFinderDataSource(null, (_) => { }).GetGalaxyData());
        }

        [TestMethod]
        public void ReturnsNull_WhenDbNullAndLogNull()
        {
            Assert.IsNull(new StarFinderDataSource(null, null).GetGalaxyData());
        }

        [TestMethod]
        public void ReturnsNull_WhenNotFound_LogNull()
        {
            Assert.IsNull(new StarFinderDataSource(new KnownVectorDB(default), null).GetGalaxyData());
        }

        [TestMethod]
        public void ReturnsExpected_VanillaData_LogNull()
        {
            // running this test in its own domain.
            // because StarFinder will scan app memory, it needs to be isolated from other tests
            // which might contaminate it.
            var testDomain = AppDomain.CreateDomain("Test Domain: ReturnsExpected_VanillaData_LogNull",
                AppDomain.CurrentDomain.Evidence, AppDomain.CurrentDomain.SetupInformation);
            testDomain.DoCallBack(() =>
            {
                var gameData = GalaxyTestData.LoadGameStyleStarArray("stardata-test-vanilla.csv");
                var expected = GalaxyTestData.LoadPositions("stardata-test-vanilla.csv");
                var actual = new StarFinderDataSource(new KnownVectorDB(expected[0]), null)
                    .GetGalaxyData();
                Assert.IsTrue(gameData.Count > 0);
                Assert.IsTrue(expected.Count > 0);
                Assert.AreEqual(expected.Count, actual.Count());
                Assert.IsFalse(expected.Except(actual).Any());
            });
        }
    }

    class KnownVectorDB : IKnownStarProvider
    {
        private readonly VectorInt3 knownVector;
        public KnownVectorDB(VectorInt3 known) { knownVector = known; }

        public bool GetFirstKnownStarPosition(out VectorInt3 known)
        {
            known = knownVector;
            return true;
        }
    }
}
