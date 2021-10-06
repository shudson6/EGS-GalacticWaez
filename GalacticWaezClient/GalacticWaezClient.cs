using Eleon.Modding;
using System.IO;
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
        private Task<bool> init = null;

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
                        SetChatHandler(CreateChatHandler());
                        modApi.Log("Listening for commands.");
                    }
                    break;

                case GameEventType.GameEnded:
                    modApi.Application.Update -= OnUpdateTilWorldVisible;
                    SetChatHandler(null);
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

        private ChatMessageHandler CreateChatHandler()
            => new ChatMessageHandler(
                new ResponseManager(modApi.Application),
                this,
                new HelpHandler());

        private void Setup()
        {
            init = Task<bool>.Factory.StartNew(SetupTask, DataSourceType.Normal);
            modApi.Application.Update += OnUpdateDuringInit;
        }

        private bool SetupTask(object obj)
        {
            Status = ModState.Initializing;
            string saveGameDir = modApi.Application.GetPathFor(AppFolder.SaveGame);
            // start with the GalaxyMap: the longest and most likely to fail
            var ksp = new KnownStarProvider(saveGameDir, modApi.Log);
            var file = new FileDataSource(saveGameDir, modApi.Log);
            // yes, FileDataSource implements both interfaces so is passed twice to NormalDataSource
            var source = new NormalDataSource(file,
                new StarFinderDataSource(ksp, modApi.Log), file);
            var galaxyMap = new GalaxyMapBuilder(modApi.Log)
                .BuildGalaxyMap(source, 110 * GalacticWaez.SectorsPerLY);
            if (galaxyMap == null)
                return false;

            // assemble the navigator
            var bm = new BookmarkManager(saveGameDir, modApi.Log);
            var nav = new Navigator(galaxyMap, new AstarPathfinder(), bm, ksp, modApi.Log,
                () => modApi.Application.GameTicks);
            var nh = new NavigationHandler(nav);

            // finally, the chat message handler
            chatHandler.PlayerProvider = new LocalPlayerInfo(modApi.Application.LocalPlayer, saveGameDir,
                () => modApi.ClientPlayfield, modApi.Log);
            chatHandler.AddHandler(nh);
            chatHandler.AddHandler(new BookmarkHandler(bm, modApi.Log));
            return true;
        }

        private void OnUpdateDuringInit()
        {
            if (init == null || init.Status < TaskStatus.RanToCompletion)
                return;

            modApi.Application.Update -= OnUpdateDuringInit;
            if (init.Status == TaskStatus.RanToCompletion && init.Result == true)
            {
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
            if (handler != null)
                modApi.Application.ChatMessageSent += handler.HandleChatMessage;
        }
    }
}
