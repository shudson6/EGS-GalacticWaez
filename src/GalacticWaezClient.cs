using Eleon.Modding;
using GalacticWaez.Command;
using System;

namespace GalacticWaez
{
    public class GalacticWaezClient : IMod
    {
        public IModApi ModApi { get; private set; }
        public IMessenger Messenger { get; private set; }

        private ICommandHandler commandHandler = null;
        private IInitializer initializer = null;

        public void Init(IModApi modApi)
        {
            ModApi = modApi;
            ModApi.GameEvent += OnGameEvent;
            ModApi.Log("GalacticWaezClient attached.");
        }

        public void Shutdown()
        {
            ModApi.GameEvent -= OnGameEvent;
            ModApi.Log("GalacticWaezClient detached.");
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
                    if (ModApi.Application.Mode == ApplicationMode.SinglePlayer)
                    {
                        //initializer = new ClientInitializer(ModApi);
                        //initializer.Initialize(InitializerType.Normal, InitializerCallback);
                    }
                    break;

                case GameEventType.GameEnded:
                    if (commandHandler != null)
                    {
                        ModApi.Application.ChatMessageSent -= commandHandler.HandleChatCommand;
                        commandHandler = null;
                    }
                    ModApi.Log("Stopped listening for commands.");
                    break;
            }
        }

        private void InitializerCallback(ICommandHandler icmd, AggregateException ex)
        {
            if (icmd != null)
            {
                commandHandler = icmd;
                ModApi.Application.ChatMessageSent += commandHandler.HandleChatCommand;
            }
            else
            {
                foreach (var e in ex.InnerExceptions)
                {
                    ModApi.LogError(e.Message);
                }
            }
        }
    }
}
