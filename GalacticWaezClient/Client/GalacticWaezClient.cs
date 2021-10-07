using Eleon.Modding;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GalacticWaez.Client
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
        private readonly PreInitCommandHandler preCmd = new PreInitCommandHandler();

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
            try
            {
                if (cmdToken == "restart")
                {
                    HandleRestart(args, player, responder);
                    return true;
                }

                if (cmdToken != "status" || !(args == null || args == ""))
                    return false;

                responder.Send(Status.ToString());
                return true;
            }
            catch (Exception ex)
            {
                modApi.Log(ex.Message);
                modApi.Log(ex.StackTrace);
                responder.Send(ex.StackTrace);
                return true;
            }
        }

        private void HandleRestart(string arg, IPlayerInfo player, IResponder responder)
        {
            DataSourceType type;
            switch (arg)
            {
                case "normal":
                    type = DataSourceType.Normal;
                    break;

                case "file-only":
                    type = DataSourceType.FileOnly;
                    break;

                case "scan-only":
                    type = DataSourceType.ScanOnly;
                    break;

                case null:
                case "":
                    responder?.Send("Required: [normal|file-only|scan-only]");
                    return;

                default:
                    responder?.Send("Unrecognized option; use [normal|file-only|scan-only]");
                    return;
            }

            // we have a valid restart command; if the chat handler has a navigator setup, get rid of it
            // cloning the list to avoid invalidating the enumerator
            var handlers = new List<ICommandHandler>(chatHandler.CommandHandlers);
            foreach (var handler in handlers)
            {
                if (handler is NavigationHandler)
                    chatHandler.RemoveHandler(handler);
            }

            modApi.Log($"Restart requested by player {player.Id} ({player.Name}) with data source {arg}");
            responder?.Send("Restarting with data source " + arg);
            Setup(type);
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
            Setup(DataSourceType.Normal);
        }

        private ChatMessageHandler CreateChatHandler()
        {
            var pp = new LocalPlayerInfo(modApi);

            return new ChatMessageHandler(pp,
                new ResponseManager(modApi.Application),
                this, preCmd,
                new HelpHandler(),
                new PinfoHandler(pp));
        }

        private void Setup(DataSourceType type)
        {
            init = Task<bool>.Factory.StartNew(SetupTask, type);
            modApi.Application.Update += OnUpdateDuringInit;
        }

        private bool SetupTask(object obj)
        {
            Status = ModState.Initializing;
            string saveGameDir = modApi.Application.GetPathFor(AppFolder.SaveGame);

            // start with the GalaxyMap: the longest and most likely to fail
            var ksp = new KnownStarProvider(saveGameDir, modApi.Log);
            var source = CreateDataSource((DataSourceType)obj, ksp, saveGameDir);
            var galaxyMap = new GalaxyMapBuilder(modApi.Log)
                .BuildGalaxyMap(source, 110 * GalacticWaez.SectorsPerLY);
            if (galaxyMap == null)
                return false;

            // assemble the navigator
            var bm = new BookmarkManager(saveGameDir, modApi.Log);
            var nav = new Navigator(galaxyMap, new AstarPathfinder(), bm, ksp, modApi.Log,
                () => modApi.Application.GameTicks);
            var nh = new NavigationHandler(nav);

            // finish assembling the chat message handler
            chatHandler.AddHandler(nh);
            chatHandler.AddHandler(new BookmarkHandler(bm, modApi.Log));
            chatHandler.RemoveHandler(preCmd);
            return true;
        }

        private IGalaxyDataSource CreateDataSource(
            DataSourceType type, IKnownStarProvider ksp, string saveGameDir)
        {
            switch (type)
            {
                case DataSourceType.FileOnly:
                    return new FileDataSource(saveGameDir, modApi.Log);
                case DataSourceType.ScanOnly:
                    return new StarFinderDataSource(ksp, modApi.Log);
                case DataSourceType.Normal:
                    // yes, FileDataSource implements both interfaces so is passed twice to NormalDataSource
                    var file = new FileDataSource(saveGameDir, modApi.Log);
                    return new NormalDataSource(
                        file,
                        new StarFinderDataSource(ksp, modApi.Log),
                        file );
                default:
                    modApi.Log("Invalid DataSourceType");
                    return null;
            }
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
