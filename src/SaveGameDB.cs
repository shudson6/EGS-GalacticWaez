using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Eleon.Modding;
using Mono.Data.Sqlite;
using System.Data;
using static GalacticWaez.Const;

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

        public VectorInt3 GetFirstKnownStarPosition()
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
                return new VectorInt3(
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

        public bool GetBookmarkVector(string bookmarkName, out VectorInt3 coordinates)
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
                    coordinates = new VectorInt3(
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
