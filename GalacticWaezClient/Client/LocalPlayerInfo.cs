using Mono.Data.Sqlite;
using System;
using System.Data;
using static GalacticWaez.GalacticWaez;
using Eleon.Modding;

namespace GalacticWaez.Client
{
    public sealed class LocalPlayerInfo : SaveGameDB, IPlayerInfo, IPlayerProvider
    {
        private readonly IModApi modApi;

        public int Id { get => modApi.Application.LocalPlayer.Id; }
        public int FactionId { get => modApi.Application.LocalPlayer.Faction.Id; }
        public float WarpRange => GetWarpRange();

        public LocalPlayerInfo(IModApi modApi)
            : base(modApi.Application.GetPathFor(AppFolder.SaveGame))
            => this.modApi = modApi;

        public IPlayerInfo GetPlayerInfo(int _) => this;

        public VectorInt3 StarCoordinates => modApi.ClientPlayfield.SolarSystemCoordinates;

        public string PlayfieldName => modApi.ClientPlayfield.Name;

        public string Name => modApi.Application.LocalPlayer.Name;

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
                modApi.Log($"SqliteException in GetPlayerData: {ex.Message}");
                modApi.Log($"Using base warp range ({BaseWarpRangeLY}LY) for player {Id}");
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