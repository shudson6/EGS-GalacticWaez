﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Eleon.Modding;
using Mono.Data.Sqlite;
using System.Data;

namespace GalacticWaez
{
    class SaveGameDB
    {
        IModApi modApi;
        string saveGameDir;
        
        public SaveGameDB(IModApi modApi)
        {
            this.modApi = modApi;
            saveGameDir = modApi.Application.GetPathFor(AppFolder.SaveGame);
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

        SqliteConnection GetConnection(bool writeable = false)
        {
            string openMode = writeable ? "ReadWrite" : "ReadOnly";
            var details = new SqliteConnectionStringBuilder();
            details.DataSource = $"{saveGameDir}\\global.db";
            details.Add("Mode", openMode);
            var connection = new SqliteConnection(details.ToString());
            connection.Open();
            return connection;
        }
    }
}
