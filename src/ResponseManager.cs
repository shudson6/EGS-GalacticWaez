using System;
using System.Collections.Generic;
using Eleon;
using Eleon.Modding;

namespace GalacticWaez
{
    /// <summary>
    /// This class provides a thread-safe means of sending chat responses.
    /// <br/>
    /// To respond to a message, use <c>CreateResponder(MessageData)</c> to get an IResponder
    /// instance to send a response to the correct recipient and channel. Said instance can be
    /// used more than once. Messages sent via the IResponder are queued to be sent from the
    /// <c>Application.Update</c> delegate.
    /// <br/>
    /// Reasoning: calling Application.SendChatMessage on a thread other than the one on which
    /// Application.Update executes causes crashes. Use this class to ensure messages are sent
    /// on the correct thread.
    /// </summary>
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
