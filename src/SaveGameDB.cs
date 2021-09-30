using System.Collections.Generic;
using System.Text;
using Mono.Data.Sqlite;
using System.Data;
using SectorCoordinates = Eleon.Modding.VectorInt3;

namespace GalacticWaez
{
    class SaveGameDB : SaveGameDBBase, ISaveGameDB
    {
        private readonly LoggingDelegate Log;

        public SaveGameDB(string saveGameDir, LoggingDelegate log)
            : base($"{saveGameDir}\\{DbFileName}")
        {
            Log = log;
        }

        public bool GetBookmarkVector(int playerId, string bookmarkName, 
            out SectorCoordinates coordinates)
        {
            SqliteConnection connection = null;
            SqliteCommand command = null;
            IDataReader reader = null;

            try
            {
                connection = GetConnection();
                command = connection.CreateCommand();
                command.CommandText = "select sectorx, sectory, sectorz from Bookmarks "
                        + $"where type='0' and name='{bookmarkName}' and entityid="
                        + $"'{playerId}';";
                reader = command.ExecuteReader();
                if (reader.Read())
                {
                    coordinates = new SectorCoordinates(
                        reader.GetInt32(0),
                        reader.GetInt32(1),
                        reader.GetInt32(2)
                    );
                    return true;
                }
            }
            catch (SqliteException ex)
            {
                Log($"SqliteException in GetBookmarkVector: {ex.Message}");
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

        // returns the number of bookmarks added
        public int InsertBookmarks(IEnumerable<SectorCoordinates> positions, 
            int playerId, ulong gameTime)
        {
            SqliteConnection connection = null;
            SqliteCommand command = null;

            try
            {
                connection = GetConnection(writeable: true);
                command = connection.CreateCommand();
                int bid = GetStartingBookmarkId(command);
                var sql = new StringBuilder(
                    "insert into Bookmarks ('bid','type','refid','facgroup','facid','entityid',"
                    + "'name','sectorx','sectory','sectorz','posx','posy','posz','icon','isshared',"
                    + "'iswaypoint','isremove','isshowhud','iscallback','createdticks',"
                    + "'expireafterticks','mindistance','maxdistance') values "
                    );
                int stepNo = 1;
                foreach (var p in positions)
                {
                    sql.Append($"({bid},0,0,1,");
                    sql.Append($"{playerId},{playerId},");
                    sql.Append($"'Waez_{stepNo}',{p.x},{p.y},{p.z},0,0,0,2,0,1,1,0,0,");
                    sql.Append($"{gameTime},0,0,0),");
                    stepNo++;
                    bid++;
                }
                sql.Replace(',', ';', sql.Length - 1, 1);
                command.CommandText = sql.ToString();
                return command.ExecuteNonQuery();
            }
            catch (SqliteException ex)
            {
                Log($"SqliteException in InsertBookmarks: {ex.Message}");
            }
            finally
            {
                command?.Dispose();
                connection?.Dispose();
            }
            return 0;
        }

        int GetStartingBookmarkId(SqliteCommand command)
        {
            IDataReader reader = null;
            try
            {
                command.CommandText = "select coalesce(max(bid), 1) from Bookmarks;";
                reader = command.ExecuteReader();
                return reader.Read() ? reader.GetInt32(0) + 1 : 1;
            }
            finally
            {
                reader?.Dispose();
            }
        }

        public int ClearPathMarkers(int playerId)
        {
            SqliteConnection connection = null;
            SqliteCommand command = null;

            try
            {
                connection = GetConnection(writeable: true);
                command = connection.CreateCommand();
                command.CommandText = "delete from Bookmarks where "
                    + $"entityid='{playerId}' and name like 'Waez\\_%' escape '\\';";
                return command.ExecuteNonQuery();
            }
            catch (SqliteException ex)
            {
                Log($"SqliteException in ClearPathMarkers: {ex.Message}");
                return 0;
            }
            finally
            {
                command?.Dispose();
                connection?.Dispose();
            }
        }

        public int ModifyPathMarkers(int playerId, string action)
        {
            SqliteConnection connection = null;
            SqliteCommand command = null;

            try
            {
                connection = GetConnection(writeable: true);
                command = connection.CreateCommand();
                switch (action)
                {
                    case "clear":
                        command.CommandText = "delete from Bookmarks "
                            + $"where entityid='{playerId}' and name like 'Waez\\_%' escape '\\';";
                        break;
                    case "hide":
                        command.CommandText = "update Bookmarks set isshowhud = 0, maxdistance = 0 "
                            + $"where entityid='{playerId}' and name like 'Waez\\_%' escape '\\';";
                        break;
                    case "show":
                        command.CommandText = "update Bookmarks set isshowhud = 1, maxdistance = -1 "
                            + $"where entityid ='{playerId}' and name like 'Waez\\_%' escape '\\';";
                        break;
                    default:
                        modApi.Application.SendChatMessage(new ChatMessage($"Invalid Command 'bookmarks {action}', use clear|hide|show",
                            modApi.Application.LocalPlayer));
                        return 0;
                }
                return command.ExecuteNonQuery();
            }
            catch (SqliteException ex)
            {
                modApi.Log($"SqliteException in ModifyPathMarkers: {ex.Message}");
                return 0;
            }
            finally
            {
                command?.Dispose();
                connection?.Dispose();
            }
        }

            public bool GetSolarSystemCoordinates(string starName, out SectorCoordinates coordinates)
        {
            SqliteConnection connection = null;
            SqliteCommand command = null;
            IDataReader reader = null;

            try
            {
                connection = GetConnection();
                command = connection.CreateCommand();
                command.CommandText = "select sectorx, sectory, sectorz from SolarSystems "
                        + $"where name='{starName}';";
                reader = command.ExecuteReader();
                if (reader.Read())
                {
                    coordinates = new SectorCoordinates(
                        reader.GetInt32(0),
                        reader.GetInt32(1),
                        reader.GetInt32(2)
                    );
                    return true;
                }
            }
            catch (SqliteException ex)
            {
                Log($"SqliteException in GetSolarSystemCoordinates: {ex.Message}");
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
    }
}
