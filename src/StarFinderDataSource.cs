using Eleon.Modding;
using System.Collections.Generic;

namespace GalacticWaez
{
    class StarFinderDataSource : IGalaxyDataSource
    {
        private readonly ISaveGameDB db;

        public StarFinderDataSource(ISaveGameDB db) { this.db = db; }

        public IEnumerable<VectorInt3> GetGalaxyData()
            => new StarFinder().Search(db.GetFirstKnownStarPosition());
    }
}
