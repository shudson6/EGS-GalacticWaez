using Eleon.Modding;
using Mono.Data.Sqlite;
using System.Data;
using System;

namespace GalacticWaez
{
    public class BookmarkManager :  SaveGameDBBase, IBookmarkManager
    {
        private readonly LoggingDelegate Log;

        public BookmarkManager(string saveGameDir, LoggingDelegate log) 
            : base(saveGameDir) { Log = log ?? delegate { }; }

        public bool TryGetVector(int playerId, int playerFacId, string bookmarkName, out VectorInt3 coordinates)
        {
            if (bookmarkName == null)
                throw new ArgumentNullException("bookmarkName must not be null");

            SqliteConnection connection = null;
            SqliteCommand command = null;
            IDataReader reader = null;

            try
            {
                connection = GetConnection();
                command = connection.CreateCommand();
                command.CommandText = "select sectorx, sectory, sectorz from Bookmarks "
                        + $"where type='0' and name='{bookmarkName}'"
                        + $" and (entityid='{playerId}'"
                        + $" or (facid='{playerFacId}' and isshared='1'));";
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
    }
}
