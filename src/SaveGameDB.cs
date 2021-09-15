using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Eleon.Modding;
using Mono.Data.Sqlite;
using System.Data;
using static GalacticWaez.Const;
using SectorCoordinates = Eleon.Modding.VectorInt3;

namespace GalacticWaez
{
    class SaveGameDB
    {
        IModApi modApi;
        
        public SaveGameDB(IModApi modApi)
        {
            this.modApi = modApi;
        }

        public PlayerData GetPlayerData()
        {
            float warpRange = BaseWarpRange;
            IPlayer player = modApi.Application.LocalPlayer;

            SqliteConnection connection = null;
            SqliteCommand command = null;
            IDataReader reader = null;

            try
            {
                connection = GetConnection();
                command = connection.CreateCommand();
                command.CommandText = "select value from PlayerSkillValues where "
                    + $"entityid='{player.Id}' and name='PilotLYRange';";
                reader = command.ExecuteReader();
                if (reader.Read())
                {
                    warpRange += reader.GetFloat(0);
                }
            }
            catch (SqliteException ex)
            {
                modApi.Log($"SqliteException in GetPlayerData: {ex.Message}");
                modApi.Log($"Using base warp range ({BaseWarpRange}LY) for {player.Name}");
            }
            finally
            {
                reader?.Dispose();
                command?.Dispose();
                connection?.Dispose();
            }

            return new PlayerData(player, warpRange);
        }

        public SectorCoordinates GetFirstKnownStarPosition()
        {
            SqliteConnection connection = null;
            SqliteCommand command = null;
            IDataReader reader = null;

            try
            {
                connection = GetConnection();
                command = connection.CreateCommand();
                // players have to start somewhere; there will always be at least one entry
                command.CommandText = "select sectorx, sectory, sectorz from SolarSystems limit 1;";
                reader = command.ExecuteReader();
                reader.Read();
                return new SectorCoordinates(
                    reader.GetInt32(0),
                    reader.GetInt32(1),
                    reader.GetInt32(2)
                );
            }
            finally
            {
                reader?.Dispose();
                command?.Dispose();
                connection?.Dispose();
            }
        }

        public bool GetBookmarkVector(string bookmarkName, out SectorCoordinates coordinates)
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
                        + $"'{modApi.Application.LocalPlayer.Id}';";
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
                modApi.Log($"SqliteException in GetBookmarkVector: {ex.Message}");
            }
            finally
            {
                reader?.Dispose();
                command?.Dispose();
                connection?.Dispose();
            }
            coordinates = ErrorVector;
            return false;
        }

        // returns the number of bookmarks added
        public int InsertBookmarks(IEnumerable<SectorCoordinates> positions, PlayerData playerData)
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
                ulong ticks = modApi.Application.GameTicks;
                foreach (var p in positions)
                {
                    sql.Append($"({bid},0,0,1,");
                    sql.Append($"{playerData.Entity.Faction.Id},{playerData.Entity.Id},");
                    sql.Append($"'Waez_{stepNo}',{p.x},{p.y},{p.z},0,0,0,2,0,1,1,1,0,");
                    sql.Append($"{ticks},0,0,{playerData.WarpRange + 1}),");
                    stepNo++;
                    bid++;
                }
                sql.Replace(',', ';', sql.Length - 1, 1);
                command.CommandText = sql.ToString();
                return command.ExecuteNonQuery();
            }
            catch (SqliteException ex)
            {
                modApi.Log($"SqliteException in InsertBookmarks: {ex.Message}");
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
                command.CommandText = "select max(bid) from Bookmarks;";
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
                modApi.Log($"SqliteException in ClearPathMarkers: {ex.Message}");
                return 0;
            }
            finally
            {
                command?.Dispose();
                connection?.Dispose();
            }
        }

        SqliteConnection GetConnection(bool writeable = false)
        {
            string openMode = writeable ? "ReadWrite" : "ReadOnly";
            var details = new SqliteConnectionStringBuilder();
            details.DataSource = modApi.Application.GetPathFor(AppFolder.SaveGame) + "\\global.db";
            details.Add("Mode", openMode);
            var connection = new SqliteConnection(details.ToString());
            connection.Open();
            return connection;
        }
    }
}
