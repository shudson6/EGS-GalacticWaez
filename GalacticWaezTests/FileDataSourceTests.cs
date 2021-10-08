using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Eleon.Modding;
using GalacticWaez;

namespace GalacticWaezTests
{
    [TestClass]
    [DeploymentItem("Dependencies\\stardata-test-small.csv")]
    [DeploymentItem("Dependencies\\stardata-test-badsum.csv")]
    public class FileDataSourceTests
    {
        private static TestContext tc; 
        [ClassInitialize]
        public static void SetupClass(TestContext _tc)
        {
            tc = _tc;
            Directory.CreateDirectory(tc.DeploymentDirectory + "\\Content\\Mods\\GalacticWaez");
        }

        [TestMethod]
        public void Constructor_NoException_NullSaveGameDir()
        {
            Assert.IsNull(new FileDataSource(null, delegate { }).GetGalaxyData());
        }

        [TestMethod]
        public void Constructor_NullDelegateNoProblem()
        {
            Assert.IsNotNull(new FileDataSource("foo", null));
        }

        [TestMethod]
        public void PathToFile_Correct()
        {
            const string expected = "foo\\bar\\Content\\Mods\\GalacticWaez\\stardata.csv";
            Assert.AreEqual(expected, new FileDataSource("foo\\bar", null).PathToFile);
        }

        [TestMethod]
        public void GetGalaxyData_LoadsExpectedData()
        {
            File.Copy(tc.DeploymentDirectory + "\\stardata-test-small.csv",
                tc.DeploymentDirectory + "\\Content\\Mods\\GalacticWaez\\stardata.csv",
                overwrite: true);
            var expected = GalaxyTestData.LoadPositions("stardata-test-small.csv");
            var actual = new FileDataSource(tc.DeploymentDirectory, delegate { }).GetGalaxyData();
            Assert.AreEqual(expected.Count(), actual.Count());
            Assert.IsFalse(expected.Except(actual).Any());
        }

        [TestMethod]
        public void GetGalaxyData_ReturnsNull_WhenFileNotExists()
        {
            Assert.IsNull(new FileDataSource("foo\\bar", delegate { }).GetGalaxyData());
        }

        [TestMethod]
        public void GetGalaxyData_ReturnsNull_WhenChecksumFail()
        {
            File.Copy(tc.DeploymentDirectory + "\\stardata-test-badsum.csv",
                tc.DeploymentDirectory + "\\Content\\Mods\\GalacticWaez\\stardata.csv",
                overwrite: true);
            Assert.IsNull(new FileDataSource(tc.DeploymentDirectory, delegate { }).GetGalaxyData());
        }

        [TestMethod]
        public void GetGalaxyData_ReturnsExpected_WhenLogNull()
        {
            File.Copy(tc.DeploymentDirectory + "\\stardata-test-small.csv",
                tc.DeploymentDirectory + "\\Content\\Mods\\GalacticWaez\\stardata.csv",
                overwrite: true);
            var expected = GalaxyTestData.LoadPositions("stardata-test-small.csv");
            var actual = new FileDataSource(tc.DeploymentDirectory, null).GetGalaxyData();
            Assert.AreEqual(expected.Count(), actual.Count());
            Assert.IsFalse(expected.Except(actual).Any());
        }

        [TestMethod]
        public void StoreGalaxyData_StoresExpectedData()
        {
            string testDir = tc.DeploymentDirectory + "\\StoreGalaxyData";
            var expected = GalaxyTestData.LoadPositions("stardata-test-small.csv");
            var fds = new FileDataSource(testDir, null);
            fds.StoreGalaxyData(expected);
            var actual = GalaxyTestData.LoadPositions(fds.PathToFile);
            Assert.AreEqual(expected.Count, actual.Count);
            Assert.IsFalse(expected.Except(actual).Any());
        }

        [TestMethod]
        public void StoreGalaxyData_OverwritesExisting()
        {
            string testDir = tc.DeploymentDirectory + "\\StoreGalaxyData";
            var expected = GalaxyTestData.LoadPositions("stardata-test-small.csv");
            var fds = new FileDataSource(testDir, null);
            Directory.CreateDirectory(Directory.GetParent(fds.PathToFile).FullName);
            File.Copy("stardata-test-badsum.csv", fds.PathToFile);
            fds.StoreGalaxyData(expected);
            var actual = GalaxyTestData.LoadPositions(fds.PathToFile);
            Assert.AreEqual(expected.Count, actual.Count);
            Assert.IsFalse(expected.Except(actual).Any());
        }
    }
}
