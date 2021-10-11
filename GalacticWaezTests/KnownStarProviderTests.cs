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
        private static readonly VectorInt3 firstVector = new VectorInt3(300000, 0, 0);
        private static readonly VectorInt3 secondVector = new VectorInt3(400000, 0, 0);
        private static readonly VectorInt3 thirdVector = new VectorInt3(500000, 0, 0);
        private static readonly VectorInt3 fourthVector = new VectorInt3(600000, 0, 0);
        private static readonly VectorInt3 fifthVector = new VectorInt3(700000, 0, 0);

        private static string dbPath;
        private static string sqlPath;
        private static string deploymentDir;

        [ClassInitialize]
        public static void SetupClass(TestContext _tc)
        {
            deploymentDir = $"{_tc.DeploymentDirectory}\\KnownStarProvider";
            Directory.CreateDirectory(deploymentDir);
            dbPath = $"{deploymentDir}\\global.db";
            SqliteConnection.CreateFile(dbPath);
            sqlPath = $"{_tc.DeploymentDirectory}\\testdb.sql";
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

        [TestMethod]
        public void GetFirstKnownStarPosition_ReturnsOne_AndItsTheFirst()
        {
            Insert5Stars();

            var ksp = new KnownStarProvider(deploymentDir, delegate { });
            Assert.IsTrue(ksp.GetFirstKnownStarPosition(out VectorInt3 actual));
            var expected = firstVector;
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void GetFirstKnownStarPosition_FalseAndDefault_WhenTableEmpty()
        {
            var ksp = new KnownStarProvider(deploymentDir, delegate { });
            Assert.IsFalse(ksp.GetFirstKnownStarPosition(out VectorInt3 actual));
            Assert.AreEqual(default, actual);
        }

        [TestMethod]
        public void GetPosition_HappyDay()
        {
            Insert5Stars();

            var ksp = new KnownStarProvider(deploymentDir, delegate { });
            Assert.IsTrue(ksp.GetPosition("Third", out VectorInt3 actual));
            var expected = thirdVector;
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void GetPosition_FalseAndDefault_NotFound()
        {
            Insert5Stars();

            var ksp = new KnownStarProvider(deploymentDir, delegate { });
            Assert.IsFalse(ksp.GetPosition("Eleventh", out VectorInt3 actual));
            Assert.AreEqual(default, actual);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void GetPosition_Throw_NullStarName()
        {
            new KnownStarProvider(deploymentDir, delegate { }).GetPosition(null, out VectorInt3 actual);
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

        private static void Insert5Stars()
        {
            var sql = GetConnection(true);
            var cmd = sql.CreateCommand();
            cmd.CommandText = "insert into SolarSystems "
                + "values "
                + $"(1, 'First', 'Test', {firstVector.x}, {firstVector.y}, {firstVector.z}),"
                + $"(2, 'Second', 'Test', {secondVector.x}, {secondVector.y}, {secondVector.z}),"
                + $"(3, 'Third', 'Test', {thirdVector.x}, {thirdVector.y}, {thirdVector.z}),"
                + $"(4, 'Fourth', 'Test', {fourthVector.x}, {fourthVector.y}, {fourthVector.z}),"
                + $"(5, 'Fifth', 'Test', {fifthVector.x}, {fifthVector.y}, {fifthVector.z});";
            cmd.ExecuteNonQuery();
            cmd.Dispose();
            sql.Dispose();
        }
    }
}
