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