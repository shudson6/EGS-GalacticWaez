using Eleon.Modding;
using System;
using System.Data.SQLite;

namespace GalacticWaez
{
    public class SaveGameDB
    {
        public const string DbFileName = "global.db";

        public string PathToDB { get; protected set; }

        protected SaveGameDB(string saveGameDir, string file = DbFileName)
        {
            if (saveGameDir == null) 
            { 
                throw new ArgumentNullException("Path to DB must be provided.");
            }
            PathToDB = $"{saveGameDir}\\{file}";
        }

        protected SQLiteConnection GetConnection(bool writeable = false)
        {
            var connection = new SQLiteConnection(new SQLiteConnectionStringBuilder()
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