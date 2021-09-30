using Eleon.Modding;
using System.Collections.Generic;

namespace GalacticWaez
{
    public class StarFinderDataSource : IGalaxyDataSource
    {
        private readonly IKnownStarProvider db;

        public StarFinderDataSource(IKnownStarProvider db) { this.db = db; }

        public IEnumerable<VectorInt3> GetGalaxyData()
        {
            if (db.GetFirstKnownStarPosition(out VectorInt3 known))
            {
                return new StarFinder().Search(known);
            }
            return null;
        }
    }
}
