using System;

namespace GalacticWaez
{
    public class NavigationHandler : ICommandHandler
    {
        private readonly INavigator Navigator;
        public NavigationHandler(INavigator nav)
            => Navigator = nav ?? throw new ArgumentNullException("NavigationHandler: nav");

        public bool HandleCommand(string commandText, IPlayerInfo player, IResponder responder)
        {
            if (commandText == null || !commandText.StartsWith("to "))
                return false;

            commandText = commandText.Substring(3);
            string bookmarkName;
            float range = 0;
            if (commandText.StartsWith("--range="))
            {
                var tokens = commandText.Split(new[] { ' ' }, 2);
                bookmarkName = (tokens.Length > 1) ? tokens[1] : string.Empty;
                if (!float.TryParse(tokens[0].Substring("--range=".Length), out range))
                {
                    responder?.Send("Invalid range argument.");
                    return true;
                }
                if (range <= 0)
                {
                    responder?.Send("Range must be a positive number.");
                    return true;
                }
                // range is expected in LY from user, but the mod uses sectors
                range *= GalacticWaez.SectorsPerLY;
            }
            else
            {
                range = player.WarpRange;
                bookmarkName = commandText.Trim();
            }
            Navigator.Navigate(player, bookmarkName, range, responder);
            return true;
        }
    }
}
