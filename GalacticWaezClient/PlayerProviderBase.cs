using static GalacticWaez.GalacticWaez;
using System.Data;
using System.Data.SQLite;

namespace GalacticWaez
{
    public abstract class PlayerProviderBase : SaveGameDB, IPlayerProvider
    {
        protected readonly LoggingDelegate Log;
        public PlayerProviderBase(string saveGameDir, LoggingDelegate log)
            : base(saveGameDir) { Log = log ?? delegate { }; }

        public abstract IPlayerInfo GetPlayerInfo(int playerId);

        protected float GetWarpRange(int playerId)
        {
            float warpRange = BaseWarpRangeLY;

            SQLiteConnection connection = null;
            SQLiteCommand command = null;
            IDataReader reader = null;

            try
            {
                connection = GetConnection();
                command = connection.CreateCommand();
                command.CommandText = "select value from PlayerSkillValues where "
                    + $"entityid='{playerId}' and name='PilotLYRange';";
                reader = command.ExecuteReader();
                if (reader.Read())
                {
                    warpRange += reader.GetFloat(0);
                }
            }
            catch (SQLiteException ex)
            {
                Log($"SQLiteException in GetPlayerData: {ex.Message}");
                Log($"Using base warp range ({BaseWarpRangeLY}LY) for player {playerId}");
            }
            finally
            {
                reader?.Dispose();
                command?.Dispose();
                connection?.Dispose();
            }

            return warpRange * SectorsPerLY;
        }
    }
}
