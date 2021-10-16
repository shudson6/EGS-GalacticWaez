using System.Text;
using static GalacticWaez.GalacticWaez;

namespace GalacticWaez
{
    public class DebugCommandHandler : ICommandHandler
    {
        private readonly GalacticWaez Waez;

        public DebugCommandHandler(GalacticWaez waez) => Waez = waez;

        public bool HandleCommand(string cmdToken, string args, IPlayerInfo player, IResponder responder)
        {
            switch (cmdToken)
            {
                case "pinfo":
                    HandlePinfo(args, player, responder);
                    break;
                case "ginfo":
                    HandleGinfo(responder);
                    break;
                default:
                    return false;
            }
            return true;
        }

        private void HandlePinfo(string args, IPlayerInfo player, IResponder responder)
        {
            if (args == null || args == "")
            {
                responder.Send(InfoMessage(player));
            }
            else
            {
                if (!int.TryParse(args, out int id))
                {
                    responder.Send("Invalid playerId");
                }
                responder.Send(InfoMessage(Waez.PlayerProvider.GetPlayerInfo(id)));
            }
        }

        private string InfoMessage(IPlayerInfo player)
        {
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

        private void HandleGinfo(IResponder responder)
        {
            if (Waez.Galaxy == null)
            {
                responder.Send("No galaxy map.");
                return;
            }
            responder.Send($"Stars: {Waez.Galaxy.Stars}\nWarp Lines: {Waez.Galaxy.WarpLines}\n"
                + $"Max Range: {Waez.Galaxy.Range / SectorsPerLY}");
            return;
        }
    }
}
