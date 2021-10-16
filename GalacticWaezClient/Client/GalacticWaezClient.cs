using Eleon.Modding;

namespace GalacticWaez.Client
{
    public class GalacticWaezClient : GalacticWaez
    {
        public override void Init(IModApi modApi)
        {
            base.Init(modApi);
            ModApi.GameEvent += OnGameEvent;
            ModApi.Log("GalacticWaezClient attached.");
        }

        public override void Shutdown()
        {
            base.Shutdown();
            ModApi.GameEvent -= OnGameEvent;
            ModApi.Log("GalacticWaezClient detached.");
        }

        protected override IPlayerProvider CreatePlayerProvider()
            => new LocalPlayerInfo(ModApi, Config.BaseWarpRange);

        private void OnGameEvent(GameEventType type,
                        object arg1 = null,
                        object arg2 = null,
                        object arg3 = null,
                        object arg4 = null,
                        object arg5 = null) 
        {
            switch (type)
            {
                case GameEventType.GameStarted:
                    if (ModApi.Application.Mode == ApplicationMode.SinglePlayer)
                    {
                        ModApi.Application.Update += OnUpdateTilWorldVisible;
                        PlayerProvider = CreatePlayerProvider();
                        ChatHandler = CreateChatHandler(PlayerProvider);
                        ModApi.Log("Listening for commands.");
                    }
                    break;

                case GameEventType.GameEnded:
                    ModApi.Application.Update -= OnUpdateTilWorldVisible;
                    ChatHandler = null;
                    ModApi.Log("Stopped listening for commands.");
                    break;
            }
        }

        private void OnUpdateTilWorldVisible()
        {
            if (!ModApi.GUI.IsWorldVisible)
                return;

            ModApi.Application.Update -= OnUpdateTilWorldVisible;
            Setup(DataSourceType.Normal);
        }

        protected override void OnInitDone()
        {
            ModApi.GUI.ShowGameMessage("Waez is ready.", prio: 0, duration: 5);
        }

        protected override void OnInitFail()
        {
            ModApi.GUI.ShowGameMessage("Waez map construction failed.", prio: 0, duration: 5);
        }
    }
}
