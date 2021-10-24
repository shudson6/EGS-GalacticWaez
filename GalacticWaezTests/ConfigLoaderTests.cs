using GalacticWaez;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using static GalacticWaez.GalacticWaez;

namespace GalacticWaezTests
{
    [TestClass]
    [DeploymentItem("Dependencies\\testconfig.ecf")]
    [DeploymentItem("Dependencies\\testconfig-nomaxrange.ecf")]
    [DeploymentItem("Dependencies\\testconfig-nobaserange.ecf")]
    [DeploymentItem("Dependencies\\testconfig-notimeout.ecf")]
    public class ConfigLoaderTests
    {
        static string dir;

        [ClassInitialize]
        public static void SetupClass(TestContext tc)
        {
            dir = tc.DeploymentDirectory;
        }

        [TestMethod]
        public void LoadConfig_Happy()
        {
            var config = new ConfigLoader(null).LoadConfig(dir + @"\testconfig.ecf");
            Assert.AreEqual(7100000, config.MaxWarpRange);
            Assert.AreEqual(11000, config.NavTimeoutMillis);
            Assert.AreEqual(3700000, config.BaseWarpRange);
        }

        [TestMethod]
        public void LoadConfig_ReturnDefault_FileNotExists()
        {
            var config = new ConfigLoader(null).LoadConfig(dir + @"\nope");
            Assert.AreEqual(DefaultMaxWarpRange, config.MaxWarpRange);
            Assert.AreEqual(DefaultBaseWarpRange, config.BaseWarpRange);
            Assert.AreEqual(DefaultNavTimeoutMillis, config.NavTimeoutMillis);
        }

        [TestMethod]
        public void LoadConfig_ReturnDefault_NullFilename()
        {
            var config = new ConfigLoader(null).LoadConfig(null);
            Assert.AreEqual(DefaultMaxWarpRange, config.MaxWarpRange);
            Assert.AreEqual(DefaultBaseWarpRange, config.BaseWarpRange);
            Assert.AreEqual(DefaultNavTimeoutMillis, config.NavTimeoutMillis);
        }

        [TestMethod]
        public void LoadConfig_DefaultMaxRange_IfNotPresent()
        {
            var config = new ConfigLoader(null).LoadConfig(dir + @"\testconfig-nomaxrange.ecf");
            Assert.AreEqual(DefaultMaxWarpRange, config.MaxWarpRange);
            Assert.AreEqual(3700000, config.BaseWarpRange);
            Assert.AreEqual(11000, config.NavTimeoutMillis);
        }

        [TestMethod]
        public void LoadConfig_DefaultBaseRange_IfNotPresent()
        {
            var config = new ConfigLoader(null).LoadConfig(dir + @"\testconfig-nobaserange.ecf");
            Assert.AreEqual(7100000, config.MaxWarpRange);
            Assert.AreEqual(DefaultBaseWarpRange, config.BaseWarpRange);
            Assert.AreEqual(11000, config.NavTimeoutMillis);
        }

        [TestMethod]
        public void LoadConfig_DefaultNavTimeout_IfNotPresent()
        {
            var config = new ConfigLoader(null).LoadConfig(dir + @"\testconfig-notimeout.ecf");
            Assert.AreEqual(7100000, config.MaxWarpRange);
            Assert.AreEqual(3700000, config.BaseWarpRange);
            Assert.AreEqual(DefaultNavTimeoutMillis, config.NavTimeoutMillis);
        }
    }
}
