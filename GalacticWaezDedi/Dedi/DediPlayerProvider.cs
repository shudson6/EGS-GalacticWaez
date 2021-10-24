using Eleon.Modding;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GalacticWaez.Dedi
{
    class DediPlayerProvider : PlayerProviderBase
    {
        class PlayerInfo : IPlayerInfo
        {
            public string name;
            public int id;
            public int facId;
            public float range;
            public VectorInt3 starCoords;
            public string pfName;
            public string ssName;

            public string Name => name;
            public int Id => id;
            public int FactionId => facId;
            public float WarpRange => range;
            public VectorInt3 StarCoordinates => starCoords;
            public string PlayfieldName => pfName;
            public string StarName => ssName;
        }

        private readonly Func<int, PlayerData?> GetPlayerData;

        public DediPlayerProvider(string saveGameDir, int baseWarpRange,
            Func<int, PlayerData?> getPlayer, LoggingDelegate log)
            : base(saveGameDir, baseWarpRange, log) 
        {
            GetPlayerData = getPlayer ?? throw new ArgumentNullException("DediPlayerProvider: getPlayer");
        }

        public override IPlayerInfo GetPlayerInfo(int playerId)
        {
            var playerData = GetPlayerData(playerId);
            if (playerData == null)
                return null;

            var info = new PlayerInfo
            {
                id = playerId,
                name = playerData?.PlayerName,
                pfName = playerData?.PlayfieldName
            };

            info.range = GetWarpRange(playerId);

            // decided to get player info this way because:
            // don't need all (or even much) of the info available from IPlayer
            // API2 doesn't have a method to get an IPlayer (except LocalPlayer) and
            // I don't want to make all the changes to get API1 involved just for like 2 things
            SQLiteConnection connection = null;
            SQLiteCommand command = null;
            try
            {
                connection = GetConnection();
                command = connection.CreateCommand();
                command.CommandText = "select facid"
                    + " from Entities"
                    + $" where entityid={playerId}";
                using (var reader = command.ExecuteReader())
                {
                    reader.Read();
                    info.facId = reader.GetInt32(0);
                }
                command.CommandText = "select s.name, s.sectorx, s.sectory, s.sectorz"
                    + " from SolarSystems s inner join playfields p using(ssid)"
                    + " where p.name=@pfName;";
                command.Parameters.AddWithValue("@pfName", info.pfName);
                using (var reader = command.ExecuteReader())
                {
                    reader.Read();
                    info.ssName = reader.GetString(0);
                    info.starCoords = new VectorInt3(
                        reader.GetInt32(1),
                        reader.GetInt32(2),
                        reader.GetInt32(3));
                }
                return info;
            }
            catch (SQLiteException ex)
            {
                Log($"SQLiteException in GetPlayerData: {ex.Message}");
                Exception inner = ex;
                while (ex.InnerException != null)
                {
                    inner = inner.InnerException;
                    Log($"Caused by: {inner.Message}");
                }
                Log($"current query was: {command?.CommandText}");
                return null;
            }
            finally
            {
                command?.Dispose();
                connection?.Dispose();
            }
        }
    }
}

