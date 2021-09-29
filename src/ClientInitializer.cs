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
    /// Used by CommandHandler to build Galaxy
    /// </summary>
    public class ClientInitializer : IInitializer
    {

        private readonly IModApi modApi;
        private readonly IStarFinder starFinder;
        private readonly IStarDataStorage storage;
        private readonly ISaveGameDB db;
        private InitializerCallback doneCallback;
        private Task<GalaxyMap> init;

        public ClientInitializer(IModApi modApi)
            : this(modApi, new StarDataStorage(modApi.Application.GetPathFor(AppFolder.SaveGame),
                "stardata.csv"),
                new StarFinder(), new SaveGameDB(modApi))
        { }

        public ClientInitializer(IModApi modApi, IStarDataStorage storage,
            IStarFinder starFinder, ISaveGameDB db)
        {
            this.modApi = modApi;
            this.starFinder = starFinder;
            this.storage = storage;
            this.db = db;
        }

        public void Initialize(InitializerType source, InitializerCallback doneCallback)
        {
            this.doneCallback = doneCallback;
            init = Task<GalaxyMap>.Factory.StartNew(function: BuildGalaxyMap, state: source);
            modApi.Application.Update += OnUpdateDuringInit;
        }

        private GalaxyMap BuildGalaxyMap(object obj)
        {
            IEnumerable<SectorCoordinates> stars = null;
            var source = (InitializerType)obj;
            switch (source)
            {
                case InitializerType.Normal:
                    if (storage.Exists())
                    {
                        stars = LoadStarData();
                    }
                    else
                    {
                        modApi.Log("No saved star positions. Beginning scan...");
                        stars = ScanForStarData(true);
                    }
                    break;

                case InitializerType.FileOnly:
                    stars = LoadStarData();
                    break;

                case InitializerType.ScanOnly:
                    stars = ScanForStarData();
                    break;
            }
            return null;
        }

        private IEnumerable<SectorCoordinates> LoadStarData()
        {
            if (!storage.Exists())
            {
                modApi.LogWarning("Stored star data not found.");
                return null;
            }
            var loaded = storage.Load();
            if (loaded != null)
            {
                modApi.Log($"Loaded {loaded.Count()} stars from file.");
            }
            else
            {
                modApi.LogError("Failed to load star data from file.");
            }
            return loaded;
        }

        /// <summary>
        /// Uses StarFinder to scan memory for star position data.
        /// If save == true, saves the found data.
        /// </summary>
        /// <param name="save">
        /// Set to true to save the found star data.
        /// Defaults to false.
        /// </param>
        /// <returns>
        /// A collection containing the positions of the stars.
        /// </returns>
        private IEnumerable<SectorCoordinates> ScanForStarData(bool save = false)
        {
            var known = db.GetFirstKnownStarPosition();
            var stopwatch = Stopwatch.StartNew();
            var stars = starFinder.Search(known);
            stopwatch.Stop();
            if (stars == null)
            {
                modApi.LogWarning("Failed to locate star position data. "
                    + $"Scan took {stopwatch.ElapsedMilliseconds}ms.");
                return null;
            }
            modApi.Log($"Located {stars.Length} stars in {stopwatch.ElapsedMilliseconds}ms.");
            if (save)
            {
                if (storage.Store(stars))
                {
                    modApi.Log("Saved star positions to file.");
                }
                else
                {
                    modApi.LogWarning("Could not save star positions to file.");
                }
            }
            return stars;
        }

        private void OnUpdateDuringInit()
        {
            if (init.Status < TaskStatus.RanToCompletion)
                return;

            modApi.Application.Update -= OnUpdateDuringInit;
            doneCallback(
                init.IsCompleted ? new CommandHandler(modApi, init.Result) : null,
                init.Exception
                );
        }
    }
}
