using Eleon.Modding;
using System.Collections.Generic;
using System.Linq;

namespace GalacticWaez
{
    /// <summary>
    /// Provides star positions by first checking for a save file and, failing
    /// that, scanning memory for them.
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
