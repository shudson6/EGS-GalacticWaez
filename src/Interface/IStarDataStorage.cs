using System.Collections.Generic;
using SectorCoordinates = Eleon.Modding.VectorInt3;

namespace GalacticWaez
{
    public interface IStarDataStorage
    {
        /// <summary>
        /// Determines if the storage associated with this instance exists.
        /// </summary>
        /// <returns>
        /// <c>true</c> if so, <c>false</c> otherwise
        /// </returns>
        bool Exists();

        /// <summary>
        /// Loads star positions from storage.
        /// </summary>
        /// <returns>
        /// A collection of star positions as sector coordinates, 
        /// or <c>null</c> if an error occurred.
        /// </returns>
        IEnumerable<SectorCoordinates> Load();

        /// <summary>
        /// Writes star positions to storage.
        /// </summary>
        /// <param name="positions">The collection of star positions to be written.</param>
        /// <returns>
        /// <c>true</c> on success, <c>false</c> on failure
        /// </returns>
        bool Store(IEnumerable<SectorCoordinates> positions);
    }
}