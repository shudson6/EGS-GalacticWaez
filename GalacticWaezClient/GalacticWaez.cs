using Eleon.Modding;
using System.Collections.Generic;
using System.Threading;
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

    public delegate void LoggingDelegate(string text);

    public abstract class GalacticWaez : IMod, ICommandHandler
    {
        /// <summary> Base value of player warp range, in sectors </summary>
        public const float DefaultBaseWarpRange = DefaultBaseWarpRangeLY * SectorsPerLY;
        /// <summary> Default maximum distance between stars to be considered neighbors, in sectors </summary>
        public const float DefaultMaxWarpRange = DefaultMaxWarpRangeLY * SectorsPerLY;
        /// <summary> Base value of player warp range, in LY, to which bonuses are added </summary>
        public const float DefaultBaseWarpRangeLY = 30;
        /// <summary> Default maximum distance between stars to be considered neighbors, in LY</summary>
        public const float DefaultMaxWarpRangeLY = 30;
        /// <summary> Default maximum time for a nav task before cancellation </summary>
        public const int DefaultNavTimeoutSeconds = 20;
        /// <summary> Default maximum time for a nav task before cancellation </summary>
        public const int DefaultNavTimeoutMillis = 1000 * DefaultNavTimeoutSeconds;
        /// <summary> Constant for converting between sectors and light-years </summary>
        public const int SectorsPerLY = 100000;

        public ModState Status { get; protected set; }
        public IConfiguration Config { get; private set; }

        private readonly ICommandHandler preInitCommandHandler = new PreInitCommandHandler();
        protected ChatMessageHandler ChatHandler
        {
            get => _chatHandler;
            set => SetChatHandler(value);
        }
        private CancellationTokenSource initCancelSource;
        private CancellationToken initCancelToken;
        protected IModApi ModApi { get; private set; }
        public IGalaxyMap Galaxy { get; private set; }
        public IPlayerProvider PlayerProvider { get; protected set; }

        private Task<bool> init = null;
        private ChatMessageHandler _chatHandler;

        public virtual void Init(IModApi modApi)
        {
            ModApi = modApi;
            Status = ModState.Uninitialized;
            Config = new ConfigLoader(ModApi.Log)
                .LoadConfig(ModApi.Application.GetPathFor(AppFolder.Mod) + "\\GalacticWaez\\config.ecf");
        }

        public virtual void Shutdown()
        {
            ChatHandler = null;
        }

        protected abstract IPlayerProvider CreatePlayerProvider();

        public bool HandleCommand(string cmdToken, string args, IPlayerInfo player, IResponder responder)
        {
            if (cmdToken == "restart")
            {
                HandleRestart(args, player, responder);
                return true;
            } 
            if (cmdToken == "store")
            {
                HandleStore(args, player, responder);
                return true;
            }

            if (cmdToken != "status" || !(args == null || args == ""))
                return false;

            responder.Send(Status.ToString());
            return true;
        }

        // IPlayerInfo parameter is here anticipating a permission check; for now it is ignored
        private void HandleStore(string arg, IPlayerInfo _, IResponder responder)
        {
            if (arg == null)
                arg = "";
            var storage = new FileDataSource(ModApi.Application.GetPathFor(AppFolder.SaveGame), ModApi.Log);
            if ((!storage.Exists && arg == "") || arg == "--replace")
            {
                if (storage.StoreGalaxyData(Galaxy.StarPositions))
                {
                    responder?.Send($"Wrote {Galaxy.Stars} star positions.");
                }
                else
                {
                    responder?.Send("Write operation failed.");
                }
            }
            else if (storage.Exists && arg == "")
            {
                responder?.Send("Stored data already exists. To replace it, use option --replace");
            }
            else
            {
                responder?.Send("Invalid argument.");
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
            var handlers = new List<ICommandHandler>(ChatHandler.CommandHandlers);
            foreach (var handler in handlers)
            {
                if (handler is NavigationHandler)
                    ChatHandler.RemoveHandler(handler);
            }

            ModApi.Log($"Restart requested by player {player.Id} ({player.Name}) with data source {arg}");
            responder?.Send("Restarting with data source " + arg);
            SetChatHandler(CreateChatHandler(CreatePlayerProvider()));
            Setup(type);
        }

        private void SetChatHandler(ChatMessageHandler handler)
        {
            ModApi.Log("Setting ChatHandler");
            if (_chatHandler != null)
                ModApi.Application.ChatMessageSent -= _chatHandler.HandleChatMessage;

            _chatHandler = handler;
            if (handler != null)
                ModApi.Application.ChatMessageSent += handler.HandleChatMessage;
        }

        protected ChatMessageHandler CreateChatHandler(IPlayerProvider pp)
        {
            return new ChatMessageHandler(pp,
                new ResponseManager(ModApi.Application),
                this, preInitCommandHandler,
                new HelpHandler(),
                new DebugCommandHandler(this));
        }

        protected void Setup(DataSourceType type)
        {
            if (init != null)
            {
                initCancelSource.Cancel();
                try { init.Wait(); }
                catch { /* don't care; it's done */ }
                initCancelSource.Dispose();
            }
            initCancelSource = new CancellationTokenSource();
            initCancelToken = initCancelSource.Token;
            init = Task<bool>.Factory.StartNew(() => SetupTask(type, initCancelToken), initCancelToken);
            ModApi.Application.Update += OnUpdateDuringInit;
        }

        private bool SetupTask(DataSourceType type, CancellationToken cancelToken)
        {
            Status = ModState.Initializing;
            string saveGameDir = ModApi.Application.GetPathFor(AppFolder.SaveGame);

            // start with the GalaxyMap: the longest and most likely to fail
            var ksp = new KnownStarProvider(saveGameDir, ModApi.Log);
            var source = CreateDataSource(type, ksp, saveGameDir);
            Galaxy = new GalaxyMapBuilder(ModApi.Log)
                .BuildGalaxyMap(source, Config.MaxWarpRange, cancelToken);
            if (Galaxy == null)
                return false;

            // assemble the navigator
            var bm = new BookmarkManager(saveGameDir, ModApi.Log);
            var nav = new Navigator(Galaxy, new AstarPathfinder(), bm, ksp, ModApi.Log,
                () => ModApi.Application.GameTicks, Config.NavTimeoutMillis);
            var nh = new NavigationHandler(nav);

            // finish assembling the chat message handler
            ChatHandler.AddHandler(nh);
            ChatHandler.AddHandler(new BookmarkHandler(bm, ModApi.Log));
            ChatHandler.RemoveHandler(preInitCommandHandler);
            return true;
        }

        private IGalaxyDataSource CreateDataSource(
            DataSourceType type, IKnownStarProvider ksp, string saveGameDir)
        {
            switch (type)
            {
                case DataSourceType.FileOnly:
                    return new FileDataSource(saveGameDir, ModApi.Log);
                case DataSourceType.ScanOnly:
                    return new StarFinderDataSource(ksp, ModApi.Log);
                case DataSourceType.Normal:
                    // yes, FileDataSource implements both interfaces so is passed twice to NormalDataSource
                    var file = new FileDataSource(saveGameDir, ModApi.Log);
                    return new NormalDataSource(
                        file,
                        new StarFinderDataSource(ksp, ModApi.Log),
                        file );
                default:
                    ModApi.Log("Invalid DataSourceType");
                    return null;
            }
        }

        private void OnUpdateDuringInit()
        {
            if (init == null || init.Status < TaskStatus.RanToCompletion)
                return;

            ModApi.Application.Update -= OnUpdateDuringInit;
            if (init.Status == TaskStatus.RanToCompletion && init.Result == true)
            {
                Status = ModState.Ready;
                OnInitDone();
            }
            else if (init.Status == TaskStatus.Canceled)
            {
                Status = ModState.Uninitialized;
                ModApi.Log("Canceled: " + init.Id);
            }
            else
            {
                Status = ModState.InitFailed;
                OnInitFail();
            }
            // either way, we're done with it
            init.Dispose();
            init = null;
        }

        protected virtual void OnInitDone() { }
        protected virtual void OnInitFail() { }
    }
}