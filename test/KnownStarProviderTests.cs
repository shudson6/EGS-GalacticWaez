using Microsoft.VisualStudio.TestTools.UnitTesting;
using Mono.Data.Sqlite;
using System;
using System.IO;
using Eleon.Modding;
using GalacticWaez;

namespace GalacticWaezTests
{
    [TestClass]
    [DeploymentItem("Dependencies\\testdb.sql")]
    [DeploymentItem("Dependencies\\sqlite3.dll")]
    public class KnownStarProviderTests
    {
        private static string dbPath;
        private static string sqlPath;
        private static string deploymentDir;

        [ClassInitialize]
        public static void SetupClass(TestContext _tc)
        {
            deploymentDir = _tc.DeploymentDirectory;
            dbPath = $"{deploymentDir}\\global.db";
            SqliteConnection.CreateFile(dbPath);
            sqlPath = $"{deploymentDir}\\testdb.sql";
        }

        [TestInitialize]
        public void SetupTest()
        {
            var sqlite = GetConnection(true);
            var command = sqlite.CreateCommand();
            command.CommandText = File.ReadAllText(sqlPath);
            command.ExecuteNonQuery();
            sqlite.Dispose();
        }

        [TestMethod]
        public void Has_Correct_PathToDB()
        {
            string expected = deploymentDir + "\\global.db";
            string actual = new KnownStarProvider(deploymentDir, delegate { }).PathToDB;
            Assert.AreEqual(expected, actual);
        }

        private static SqliteConnection GetConnection(bool writeable)
        {
            var sql = new SqliteConnection(new SqliteConnectionStringBuilder
            {
                DataSource = dbPath,
                Version = 3,
                ReadOnly = !writeable
            }.ToString());
            sql.Open();
            return sql;
        }

    }
}
