using Eleon.Modding;
using System.Threading.Tasks;

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
        private ChatMessageHandler chatHandler = null;
        private Task<ChatMessageHandler> init = null;

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

        public bool HandleCommand(string cmdToken, string args, IPlayerInfo player, IResponder responder)
        {
            if (cmdToken != "status" || !(args == null || args == ""))
                return false;

            responder.Send(Status.ToString());
            return true;
        }

        private void OnGameEvent(GameEventType type,
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
                        modApi.Application.Update += OnUpdateTilWorldVisible;
                        modApi.Log("Listening for commands.");
                    }
                    break;

                case GameEventType.GameEnded:
                    modApi.Application.Update -= OnUpdateTilWorldVisible;
                    if (chatHandler != null)
                    {
                        modApi.Application.ChatMessageSent -= chatHandler.HandleChatMessage;
                    }
                    modApi.Log("Stopped listening for commands.");
                    break;
            }
        }

        private void OnUpdateTilWorldVisible()
        {
            if (!modApi.GUI.IsWorldVisible)
                return;

            modApi.Application.Update -= OnUpdateTilWorldVisible;
            Setup();
        }

        private void Setup()
        {
            init = Task<ChatMessageHandler>.Factory.StartNew(SetupTask, DataSourceType.Normal);
            modApi.Application.Update += OnUpdateDuringInit;
        }

        private ChatMessageHandler SetupTask(object obj)
        {
            Status = ModState.Initializing;
            string saveGameDir = modApi.Application.GetPathFor(AppFolder.SaveGame);
            // start with the GalaxyMap: the longest and most likely to fail
            var ksp = new KnownStarProvider(saveGameDir, modApi.Log);
            var source = new NormalDataSource(
                new FileDataSource(saveGameDir, modApi.Log),
                new StarFinderDataSource(ksp, modApi.Log));
            var galaxyMap = new GalaxyMapBuilder(modApi.Log)
                .BuildGalaxyMap(source, 110 * GalacticWaez.SectorsPerLY);

            // assemble the navigator
            var bm = new BookmarkManager(saveGameDir, modApi.Log);
            var nav = new Navigator(galaxyMap, new AstarPathfinder(), bm, ksp, modApi.Log,
                () => modApi.Application.GameTicks);
            var nh = new NavigationHandler(nav);

            // finally, the chat message handler
            var pp = new LocalPlayerInfo(modApi.Application.LocalPlayer, saveGameDir,
                () => modApi.ClientPlayfield, modApi.Log);
            return new ChatMessageHandler(pp, new ResponseManager(modApi.Application), 
                nh, this, new BookmarkHandler(bm, modApi.Log), new HelpHandler());
        }

        private void OnUpdateDuringInit()
        {
            if (init == null || init.Status < TaskStatus.RanToCompletion)
                return;

            modApi.Application.Update -= OnUpdateDuringInit;
            if (init.Status == TaskStatus.RanToCompletion)
            {
                SetChatHandler(init.Result);
                Status = ModState.Ready;
                modApi.GUI.ShowGameMessage("Waez is Ready.", prio: 0, duration: 5);
            }
            else
            {
                Status = ModState.InitFailed;
                modApi.GUI.ShowGameMessage("Waez failed to start.", prio: 0, duration: 5);
            }
        }

        private void SetChatHandler(ChatMessageHandler handler)
        {
            if (chatHandler != null)
                modApi.Application.ChatMessageSent -= chatHandler.HandleChatMessage;

            chatHandler = handler;
            modApi.Application.ChatMessageSent += handler.HandleChatMessage;
        }
    }
}
