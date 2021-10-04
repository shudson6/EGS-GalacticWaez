using Eleon;

namespace GalacticWaez
{
    public class ChatMessageHandler 
    {
        private readonly IPlayerProvider PlayerProvider;
        private readonly IResponseManager ResponseManager;
        private readonly ICommandHandler[] Handlers;

        public ChatMessageHandler(IPlayerProvider players, IResponseManager responseMgr,
            params ICommandHandler[] handlers)
        {
            PlayerProvider = players;
            ResponseManager = responseMgr;
            Handlers = handlers;
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
            var command = line[1].TrimStart().Split(new[] { ' ' }, 2);
            var token = command[0];
            var args = (command.Length == 2) ? command[1] : null;

            foreach (var h in Handlers)
            {
                if (h.HandleCommand(token, args, player, responder))
                    return;
            }

            responder.Send("Unrecognized Command: " + line[1]);
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
