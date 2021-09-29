using System.Collections.Generic;
using Eleon.Modding;

namespace GalacticWaez
{
    public interface IGalaxyDataSource
    {
        IEnumerable<VectorInt3> GetGalaxyData();
    }
}
