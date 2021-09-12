using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Eleon;
using Eleon.Modding;

namespace GalacticWaez
{
    class CommandToken
    {
        public const string Introducer = "/waez";
        public const string Init = "init";
    }

    public class GalacticWaezClient : IMod
    {
        IModApi modApi;

        public void Init(IModApi modApi)
        {
            this.modApi = modApi;
            modApi.GameEvent += OnGameEvent;
            modApi.Log("GalacticWaezClient attached.");
        }

        public void Shutdown()
        {
            modApi.GameEvent -= OnGameEvent;
            modApi.Application.ChatMessageSent -= OnChatMessageSent;
            modApi.Log("GalacticWaezClient detached.");
        }

        void OnGameEvent(GameEventType type,
                        object arg1 = null,
                        object arg2 = null,
                        object arg3 = null,
                        object arg4 = null,
                        object arg5 = null
        ) {
            switch (type)
            {
                case GameEventType.GameStarted:
                    if (modApi.Application.Mode == ApplicationMode.SinglePlayer)
                    {
                        modApi.Application.ChatMessageSent += OnChatMessageSent;
                        modApi.Log("Listening for commands.");
                    }
                    break;

                case GameEventType.GameEnded:
                    modApi.Application.ChatMessageSent -= OnChatMessageSent;
                    modApi.Log("Stopped listening for commands.");
                    break;
            }
        }

        void OnChatMessageSent(MessageData messageData)
        {
            if (messageData.Text.StartsWith(CommandToken.Introducer))
            {
                string command = messageData.Text.Substring(CommandToken.Introducer.Length).Trim();
                if (command.Equals(CommandToken.Init))
                {
                    modApi.Log("Initializing galactic highway map...");
                    // TODO: begin memory scan
                }
            }
        }
    }
}
