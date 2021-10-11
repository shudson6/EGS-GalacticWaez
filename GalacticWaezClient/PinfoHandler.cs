using System.Text;
using static GalacticWaez.GalacticWaez;

namespace GalacticWaez
{
    public class PinfoHandler : ICommandHandler
    {
        private readonly IPlayerProvider playerProvider;

        public PinfoHandler(IPlayerProvider provider) => playerProvider = provider;

        public bool HandleCommand(string cmdToken, string args, IPlayerInfo player, IResponder responder)
        {
            if (cmdToken != "pinfo")
                return false;

            int id;
            if (args == null || args == "")
            {
                id = player.Id;
            }
            else
            {
                if (!int.TryParse(args, out id))
                {
                    responder.Send("Invalid playerId");
                    return true;
                }
            }

            responder.Send(InfoMessage(id));
            return true;
        }

        private string InfoMessage(int playerId)
        {
            var player = playerProvider.GetPlayerInfo(playerId);
            if (player == null)
                return "Not available.";

            var starCoords = player.StarCoordinates.Divide(SectorsPerLY);

            var sb = new StringBuilder();
            sb.AppendLine($"Name: {player.Name}");
            sb.AppendLine($"ID: {player.Id}");
            sb.AppendLine($"Playfield: {player.PlayfieldName}");
            sb.AppendLine($"Star: {player.StarName} ({starCoords.x}, {starCoords.y}, {starCoords.z})");
            sb.AppendLine($"Warp Range: {player.WarpRange / SectorsPerLY}LY");
            return sb.ToString();
        }
    }
}
