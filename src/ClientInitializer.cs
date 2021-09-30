using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Eleon.Modding;
using GalacticWaez.Command;
using SectorCoordinates = Eleon.Modding.VectorInt3;

namespace GalacticWaez
{
    /// <summary>
    /// Creates the galactic highway map and assembles the necessary objects to get
    /// Waez up and running happily.
    /// </summary>
    public class ClientInitializer : IInitializer
    {
        private readonly string SaveGameDir;
        private readonly IGalaxyDataSource DataSource;
        private readonly LoggingDelegate Log;

        private UpdateDelegate appUpdate;
        private InitializerCallback doneCallback;
        private Task<ICommandHandler> init;

        public ClientInitializer(string saveGameDir, InitializerType type, 
            UpdateDelegate appUpdate, LoggingDelegate log)
        {
            Log = log;
            this.appUpdate = appUpdate;
            SaveGameDir = saveGameDir;
            DataSource = CreateDataSource(type);
        }

        public void Initialize(InitializerCallback doneCallback)
        {
            this.doneCallback = doneCallback;
            init = Task<ICommandHandler>.Factory.StartNew(Setup);
            appUpdate += OnUpdateDuringInit;
        }

        private ICommandHandler Setup()
        {
            var galaxy = new GalaxyMapBuilder(Log).BuildGalaxyMap(DataSource, Const.DefaultMaxWarpRangeLY);
            return null;
        }

        private void OnUpdateDuringInit()
        {
            if (init.Status < TaskStatus.RanToCompletion)
                return;

            appUpdate -= OnUpdateDuringInit;
            doneCallback(
                init.IsCompleted ? init.Result : null,
                init.Exception
                );
        }

        private IGalaxyDataSource CreateDataSource(InitializerType type)
        {
            switch (type)
            {
                case InitializerType.Normal:
                    return new NormalDataSource(
                        new FileDataSource(SaveGameDir, Log),
                        new StarFinderDataSource(new KnownStarProvider(SaveGameDir, Log), Log));

                case InitializerType.FileOnly:
                    return new FileDataSource(SaveGameDir, Log);

                case InitializerType.ScanOnly:
                    return new StarFinderDataSource(new KnownStarProvider(SaveGameDir, Log), Log);

                default:
                    return null;
            }
        }
    }
}
