using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Eleon;
using Eleon.Modding;

namespace GalacticWaez
{
    public class GalacticWaezClient : IMod
    {
        const string CommandTrigger = "/waez";
        const string InitCommand = "init";

        IModApi modApi;

        public void Init(IModApi modApi)
        {
            this.modApi = modApi;
            modApi.GameEvent += onGameEvent;
            modApi.Log("GalacticWaezClient attached.");
        }

        public void Shutdown()
        {
            modApi.GameEvent -= onGameEvent;
            modApi.Application.ChatMessageSent -= onChatMessageSent;
            modApi.Log("GalacticWaezClient detached.");
        }

        void onGameEvent(GameEventType type,
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
                        modApi.Application.ChatMessageSent += onChatMessageSent;
                        modApi.Log("Listening for commands.");
                    }
                    break;

                case GameEventType.GameEnded:
                    modApi.Application.ChatMessageSent -= onChatMessageSent;
                    modApi.Log("Stopped listening for commands.");
                    break;
            }
        }

        void onChatMessageSent(MessageData messageData)
        {
            if (messageData.Text.StartsWith(CommandTrigger))
            {
                string command = messageData.Text.Substring(CommandTrigger.Length).Trim();
                if (command.Equals(InitCommand))
                {
                    modApi.Log("Initializing galactic highway map...");
                    // TODO: begin memory scan
                }
            }
        }
    }
}
