using Eleon;

namespace GalacticWaez
{
    public class ChatMessageHandler 
    {
        private readonly IPlayerProvider PlayerProvider;
        private readonly IResponseManager ResponseManager;
        private readonly ICommandHandler Navigation;
        private readonly ICommandHandler StatusHandler;

        public ChatMessageHandler(IPlayerProvider players, IResponseManager responseMgr,
            ICommandHandler navHandler,
            ICommandHandler statusHandler)
        {
            PlayerProvider = players;
            ResponseManager = responseMgr;
            Navigation = navHandler;
            StatusHandler = statusHandler;
        }
        
        public void HandleChatMessage(MessageData messageData)
        {
            var line = messageData.Text.TrimStart().Split(new[] { ' ' }, 2);
            if (line[0] != "/waez")
                return;

            var responder = ResponseManager.CreateResponder(messageData);
            var player = PlayerProvider.GetPlayerInfo(messageData.SenderEntityId);
            if (line.Length < 2)
            {
                responder.Send("hm? (don't know what to do? \"/waez help\")");
                return;
            }
            string commandText = line[1].TrimStart();
            string commandToken = commandText.Split(new[] { ' ' }, 2)[0];

            responder.Send($"echo: {commandToken}|{commandText}");
            switch (commandToken)
            {
                case "status":
                    StatusHandler.HandleCommand(commandText, player, responder);
                    break;

                case "to":
                    Navigation.HandleCommand(commandText, player, responder);
                    break;
            }
            //string[] tokens = commandText.Split(separator: new[] { ' ' }, count: 2);
            //if (tokens.Length == 2 && tokens[0].Equals(CommandToken.Bookmarks))
            //{
            //    HandleBookmarkRequest(tokens[1]);
            //    return;
            //}
        }

        private const string HelpText = "Waez commands:\n"
            + "to [mapmarker]: plot a course to [mapmarker] and add mapmarkers for each step\n"
            + "status: find out what Waez is up to\n"
            + "init: initialize Waez. this should happen automatically\n"
            + "clear: remove all map markers that start with Waez_\n"
            + "bookmarks [clear|hide|show]: remove Waez_ bookmarks or hide/show them in HUD (requires exit/resume)\n"
            + "help: get this help message\n";

        //private void HandleBookmarkRequest(string operation)
        //{
        //    string message = $"Modified "
        //        + saveGameDB.ModifyPathMarkers(modApi.Application.LocalPlayer.Id, operation)
        //        + " map markers.";
        //    modApi.Application.SendChatMessage(new ChatMessage(message,
        //        modApi.Application.LocalPlayer));
        //    return;
        //}

    }
}
