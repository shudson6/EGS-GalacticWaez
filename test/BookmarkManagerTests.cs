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
    public class BookmarkManagerTests
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
        public void CanWriteToDB()
        {
            var sql = GetConnection(true);
            var cmd = sql.CreateCommand();
            cmd.CommandText = "begin transaction;";
            cmd.CommandText += "insert into SolarSystems values (1, 'Foo', 'G', 1, 2, 3);";
            cmd.CommandText += "commit;";
            cmd.ExecuteNonQuery();
            cmd.CommandText = "select name from SolarSystems;";
            var reader = cmd.ExecuteReader();
            Assert.IsTrue(reader.Read());
            Assert.AreEqual("Foo", reader.GetString(0));
            reader.Close();
            cmd.Dispose();
            sql.Dispose();
        }

        [TestMethod]
        public void FalseAndDefault_WhenNotFound()
        {
            // we have a nice, empty table
            var bm = new BookmarkManager(deploymentDir, delegate { });
            Assert.IsFalse(bm.TryGetVector(1337, 1337, "idontexist", out VectorInt3 vector));
            Assert.AreEqual(default(VectorInt3), vector);
        }

        [TestMethod]
        public void HappyDay()
        {
            var sql = GetConnection(true);
            var cmd = sql.CreateCommand();
            var expected = new VectorInt3(300000, 400000, 500000);
            cmd.CommandText = "insert into Bookmarks"
                + " values (1, 0, 0, 1, 1337, 1337, 42, 'Test Star',"
                + $"{expected.x}, {expected.y}, {expected.z}, 0, 0, 0,"
                + " 2, 0, 0, 0, 1, 0, 131071, 0, 0, -1);";
            cmd.ExecuteNonQuery();
            cmd.Dispose();
            sql.Dispose();

            var bm = new BookmarkManager(deploymentDir, delegate { });
            Assert.IsTrue(bm.TryGetVector(1337, 1337, "Test Star", out VectorInt3 actual));
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void NullLogger_NoProblem()
        {
            var bm = new BookmarkManager(deploymentDir, null);
            // not under test, just making sure it doesn't throw
            bm.TryGetVector(0, 0, "hi", out var _);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Throws_IfSaveGameDirNull()
        {
            var _ = new BookmarkManager(null, delegate { });
        }

        [TestMethod]
        public void Succeed_IfFactionAndShared()
        {
            var sql = GetConnection(true);
            var cmd = sql.CreateCommand();
            var expected = new VectorInt3(300000, 400000, 500000);
            cmd.CommandText = "insert into Bookmarks"
                + " values (1, 0, 0, 1, 1337, 418, 42, 'Test Star',"
                + $"{expected.x}, {expected.y}, {expected.z}, 0, 0, 0,"
                // isshared is the second entry on this row
                + " 2, 1, 0, 0, 1, 0, 131071, 0, 0, -1);";
            cmd.ExecuteNonQuery();
            cmd.Dispose();
            sql.Dispose();

            var bm = new BookmarkManager(deploymentDir, delegate { });
            Assert.IsTrue(bm.TryGetVector(1337, 1337, "Test Star", out VectorInt3 actual));
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void Fail_IfFactionButNotShared()
        {
            var sql = GetConnection(true);
            var cmd = sql.CreateCommand();
            var vector = new VectorInt3(300000, 400000, 500000);
            cmd.CommandText = "insert into Bookmarks"
                + " values (1, 0, 0, 1, 1337, 418, 42, 'Test Star',"
                + $"{vector.x}, {vector.y}, {vector.z}, 0, 0, 0,"
                // isshared is the second entry on this row
                + " 2, 0, 0, 0, 1, 0, 131071, 0, 0, -1);";
            cmd.ExecuteNonQuery();
            cmd.Dispose();
            sql.Dispose();

            var bm = new BookmarkManager(deploymentDir, delegate { });
            Assert.IsFalse(bm.TryGetVector(1337, 1337, "Test Star", out VectorInt3 actual));
            Assert.AreEqual(default(VectorInt3), actual);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Throw_IfBookmarkNameNull()
        {
            var bm = new BookmarkManager(deploymentDir, delegate { });
            bm.TryGetVector(0, 0, null, out var _);
        }

        [TestMethod]
        public void Fail_IfPlayerNorFactionMatch()
        {
            var sql = GetConnection(true);
            var cmd = sql.CreateCommand();
            var vector = new VectorInt3(300000, 400000, 500000);
            cmd.CommandText = "insert into Bookmarks"
                + " values (1, 0, 0, 1, 1337, 418, 42, 'Test Star',"
                + $"{vector.x}, {vector.y}, {vector.z}, 0, 0, 0,"
                + " 2, 0, 0, 0, 1, 0, 131071, 0, 0, -1);";
            cmd.ExecuteNonQuery();
            cmd.Dispose();
            sql.Dispose();

            var bm = new BookmarkManager(deploymentDir, delegate { });
            Assert.IsFalse(bm.TryGetVector(404, 404, "Test Star", out VectorInt3 actual));
            Assert.AreEqual(default(VectorInt3), actual);
        }

        [TestMethod]
        public void Fail_IfPlayerNorFactionMatch_EvenIfShared()
        {
            var sql = GetConnection(true);
            var cmd = sql.CreateCommand();
            var vector = new VectorInt3(300000, 400000, 500000);
            cmd.CommandText = "insert into Bookmarks"
                + " values (1, 0, 0, 1, 1337, 418, 42, 'Test Star',"
                + $"{vector.x}, {vector.y}, {vector.z}, 0, 0, 0,"
                // isshared is the second entry on this row
                + " 2, 1, 0, 0, 1, 0, 131071, 0, 0, -1);";
            cmd.ExecuteNonQuery();
            cmd.Dispose();
            sql.Dispose();

            var bm = new BookmarkManager(deploymentDir, delegate { });
            Assert.IsFalse(bm.TryGetVector(404, 404, "Test Star", out VectorInt3 actual));
            Assert.AreEqual(default(VectorInt3), actual);
        }

        [TestMethod]
        public void Succeed_IfPlayerButNotFaction()
        {
            // i don't actually think this can happen
            var sql = GetConnection(true);
            var cmd = sql.CreateCommand();
            var expected = new VectorInt3(300000, 400000, 500000);
            cmd.CommandText = "insert into Bookmarks"
                + " values (1, 0, 0, 1, 418, 1337, 42, 'Test Star',"
                + $"{expected.x}, {expected.y}, {expected.z}, 0, 0, 0,"
                + " 2, 1, 0, 0, 1, 0, 131071, 0, 0, -1);";
            cmd.ExecuteNonQuery();
            cmd.Dispose();
            sql.Dispose();

            var bm = new BookmarkManager(deploymentDir, delegate { });
            Assert.IsTrue(bm.TryGetVector(1337, 1337, "Test Star", out VectorInt3 actual));
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
