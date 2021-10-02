using Mono.Data.Sqlite;
using System;
using Eleon.Modding;

namespace GalacticWaez
{
    public class SaveGameDBBase
    {
        public const string DbFileName = "global.db";

        public string PathToDB { get; protected set; }

        protected SaveGameDBBase(string saveGameDir, string file = DbFileName)
        {
            if (saveGameDir == null) 
            { 
                throw new ArgumentNullException("Path to DB must be provided.");
            }
            PathToDB = $"{saveGameDir}\\{file}";
        }

        protected SqliteConnection GetConnection(bool writeable = false)
        {
            var connection = new SqliteConnection(new SqliteConnectionStringBuilder()
                {
                    DataSource = PathToDB,
                    Version = 3,
                    ReadOnly = !writeable
                }.ToString());
            connection.Open();
            return connection;
        }
    }
}