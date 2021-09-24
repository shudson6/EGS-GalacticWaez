using Mono.Data.Sqlite;
using Eleon.Modding;

namespace GalacticWaez
{
    public class SaveGameDBBase
    {
        public string PathToDB { get; protected set; }

        protected SaveGameDBBase(string pathToDbFile)
        {
            PathToDB = pathToDbFile;
        }

        protected SqliteConnection GetConnection(bool writeable = false)
        {
            string openMode = writeable ? "ReadWrite" : "ReadOnly";
            string connString = $"Data Source=\"{PathToDB}\"Mode={openMode}";
            var connection = new SqliteConnection(connString);
            connection.Open();
            return connection;
        }
    }
}