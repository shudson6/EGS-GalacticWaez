using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Eleon.Modding;
using GalacticWaez.Navigation;
using SectorCoordinates = Eleon.Modding.VectorInt3;

namespace GalacticWaez
{
    class Initializer
    {
        public delegate void DoneCallback(Galaxy galaxy, string message);

        private readonly IModApi modApi;
        private DoneCallback doneCallback;
        private Galaxy galaxy;
        private Task<string> init;

        public Initializer(IModApi modApi)
        {
            this.modApi = modApi;
        }

        public void Initialize(DoneCallback doneCallback)
        {
            this.doneCallback = doneCallback;
            init = Task<string>.Factory.StartNew(BuildGalaxyMap);
            modApi.Application.Update += OnUpdateDuringInit;
        }

        private string BuildGalaxyMap()
        {
            var db = new SaveGameDB(modApi);
            var knownStar = db.GetFirstKnownStarPosition();
            var message = new StringBuilder();
            var stars = FindStarData(knownStar, message);
            float range = db.GetLocalPlayerWarpRange();
            galaxy = CreateGalaxy(stars, range, message);
            return message.ToString();
        }

        private Galaxy CreateGalaxy(IEnumerable<SectorCoordinates> locations, float range, StringBuilder msg)
        {
            var stopwatch = Stopwatch.StartNew();
            var g = Galaxy.CreateNew(locations, range);
            stopwatch.Stop();
            float time = (float)stopwatch.ElapsedMilliseconds / 1000;
            msg.AppendLine("Constructed galactic highway map: "
                + $"{g.StarCount} stars, {g.WarpLines} warp lines. "
                + $"Took {time}s.");
            return g;
        }

        private IEnumerable<SectorCoordinates> FindStarData(SectorCoordinates known, StringBuilder msg)
        {
            var stored = new StarDataStorage(modApi);
            if (stored.Exists())
            {
                var loaded = stored.Load();
                msg.AppendLine($"Loaded {loaded.Count()} stars from file.");
                return loaded;
            }
            msg.AppendLine("No stored star data; scanning memory.");
            var found = ScrapeForStarData(known, msg);
            if ((found?.Count() ?? 0) > 0)
            {
                msg.AppendLine("Writing star positions to file...");
                stored.Store(found);
            }
            return found;
        }

        private IEnumerable<SectorCoordinates> ScrapeForStarData(SectorCoordinates known, StringBuilder msg)
        {
            var stopwatch = Stopwatch.StartNew();
            var stars = new StarFinder().Search(known);
            stopwatch.Stop();
            if (stars == null)
            {
                msg.AppendLine("Failed to locate star position data. "
                    + $"Scan took {stopwatch.ElapsedMilliseconds}ms.");
                return null;
            }
            msg.AppendLine($"Located {stars.Length} stars in {stopwatch.ElapsedMilliseconds}ms.");
            return stars;
        }

        private void OnUpdateDuringInit()
        {
            if (!init.IsCompleted) return;

            modApi.Application.Update -= OnUpdateDuringInit;
            doneCallback(galaxy, init.Result);
        }
    }
}
