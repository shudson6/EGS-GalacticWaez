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
        private static string BaseDir;
        private static string ContentDir;

        [ClassInitialize]
        public static void SetupClass(TestContext tc)
        {
            GoodData = GalaxyTestData.LoadPositions(tc.DeploymentDirectory + "\\stardata-test-small.csv");
            BaseDir = tc.DeploymentDirectory;
            ContentDir = tc.DeploymentDirectory + "\\" + StarDataStorage.DefaultContentDir;
            Directory.CreateDirectory(ContentDir);
            File.Copy(tc.DeploymentDirectory + "\\stardata-test-small.csv", 
                ContentDir + "\\stardata-test-small.csv");
            File.Copy(tc.DeploymentDirectory + "\\stardata-test-badsum.csv", 
                ContentDir + "\\stardata-test-badsum.csv");
        }

        [TestMethod]
        public void Exists_True()
        {
            Assert.IsTrue(new StarDataStorage(BaseDir, "stardata-test-small.csv").Exists());
        }

        [TestMethod]
        public void Exists_False()
        {
            Assert.IsFalse(new StarDataStorage(BaseDir, "iDontExist").Exists());
        }

        [TestMethod]
        public void Load_Success()
        {
            var loaded = new StarDataStorage(BaseDir, "stardata-test-small.csv").Load();
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
            Assert.IsNull(new StarDataStorage(BaseDir, "idontexist.foo").Load());
        }

        [TestMethod]
        public void Load_BadSum()
        {
            Assert.IsNull(new StarDataStorage(BaseDir, "stardata-test-badsum.csv").Load());
        }

        [TestMethod]
        public void Store_Success()
        {
            new StarDataStorage(BaseDir, "stardata.csv").Store(GoodData);
            // verify it
            var stored = new StarDataStorage(BaseDir, "stardata.csv").Load();
            Assert.AreEqual(GoodData.Count, stored.Count());
            int i = 0;
            foreach (var p in stored)
            {
                Assert.AreEqual(GoodData[i], p);
                i++;
            }
        }

        [TestMethod]
        public void CheckCustomizedPaths()
        {
            var s = new StarDataStorage("foo\\bar", "test");
            Assert.AreEqual($"foo\\bar\\{StarDataStorage.DefaultContentDir}\\test", s.FilePath);
            Assert.AreEqual($"foo\\bar\\{StarDataStorage.DefaultContentDir}", s.DirectoryPath);
            Assert.AreEqual("test", s.FileName);
        }

        [TestMethod]
        public void CheckDefaultPaths()
        {
            var s = new StarDataStorage("");
            Assert.AreEqual("Content\\Mods\\GalacticWaez\\stardata.csv", s.FilePath);
            Assert.AreEqual("Content\\Mods\\GalacticWaez", s.DirectoryPath);
            Assert.AreEqual("stardata.csv", s.FileName);
        }
    }
}
