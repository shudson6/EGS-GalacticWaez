using System.Collections.Generic;
using SectorCoordinates = Eleon.Modding.VectorInt3;

namespace GalacticWaez
{
    public interface IGalaxyStorage
    {
        bool Exists { get; }

        /// <summary>
        /// Writes star positions to storage. Existing storage (if any) is overwritten.
        /// </summary>
        /// <param name="positions">The collection of star positions to be written.</param>
        /// <returns>
        /// <c>true</c> on success, <c>false</c> on failure
        /// </returns>
        bool StoreGalaxyData(IEnumerable<SectorCoordinates> positions);
    }
}