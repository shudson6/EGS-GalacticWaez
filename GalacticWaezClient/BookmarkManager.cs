using Eleon.Modding;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Text;

namespace GalacticWaez
{
    public class BookmarkManager :  SaveGameDB, IBookmarkManager
    {
        private readonly LoggingDelegate Log;

        public BookmarkManager(string saveGameDir, LoggingDelegate log) 
            : base(saveGameDir) { Log = log ?? delegate { }; }

        public bool TryGetVector(int playerId, int playerFacId, string bookmarkName, out VectorInt3 coordinates)
        {
            if (bookmarkName == null)
                bookmarkName = "";

            SQLiteConnection connection = null;
            SQLiteCommand command = null;
            IDataReader reader = null;

            try
            {
                connection = GetConnection();
                command = connection.CreateCommand();
                command.CommandText = "select sectorx, sectory, sectorz from Bookmarks "
                        + $"where type='0' and name='{bookmarkName}'"
                        + $" and (entityid='{playerId}'"
                        + $" or (facid='{playerFacId}' and isshared='1'));";
                reader = command.ExecuteReader();
                if (reader.Read())
                {
                    coordinates = new VectorInt3(
                        reader.GetInt32(0),
                        reader.GetInt32(1),
                        reader.GetInt32(2)
                    );
                    return true;
                }
            }
            catch (SQLiteException ex)
            {
                Log($"SQLiteException in GetBookmarkVector: {ex.Message}");
            }
            finally
            {
                reader?.Dispose();
                command?.Dispose();
                connection?.Dispose();
            }
            coordinates = default;
            return false;
        }

        public int InsertBookmarks(IEnumerable<VectorInt3> coordinates, BookmarkData data)
        {
            if (coordinates == null)
                return 0;

            SQLiteConnection connection = null;
            SQLiteCommand command = null;
            try
            {
                connection = GetConnection(writeable: true);
                command = connection.CreateCommand();
                //int bid = GetStartingBookmarkId(command);
                command.CommandText = BuildInsertBookmarks(coordinates, data, 0);// bid);
                return command.ExecuteNonQuery();
            }
            catch (SQLiteException ex)
            {
                Log("SQLiteException in InsertBookmarks: " + ex.Message);
            }
            finally
            {
                command?.Dispose();
                connection?.Dispose();
            }
            return 0;
        }

        private string BuildInsertBookmarks(IEnumerable<VectorInt3> coordinates, BookmarkData data, int bid0)
        {
            // removed bid from insert to see if it would work better
            var sql = new StringBuilder(
                "insert into Bookmarks "
                + "('type', 'refid', 'facgroup', 'facid', 'entityid', 'name',"
                + "'sectorx', 'sectory', 'sectorz', 'posx', 'posy', 'posz',"
                + "'icon', 'isshared', 'iswaypoint', 'isremove', 'isshowhud', 'iscallback',"
                + "'createdticks', 'maxdistance') "
                + "values "
                );
            int bid = bid0;
            int step = 1;
            foreach (var p in coordinates)
            {
                sql.Append($"(0, 0, {data.FacGroup}, {data.PlayerFacId}, {data.PlayerId},");
                sql.Append($"'Waez_{step}', {p.x}, {p.y}, {p.z}, 0, 0, 0,");
                sql.Append($"{data.Icon}, {data.IsShared}, {data.IsWaypoint}, {data.IsRemove},");
                sql.Append($"{data.IsShowHud}, 0, {data.GameTime}, {data.MaxDistance}),");
                step++;
                bid++;
            }
            sql.Replace(',', ';', sql.Length - 1, 1);
            return sql.ToString();
        }

        //private int GetStartingBookmarkId(SQLiteCommand command)
        //{
        //    IDataReader reader = null;
        //    try
        //    {
        //        command.CommandText = "select coalesce(max(bid), 1) from Bookmarks;";
        //        reader = command.ExecuteReader();
        //        return reader.Read() ? reader.GetInt32(0) + 1 : 1;
        //    }
        //    finally
        //    {
        //        reader?.Dispose();
        //    }
        //}

        public int ModifyPathMarkers(int playerId, string action)
        {
            string sql;
            switch (action)
            {
                case "clear":
                    sql = "delete from Bookmarks "
                        + $"where entityid='{playerId}' and name like 'Waez\\_%' escape '\\';";
                    break;
                case "hide":
                    sql = "update Bookmarks set isshowhud = 0, maxdistance = 0 "
                        + $"where entityid='{playerId}' and name like 'Waez\\_%' escape '\\';";
                    break;
                case "show":
                    sql = "update Bookmarks set isshowhud = 1, maxdistance = -1 "
                        + $"where entityid ='{playerId}' and name like 'Waez\\_%' escape '\\';";
                    break;
                default:
                    Log($"Invalid Command 'bookmarks {action}', use clear|hide|show");
                    return 0;
            }

            SQLiteConnection connection = null;
            SQLiteCommand command = null;

            try
            {
                connection = GetConnection(writeable: true);
                command = connection.CreateCommand();
                command.CommandText = sql;
                return command.ExecuteNonQuery();
            }
            catch (SQLiteException ex)
            {
                Log($"SQLiteException in ModifyPathMarkers: {ex.Message}");
                return 0;
            }
            finally
            {
                command?.Dispose();
                connection?.Dispose();
            }
        }
    }
}
