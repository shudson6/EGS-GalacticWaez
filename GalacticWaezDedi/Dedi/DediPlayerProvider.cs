using Eleon.Modding;
using Mono.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Data;
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

        public DediPlayerProvider(string saveGameDir, LoggingDelegate log)
            : base(saveGameDir, log) { }

        public override IPlayerInfo GetPlayerInfo(int playerId)
        {
            // using DB access to get info b/c API2 doesn't expose it
            // and API1 does it in a cumbersome way that I just don't feel like doing
            float warpRange = GetWarpRange(playerId);
            string playerName;
            int factionId;
            VectorInt3 starPos;
            string pfName;
            int pfid;
            string ssName;

            SqliteConnection connection = null;
            SqliteCommand command = null;
            try
            {
                connection = GetConnection();
                command = connection.CreateCommand();
                command.CommandText = "select name, facid, pfid"
                    + " from Entities"
                    + $" where entityid={playerId}";
                using (var reader = command.ExecuteReader())
                {
                    reader.Read();
                    playerName = reader.GetString(0);
                    factionId = reader.GetInt32(1);
                    pfid = reader.GetInt32(2);
                }
                command.CommandText = "select s.name, s.sectorx, s.sectory, s.sectorz, p.name"
                    + " from SolarSystems s inner join playfields p using(ssid)"
                    + $" where pfid={pfid};";
                using (var reader = command.ExecuteReader())
                {
                    reader.Read();
                    ssName = reader.GetString(0);
                    starPos = new VectorInt3(
                        reader.GetInt32(1),
                        reader.GetInt32(2),
                        reader.GetInt32(3));
                    pfName = reader.GetString(4);
                }
                return new PlayerInfo
                {
                    name = playerName,
                    id = playerId,
                    facId = factionId,
                    range = warpRange,
                    starCoords = starPos,
                    pfName = pfName,
                    ssName = ssName
                };
            }
            catch (SqliteException ex)
            {
                Log($"SqliteException in GetPlayerData: {ex.Message}");
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

