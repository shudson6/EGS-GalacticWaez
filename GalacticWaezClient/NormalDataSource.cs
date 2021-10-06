using Eleon.Modding;
using System.Collections.Generic;
using System.Linq;

namespace GalacticWaez
{
    /// <summary>
    /// Provides star positions by first checking for a save file and, failing
    /// that, scanning memory for them.
    /// In the case that star positions are found by scanning memory, saves
    /// the data to the savegame.
    /// </summary>
    public class NormalDataSource : IGalaxyDataSource
    {
        public readonly IFileDataSource fileSource;
        public readonly IGalaxyDataSource scanSource;
        public readonly IGalaxyStorage storage;

        public NormalDataSource(IFileDataSource fileSource, IGalaxyDataSource scanSource,
            IGalaxyStorage storage)
        {
            this.fileSource = fileSource;
            this.scanSource = scanSource;
            this.storage = storage;
        }

        /// <summary>
        /// Attempts to load star data from the <see cref="IFileDataSource"/>. If this fails,
        /// attempts to retrieve data from the secondary source (intended to be a
        /// <see cref="StarFinderDataSource"/>). If found by the secondary, saves the data
        /// using the <see cref="IGalaxyStorage"/>.
        /// </summary>
        /// <returns>
        /// the star positions, or <c>null</c> if none found
        /// </returns>
        public IEnumerable<VectorInt3> GetGalaxyData()
        {
            var positions = fileSource?.GetGalaxyData() ?? null;
            if (positions == null || !positions.Any())
            {
                positions = scanSource?.GetGalaxyData() ?? null;
                // store the data if any were found;
                // there obviously wasn't a file if we're here
                if (positions != null && positions.Any())
                {
                    storage?.StoreGalaxyData(positions);
                }
                if (positions != null && !positions.Any())
                {
                    positions = null;
                }
            }
            return positions;
        }
    }
}
