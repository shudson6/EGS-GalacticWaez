using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using GalacticWaez;

namespace GalacticWaezTests
{
    [TestClass]
    [DeploymentItem("Dependencies\\stardata-test-small.csv")]
    public class FileDataSourceTests
    {
        [TestMethod]
        public void PathToFile_Correct()
        {
            const string expected = "foo\\bar\\Content\\Mods\\GalacticWaez\\stardata.csv";
            Assert.AreEqual(expected, new FileDataSource("foo\\bar", null).PathToFile);
        }

        [TestMethod]
        public void ReturnsNull_WhenFileNotExists()
        {
            Assert.IsNull(new FileDataSource("foo\\bar", (_) => { }).GetGalaxyData());
        }
    }
}
