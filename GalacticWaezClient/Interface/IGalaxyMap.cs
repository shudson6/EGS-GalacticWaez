using Eleon.Modding;
using System.Collections.Generic;

namespace GalacticWaez
{
    public interface IGalaxyMap
    {
        IEnumerable<VectorInt3> StarPositions { get; }
        int Stars { get; }
        int WarpLines { get; }
        int Range { get; }

        IGalaxyNode GetNode(VectorInt3 coordinates);
    }
}