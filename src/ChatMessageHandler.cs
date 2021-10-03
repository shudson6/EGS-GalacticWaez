using Eleon;
using Eleon.Modding;
using GalacticWaez.Navigation;

namespace GalacticWaez.Command
{
    public class ChatMessageHandler 
    {
        private readonly IPlayerProvider PlayerProvider;
        private readonly IResponseManager ResponseManager;
        private readonly ICommandHandler Navigation;

        public ChatMessageHandler(IPlayerProvider players, IResponseManager responseMgr,
            ICommandHandler navHandler)
        {
            PlayerProvider = players;
            ResponseManager = responseMgr;
            Navigation = navHandler;
        }
        
        public void HandleChatMessage(MessageData messageData)
        {
            if (messageData.Text.StartsWith("/waez "))
            {
                var responder = ResponseManager.CreateResponder(messageData);
                string commandText = messageData.Text.Remove(0, "/waez ".Length).Trim();
                if (commandText.StartsWith("to "))
                {
                    Navigation.HandleCommand(commandText,
                        PlayerProvider.GetPlayerInfo(messageData.SenderEntityId),
                        responder);
                }
                //string[] tokens = commandText.Split(separator: new[] { ' ' }, count: 2);
                //if (tokens.Length == 2 && tokens[0].Equals(CommandToken.Bookmarks))
                //{
                //    HandleBookmarkRequest(tokens[1]);
                //    return;
                //}
            }
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
