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
        public void PathToFile_Correct()
        {
            const string expected = "foo\\bar\\Content\\Mods\\GalacticWaez\\stardata.csv";
            Assert.AreEqual(expected, new FileDataSource("foo\\bar", null).PathToFile);
        }

        [TestMethod]
        public void LoadsExpectedData()
        {
            File.Copy(tc.DeploymentDirectory + "\\stardata-test-small.csv",
                tc.DeploymentDirectory + "\\Content\\Mods\\GalacticWaez\\stardata.csv",
                overwrite: true);
            var expected = GalaxyTestData.LoadPositions("stardata-test-small.csv");
            var actual = new FileDataSource(tc.DeploymentDirectory, (_) => { }).GetGalaxyData();
            Assert.AreEqual(expected.Count(), actual.Count());
            Assert.IsFalse(expected.Except(actual).Any());
        }

        [TestMethod]
        public void ReturnsNull_WhenFileNotExists()
        {
            Assert.IsNull(new FileDataSource("foo\\bar", (_) => { }).GetGalaxyData());
        }

        [TestMethod]
        public void ReturnsNull_WhenChecksumFail()
        {
            File.Copy(tc.DeploymentDirectory + "\\stardata-test-badsum.csv",
                tc.DeploymentDirectory + "\\Content\\Mods\\GalacticWaez\\stardata.csv",
                overwrite: true);
            Assert.IsNull(new FileDataSource(tc.DeploymentDirectory, (_) => { }).GetGalaxyData());
        }
    }
}
