using Mono.Data.Sqlite;
using System;
using System.Data;
using static GalacticWaez.GalacticWaez;
using Eleon.Modding;

namespace GalacticWaez
{
    public sealed class LocalPlayerInfo : SaveGameDB, IPlayerInfo, IPlayerProvider
    {
        private readonly IPlayer player;
        private readonly LoggingDelegate Log;
        private readonly Func<IPlayfield> CurrentPlayfield;

        public int Id { get => player.Id; }
        public int FactionId { get => player.Faction.Id; }
        public float WarpRange => GetWarpRange();

        public LocalPlayerInfo(IPlayer localPlayer, string saveGameDir, 
            Func<IPlayfield> getPlayfield, LoggingDelegate log) 
            : base(saveGameDir)
        {
            player = localPlayer;
            Log = log ?? delegate { };
            CurrentPlayfield = getPlayfield 
                ?? throw new ArgumentNullException("LocalPlayerInfo: getPlayfield");
        }

        public IPlayerInfo GetPlayerInfo(int _) => this;

        public VectorInt3 GetCurrentStarCoordinates() => CurrentPlayfield().SolarSystemCoordinates;

        private float GetWarpRange()
        {
            float warpRange = BaseWarpRangeLY;

            SqliteConnection connection = null;
            SqliteCommand command = null;
            IDataReader reader = null;

            try
            {
                connection = GetConnection();
                command = connection.CreateCommand();
                command.CommandText = "select value from PlayerSkillValues where "
                    + $"entityid='{Id}' and name='PilotLYRange';";
                reader = command.ExecuteReader();
                if (reader.Read())
                {
                    warpRange += reader.GetFloat(0);
                }
            }
            catch (SqliteException ex)
            {
                Log($"SqliteException in GetPlayerData: {ex.Message}");
                Log($"Using base warp range ({BaseWarpRangeLY}LY) for player {Id}");
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