using Eleon.Modding;
using System;
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
                case "ship":
                    HandleShip(responder);
                    break;
                case "refresh":
                    HandleRefresh(responder);
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

        private void HandleShip(IResponder responder)
        {
            if (Waez.ModApi.Application.Mode != ApplicationMode.SinglePlayer)
            {
                responder.Send("Only available in single player.");
                return;
            }
            var ship = Waez.ModApi.Application.LocalPlayer.CurrentStructure;
            if (ship == null)
            {
                responder.Send("No current structure.");
                return;
            }
            responder.Send($"Ship: {ship.Entity.Id} {ship.Entity.Name}");
            foreach (var s in ship.GetAllCustomDeviceNames())
                responder.Send(s);
        }

        private void HandleRefresh(IResponder responder)
        {
            try
            {
                new Eleon.PdaScript.Task("hello").RefreshHUD();
                responder.Send("Tried it");
            }
            catch (Exception ex)
            {
                responder.Send(ex.ToString());
                Waez.ModApi.Log("Debug.HandleRefresh: " + ex.Message);
            }
        }
    }
}
