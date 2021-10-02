using Microsoft.VisualStudio.TestTools.UnitTesting;
using Mono.Data.Sqlite;
using System;
using System.IO;
using System.Linq;
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
        public void TryGetVector_FalseAndDefault_WhenNotFound()
        {
            // we have a nice, empty table
            var bm = new BookmarkManager(deploymentDir, delegate { });
            Assert.IsFalse(bm.TryGetVector(1337, 1337, "idontexist", out VectorInt3 vector));
            Assert.AreEqual(default(VectorInt3), vector);
        }

        [TestMethod]
        public void TryGetVector_HappyDay()
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
        public void TryGetVector_NullLogger_NoProblem()
        {
            var bm = new BookmarkManager(deploymentDir, null);
            // not under test, just making sure it doesn't throw
            bm.TryGetVector(0, 0, "hi", out var _);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TryGetVector_Throws_IfSaveGameDirNull()
        {
            var _ = new BookmarkManager(null, delegate { });
        }

        [TestMethod]
        public void TryGetVector_Succeed_IfFactionAndShared()
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
        public void TryGetVector_Fail_IfFactionButNotShared()
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
        public void TryGetVector_Throw_IfBookmarkNameNull()
        {
            var bm = new BookmarkManager(deploymentDir, delegate { });
            bm.TryGetVector(0, 0, null, out var _);
        }

        [TestMethod]
        public void TryGetVector_Fail_IfPlayerNorFactionMatch()
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
        public void TryGetVector_Fail_IfPlayerNorFactionMatch_EvenIfShared()
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
        public void TryGetVector_Succeed_IfPlayerButNotFaction()
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

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void InsertBookmarks_ThrowIfCoordinatesNull()
        {
            var bm = new BookmarkManager(deploymentDir, delegate { });
            bm.InsertBookmarks(null, default);
        }

        [TestMethod]
        public void InsertBookmarks_HappyDay1()
        {
            var data = new BookmarkData
            {
                PlayerId = 1337,
                PlayerFacId = 418,
                FacGroup = 1,
                Icon = 2,
                IsShared = false,
                IsWaypoint = false,
                IsRemove = false,
                IsShowHud = false,
                GameTime = 131071,
                MaxDistance = -1
            };
            var path = new[]
            {
                new VectorInt3(300000, 400000, 500000)
            };
            var bm = new BookmarkManager(deploymentDir, delegate { });
            Assert.AreEqual(1, bm.InsertBookmarks(path, data));
            // now verify what was written
            var sql = GetConnection(false);
            var cmd = sql.CreateCommand();
            cmd.CommandText = "select * from Bookmarks;";
            var reader = cmd.ExecuteReader();
            Assert.IsTrue(reader.Read());
            var actual = ExtractBookmark(reader);
            Assert.AreEqual(ExpectedBookmark("Waez_1", path[0], data), actual);
            Assert.IsFalse(reader.Read());
            cmd.Dispose();
            sql.Dispose();
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void InsertBookmarks_Throws_CoordinatesIsNull()
        {
            var bm = new BookmarkManager(deploymentDir, delegate { });
            bm.InsertBookmarks(null, default);
        }

        [TestMethod]
        public void InsertBookmarks_HappyDay3x()
        {
            var data = new BookmarkData
            {
                PlayerId = 1337,
                PlayerFacId = 418,
                FacGroup = 1,
                Icon = 2,
                IsShared = false,
                IsWaypoint = false,
                IsRemove = false,
                IsShowHud = false,
                GameTime = 131071,
                MaxDistance = -1
            };
            var path = new[]
            {
                new VectorInt3(300000, 400000, 500000),
                new VectorInt3(600000, 800000, 1000000),
                new VectorInt3(900000, 1200000, 1500000)
            };
            var bm = new BookmarkManager(deploymentDir, delegate { });
            Assert.AreEqual(3, bm.InsertBookmarks(path, data));
            // now verify what was written
            var sql = GetConnection(false);
            var cmd = sql.CreateCommand();
            cmd.CommandText = "select * from Bookmarks;";
            var reader = cmd.ExecuteReader();
            var expected = new Bookmark[3];
            var actual = new Bookmark[3];
            for (int i = 0; i < 3; i++)
            {
                expected[i] = ExpectedBookmark($"Waez_{i + 1}", path[i], data);
                Assert.IsTrue(reader.Read());
                actual[i] = ExtractBookmark(reader);
            }
            Assert.IsFalse(reader.Read());
            Assert.IsFalse(expected.Except(actual).Any());
            cmd.Dispose();
            sql.Dispose();
        }

        private Bookmark ExtractBookmark(SqliteDataReader reader)
        {
            return new Bookmark
            {
                type = reader.GetInt32(reader.GetOrdinal("type")),
                refid = reader.GetInt32(reader.GetOrdinal("refid")),
                facgroup = reader.GetInt32(reader.GetOrdinal("facgroup")),
                facid = reader.GetInt32(reader.GetOrdinal("facid")),
                entityid = reader.GetInt32(reader.GetOrdinal("entityid")),
                pfidIsNull = reader.IsDBNull(reader.GetOrdinal("pfid")),
                name = reader.GetString(reader.GetOrdinal("name")),
                sectorx = reader.GetInt32(reader.GetOrdinal("sectorx")),
                sectory = reader.GetInt32(reader.GetOrdinal("sectory")),
                sectorz = reader.GetInt32(reader.GetOrdinal("sectorz")),
                posx = reader.GetFloat(reader.GetOrdinal("posx")),
                posy = reader.GetFloat(reader.GetOrdinal("posy")),
                posz = reader.GetFloat(reader.GetOrdinal("posz")),
                icon = reader.GetInt32(reader.GetOrdinal("icon")),
                isshared = reader.GetBoolean(reader.GetOrdinal("isshared")),
                iswaypoint = reader.GetBoolean(reader.GetOrdinal("iswaypoint")),
                isremove = reader.GetBoolean(reader.GetOrdinal("isremove")),
                isshowhud = reader.GetBoolean(reader.GetOrdinal("isshowhud")),
                createdticks = (ulong)reader.GetInt64(reader.GetOrdinal("createdticks")),
                expireafterticks = (ulong)reader.GetInt64(reader.GetOrdinal("expireafterticks")),
                mindistance = reader.GetInt32(reader.GetOrdinal("mindistance")),
                maxdistance = reader.GetInt32(reader.GetOrdinal("maxdistance"))
            };
        }

        private Bookmark ExpectedBookmark(string name, VectorInt3 pos, BookmarkData data)
        {
            return new Bookmark
            {
                type = 0,
                refid = 0,
                facgroup = data.FacGroup,
                facid = data.PlayerFacId,
                entityid = data.PlayerId,
                pfidIsNull = true,
                name = name,
                sectorx = pos.x,
                sectory = pos.y,
                sectorz = pos.z,
                posx = 0,
                posy = 0,
                posz = 0,
                icon = data.Icon,
                isshared = data.IsShared,
                iswaypoint = data.IsWaypoint,
                isremove = data.IsRemove,
                isshowhud = data.IsShowHud,
                createdticks = data.GameTime,
                expireafterticks = 0,
                mindistance = 0,
                maxdistance = -1
            };
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

        private struct Bookmark
        {
            public int type, refid, facgroup, facid, entityid;
            /// <summary>
            /// The db column is an integer, but we expect always null, so we will verify that.
            /// </summary>
            public bool pfidIsNull;
            public string name;
            public int sectorx, sectory, sectorz;
            public float posx, posy, posz;
            public int icon;
            public bool isshared, iswaypoint, isremove, isshowhud;
            public ulong createdticks, expireafterticks;
            public int mindistance, maxdistance;
        }
    }
}
