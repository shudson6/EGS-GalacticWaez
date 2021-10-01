using Eleon.Modding;
using Mono.Data.Sqlite;
using System;
using System.Data;

namespace GalacticWaez
{
    public class KnownStarProvider : SaveGameDBBase, IKnownStarProvider
    {
        private readonly LoggingDelegate Log;

        public KnownStarProvider(string saveGameDir, LoggingDelegate log)
            : base($"{saveGameDir}\\{DbFileName}")
        {
            Log = log;
        }

        public bool GetFirstKnownStarPosition(out VectorInt3 pos)
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
                pos = new VectorInt3(
                    reader.GetInt32(0),
                    reader.GetInt32(1),
                    reader.GetInt32(2)
                );
                return true;
            }
            catch (Exception ex)
            {
                Log("Exception in GetFirstKnownStarPosition:");
                Log(ex.Message);
                pos = default;
                return false;
            }
            finally
            {
                reader?.Dispose();
                command?.Dispose();
                connection?.Dispose();
            }
        }

        public bool GetPosition(string name, out VectorInt3 pos)
        {
            SqliteConnection connection = null;
            SqliteCommand command = null;
            IDataReader reader = null;

            try
            {
                connection = GetConnection();
                command = connection.CreateCommand();
                command.CommandText = "select sectorx, sectory, sectorz from SolarSystems "
                        + $"where name='{name}';";
                reader = command.ExecuteReader();
                if (reader.Read())
                {
                    pos = new VectorInt3(
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
            pos = default;
            return false;
        }
    }
}
