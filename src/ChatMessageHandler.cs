using Eleon;

namespace GalacticWaez
{
    public class ChatMessageHandler 
    {
        private readonly IPlayerProvider PlayerProvider;
        private readonly IResponseManager ResponseManager;
        private readonly ICommandHandler Navigation;
        private readonly ICommandHandler StatusHandler;
        private readonly ICommandHandler BookmarkHandler;
        private readonly ICommandHandler HelpHandler;

        public ChatMessageHandler(IPlayerProvider players, IResponseManager responseMgr,
            ICommandHandler navHandler,
            ICommandHandler statusHandler,
            ICommandHandler bookmarkHandler,
            ICommandHandler helper)
        {
            PlayerProvider = players;
            ResponseManager = responseMgr;
            Navigation = navHandler;
            StatusHandler = statusHandler;
            BookmarkHandler = bookmarkHandler;
            HelpHandler = helper;
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

                case "clear": // bookmarks handler will handle the shorthand "clear"
                case "bookmarks":
                    BookmarkHandler.HandleCommand(commandText, player, responder);
                    break;

            }
            //string[] tokens = commandText.Split(separator: new[] { ' ' }, count: 2);
            //if (tokens.Length == 2 && tokens[0].Equals(CommandToken.Bookmarks))
            //{
            //    HandleBookmarkRequest(tokens[1]);
            //    return;
            //}
        }

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
