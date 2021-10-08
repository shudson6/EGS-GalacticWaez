using System;
using System.Collections.Generic;
using Eleon;

namespace GalacticWaez
{
    public class ChatMessageHandler 
    {
        private readonly List<ICommandHandler> Handlers;

        public IReadOnlyList<ICommandHandler> CommandHandlers => Handlers;
        public IPlayerProvider PlayerProvider { get; }
        public IResponseManager ResponseManager { get; }

        public ChatMessageHandler(IPlayerProvider playerProvider, IResponseManager responseMgr,
            params ICommandHandler[] handlers)
        {
            PlayerProvider = playerProvider 
                ?? throw new ArgumentNullException("ChatMessageHandler: playerProvider");
            ResponseManager = responseMgr
                ?? throw new ArgumentNullException("ChatMessageHandler: responseMgr");
            Handlers = new List<ICommandHandler>(handlers);
        }
        
        public void HandleChatMessage(MessageData messageData)
        {
            if (messageData == null || messageData.Text == null || messageData.Text == "")
                return;

            var line = messageData.Text.TrimStart().Split(new[] { ' ' }, 2, StringSplitOptions.RemoveEmptyEntries);
            if (line[0] != "/waez")
                return;

            var responder = ResponseManager.CreateResponder(messageData);
            var player = PlayerProvider.GetPlayerInfo(messageData.SenderEntityId);
            if (player == null)
            {
                responder.Send("Can't identify requesting player");
                return;
            }
            if (line.Length < 2)
            {
                responder.Send("hm? (don't know what to do? \"/waez help\")");
                return;
            }
            var command = line[1].TrimStart().Split(new[] { ' ' }, 2);
            var token = command[0];
            var args = (command.Length == 2) ? command[1] : null;

            lock (Handlers)
            {
                foreach (var h in Handlers)
                {
                    if (h.HandleCommand(token, args, player, responder))
                        return;
                }
            }

            responder.Send("Unrecognized Command: " + line[1]);
        }

        public void AddHandler(ICommandHandler handler)
        {
            lock (Handlers)
            {
                if (!Handlers.Contains(handler))
                    Handlers.Add(handler);
            }
        }

        public void RemoveHandler(ICommandHandler handler)
        {
            lock (Handlers)
            {
                Handlers.Remove(handler);
            }
        }
    }
}
