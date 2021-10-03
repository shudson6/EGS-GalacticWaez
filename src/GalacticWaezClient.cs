using Eleon.Modding;
using System;

namespace GalacticWaez
{
    public enum ModState
    {
        Uninitialized,
        Initializing,
        InitFailed,
        Ready
    }

    public class GalacticWaezClient : IMod, ICommandHandler
    {
        private IModApi modApi;

        private ICommandHandler commandHandler = null;
        public ModState Status { get; private set; }

        public void Init(IModApi modApi)
        {
            this.modApi = modApi;
            modApi.GameEvent += OnGameEvent;
            modApi.Log("GalacticWaezClient attached.");
            Status = ModState.Uninitialized;
        }

        public void Shutdown()
        {
            modApi.GameEvent -= OnGameEvent;
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
                    }
                    break;

                case GameEventType.GameEnded:
                    if (commandHandler != null)
                    {
                    }
                    modApi.Log("Stopped listening for commands.");
                    break;
            }
        }

        public bool HandleCommand(string commandText, IPlayerInfo player, IResponder responder)
        {
            if (commandText.Trim() != "status")
                return false;

            responder.Send(Status.ToString());
            return true;
        }
    }
}
