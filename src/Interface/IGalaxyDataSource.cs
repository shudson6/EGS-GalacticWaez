using System.Collections.Generic;
using Eleon.Modding;

namespace GalacticWaez
{
    public interface IGalaxyDataSource
    {
        /// <summary>
        /// Retrieves galaxy data (star positions) from whatever data source this
        /// instance represents.
        /// </summary>
        /// <returns>
        /// the positions of stars within the galaxy, or <c>null</c> if they could not
        /// be retrieved
        /// </returns>
        IEnumerable<VectorInt3> GetGalaxyData();
    }
}
