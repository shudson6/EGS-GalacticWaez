using Microsoft.VisualStudio.TestTools.UnitTesting;
using GalacticWaez;

namespace GalacticWaezTests
{
    [TestClass]
    [DeploymentItem("stardata-test-small.csv", ".\\Content\\Mods\\GalacticWaez")]
    public class InitializerTests
    {
        private static TestContext tc;

        [ClassInitialize]
        public static void SetupClass(TestContext _tc)
        {
            tc = _tc;
        }

        [TestMethod]
        [Timeout(500)]
        public void FileNotFound()
        {
            var modApi = new FakeModApi(new FakeApplication(tc.DeploymentDirectory));
            var cmd = new Initializer(modApi);
            bool done = false;
            cmd.Initialize(Initializer.Source.File,
                (galaxy, ex) =>
                {
                    Assert.IsNull(galaxy);
                    Assert.IsTrue(modApi.LogContains("Stored star data not found."));
                    done = true;
                });
            while (!done) ;
        }
    }
}
