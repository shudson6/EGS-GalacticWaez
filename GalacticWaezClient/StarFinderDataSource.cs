using Eleon.Modding;
using System.Collections.Generic;

namespace GalacticWaez
{
    /// <summary>
    /// <see cref="IGalaxyDataSource"/> that uses a <see cref="StarFinder"/>
    /// to find star position data residing in memory.
    /// </summary>
    public class StarFinderDataSource : IGalaxyDataSource
    {
        private readonly LoggingDelegate Log;
        private readonly IKnownStarProvider db;

        public StarFinderDataSource(IKnownStarProvider db, LoggingDelegate log) 
        { 
            this.db = db;
            Log = log ?? delegate { };
        }

        public IEnumerable<VectorInt3> GetGalaxyData()
        {
            if (db != null && db.GetFirstKnownStarPosition(out VectorInt3 known))
            {
                Log("Performing memory scan...");
                return new StarFinder().Search(known);
            }
            Log("Failed to retrieve any known star position.");
            return null;
        }
    }
}
