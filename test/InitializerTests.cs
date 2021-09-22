using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Linq;
using System.Threading;
using Mono.Data.Sqlite;
using GalacticWaez;

namespace GalacticWaezTests
{
    [TestClass]
    [DeploymentItem("Dependencies\\sqlite3.dll")]
    [DeploymentItem("Dependencies\\testdb.sql")]
    [DeploymentItem("Dependencies\\stardata-test-small.csv")]
    public class InitializerTests
    {
        private static TestContext tc;
        private static string TestDbFile;

        [ClassInitialize]
        public static void SetupClass(TestContext _tc)
        {
            tc = _tc;
            TestDbFile = tc.DeploymentDirectory + $"\\global.db";
            string createDb = File.ReadAllText(tc.DeploymentDirectory + $"\\testdb.sql");
            string cs = $"Data Source={TestDbFile};Mode=ReadWrite";
            SqliteConnection.CreateFile(TestDbFile);
            var sql = new SqliteConnection();
            sql.ConnectionString = cs;
            sql.Open();
            var cmd = sql.CreateCommand();
            cmd.CommandText = createDb;
            cmd.ExecuteNonQuery();

            var reader = new StreamReader(new FileStream(
                tc.DeploymentDirectory + "\\stardata-test-small.csv",
                FileMode.Open, FileAccess.Read
                ));
            reader.ReadLine();
            string line = reader.ReadLine();
            var coords = line.Split(',');
            cmd.CommandText = "insert into SolarSystems "
                + $"values (1, 'Foo', 'G', {coords[0]}, {coords[1]}, {coords[2]});";
            cmd.ExecuteNonQuery();

            cmd.Dispose();
            sql.Dispose();
        }

        [TestMethod]
        [Timeout(500)]
        public void FileNotFound()
        {
            var fakeApp = new FakeApplication(tc.DeploymentDirectory);
            var modApi = new FakeModApi(fakeApp);
            var init = new Initializer(modApi);
            bool done = false;
            init.Initialize(Initializer.Source.File,
                (galaxy, ex) =>
                {
                    Assert.IsNull(galaxy);
                    Assert.IsTrue(modApi.LogContains("Stored star data not found."));
                    done = true;
                });
            while (!done)
            {
                Thread.Sleep(20);
                fakeApp.FireUpdate();
            }
        }

        [TestMethod]
        [Timeout(3000)]
        public void NoDataInMemory()
        {
            var fakeApp = new FakeApplication(tc.DeploymentDirectory);
            var modApi = new FakeModApi(fakeApp);
            var init = new Initializer(modApi);
            bool done = false;
            init.Initialize(Initializer.Source.Scanner,
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
    }
}
