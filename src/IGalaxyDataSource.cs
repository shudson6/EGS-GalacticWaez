using System.Collections.Generic;
using Eleon.Modding;

namespace GalacticWaez
{
    interface IGalaxyDataSource
    {
        ICollection<VectorInt3> GetGalaxyData();
    }
}
