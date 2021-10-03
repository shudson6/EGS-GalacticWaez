using System;
using System.Collections.Generic;
using Eleon;
using Eleon.Modding;

namespace GalacticWaez
{
    public class ResponseManager : IResponseManager
    {
        private class Responder : IResponder
        {
            private readonly MessageData req;
            private readonly Action<MessageData> SendMessage;
            private readonly Func<ulong> GameTicks;

            public Responder(MessageData request, Action<MessageData> send, Func<ulong> ticks)
            {
                req = request;
                SendMessage = send;
                GameTicks = ticks;
            }

            public void Send(string text)
            {
                SendMessage(new MessageData
                {
                    SenderType = SenderType.System,
                    SenderNameOverride = "Waez",
                    Text = text,
                    GameTime = GameTicks(),
                    Channel = req.Channel,
                    RecipientEntityId = req.SenderEntityId,
                    RecipientFaction = req.SenderFaction,
                    IsTextLocaKey = false
                });
            }
        }


        private readonly Queue<MessageData> messageQueue;
        private readonly IApplication App;

        public ResponseManager(IApplication app)
        {
            App = app;
            messageQueue = new Queue<MessageData>();
        }

        public IResponder CreateResponder(MessageData msg)
            => new Responder(msg, EnqueueMessage, () => App.GameTicks);

        private void EnqueueMessage(MessageData msg)
        {
            lock (messageQueue)
            {
                if (messageQueue.Count == 0)
                    App.Update += SendMessagesOnUpdate;

                messageQueue.Enqueue(msg);
            }
        }

        private void SendMessagesOnUpdate()
        {
            lock (messageQueue)
            {
                if (messageQueue.Count > 0)
                    App.SendChatMessage(messageQueue.Dequeue());

                if (messageQueue.Count == 0)
                    App.Update -= SendMessagesOnUpdate;
            }
        }
    }
}
