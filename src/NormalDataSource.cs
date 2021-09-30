using Eleon.Modding;
using System.Collections.Generic;
using System.IO;

namespace GalacticWaez.src
{
    /// <summary>
    /// Provides star positions by first checking for a save file and, failing
    /// that, scanning memory for them.
    /// </summary>
    class NormalDataSource : IGalaxyDataSource
    {
        private readonly string SaveGameDir;
        private readonly LoggingDelegate Log;

        public NormalDataSource(string saveGameDir, LoggingDelegate logger)
        {
            SaveGameDir = saveGameDir;
            Log = logger;
        }

        public IEnumerable<VectorInt3> GetGalaxyData()
        {
            var fileSource = new FileDataSource(SaveGameDir, Log);
            if (File.Exists(fileSource.PathToFile))
            {
                return fileSource.GetGalaxyData();
            }
            Log("Saved star data not found. Beginning memory scan...");
            if (new KnownStarProvider(SaveGameDir, Log).GetFirstKnownStarPosition(out VectorInt3 known))
            {
                return new StarFinder().Search(known);
            }
            Log("Failed to retrieve first known star position.");
            return null;
        }
    }
}
