﻿using System.Collections.Generic;
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
                        Log($"Invalid Command 'bookmarks {action}', use clear|hide|show");
                        return 0;
                }
                return command.ExecuteNonQuery();
            }
            catch (SqliteException ex)
            {
                Log($"SqliteException in ModifyPathMarkers: {ex.Message}");
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
