using Eleon.Modding;
using System.Collections.Generic;

namespace GalacticWaez
{
    public interface IGalaxyMap
    {
        IEnumerable<IGalaxyNode> Nodes { get; }
        int Stars { get; }
        int WarpLines { get; }

        IGalaxyNode GetNode(VectorInt3 coordinates);
    }
}