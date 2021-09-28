using Mono.Data.Sqlite;
using System.Data;
using static GalacticWaez.Const;
using Eleon.Modding;
using SectorCoordinates = Eleon.Modding.VectorInt3;

namespace GalacticWaez.Navigation
{
    public sealed class LocalPlayerTracker : SaveGameDBBase, IPlayerTracker
    {
        private readonly IModApi modApi;
        private readonly float warpRangeOverride;

        public int GetPlayerId() => modApi.Application.LocalPlayer.Id;

        public LocalPlayerTracker(IModApi modApi) : this(modApi, 0) { }
        public LocalPlayerTracker(IModApi modApi, float warpRangeOverride)
            : base(modApi.Application.GetPathFor(AppFolder.SaveGame) + "\\global.db") 
        {
            this.modApi = modApi;
            this.warpRangeOverride = warpRangeOverride;
        }

        public SectorCoordinates GetCurrentStarCoordinates()
            => modApi.ClientPlayfield.SolarSystemCoordinates;

        public float GetWarpRange()
        {
            if (warpRangeOverride > 0)
                return warpRangeOverride;

            float warpRange = BaseWarpRangeLY;
            int playerId = modApi.Application.LocalPlayer.Id;

            SqliteConnection connection = null;
            SqliteCommand command = null;
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
            catch (SqliteException ex)
            {
                modApi.Log($"SqliteException in GetPlayerData: {ex.Message}");
                modApi.Log($"Using base warp range ({BaseWarpRangeLY}LY) for player {playerId}");
            }
            finally
            {
                reader?.Dispose();
                command?.Dispose();
                connection?.Dispose();
            }

            return warpRange;
        }
    }
}