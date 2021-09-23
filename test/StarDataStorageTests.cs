using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using GalacticWaez;
using SectorCoordinates = Eleon.Modding.VectorInt3;

namespace GalacticWaezTests
{
    [TestClass]
    [DeploymentItem("Dependencies\\stardata-test-small.csv")]
    [DeploymentItem("Dependencies\\stardata-test-badsum.csv")]
    public class StarDataStorageTests
    {
        private static IReadOnlyList<SectorCoordinates> GoodData;
        private static string DirectoryPath;

        [ClassInitialize]
        public static void SetupClass(TestContext tc)
        {
            GoodData = GalaxyTestData.LoadPositions(tc.DeploymentDirectory + "\\stardata-test-small.csv");
            DirectoryPath = tc.DeploymentDirectory + "\\" + StarDataStorage.DefaultContentDir;
            Directory.CreateDirectory(DirectoryPath);
            File.Copy(tc.DeploymentDirectory + "\\stardata-test-small.csv", 
                DirectoryPath + "\\stardata-test-small.csv");
            File.Copy(tc.DeploymentDirectory + "\\stardata-test-badsum.csv", 
                DirectoryPath + "\\stardata-test-badsum.csv");
        }

        [TestMethod]
        public void Exists_True()
        {
            Assert.IsTrue(new StarDataStorage(DirectoryPath, "stardata-test-small.csv").Exists());
        }

        [TestMethod]
        public void Exists_False()
        {
            Assert.IsFalse(new StarDataStorage(DirectoryPath, "iDontExist").Exists());
        }

        [TestMethod]
        public void Load_Success()
        {
            var loaded = new StarDataStorage(DirectoryPath, "stardata-test-small.csv").Load();
            Assert.AreEqual(GoodData.Count, loaded.Count());
            int i = 0;
            foreach (var p in loaded)
            {
                Assert.AreEqual(GoodData[i], p);
                i++;
            }
        }

        [TestMethod]
        public void Load_NotFound()
        {
            Assert.IsNull(new StarDataStorage(DirectoryPath, "idontexist.foo").Load());
        }

        [TestMethod]
        public void Load_BadSum()
        {
            Assert.IsNull(new StarDataStorage(DirectoryPath, "stardata-test-badsum.csv").Load());
        }

        [TestMethod]
        public void Store_Success()
        {
            new StarDataStorage(DirectoryPath, "stardata.csv").Store(GoodData);
            // verify it
            var stored = new StarDataStorage(DirectoryPath, "stardata.csv").Load();
            Assert.AreEqual(GoodData.Count, stored.Count());
            int i = 0;
            foreach (var p in stored)
            {
                Assert.AreEqual(GoodData[i], p);
                i++;
            }
        }
    }
}
