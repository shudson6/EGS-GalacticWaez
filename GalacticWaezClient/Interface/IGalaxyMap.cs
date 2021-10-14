using Eleon.Modding;
using System.Collections.Generic;

namespace GalacticWaez
{
    public interface IGalaxyMap
    {
        IEnumerable<GalaxyMap.Node> Nodes { get; }
        int Stars { get; }
        int WarpLines { get; }

        GalaxyMap.Node GetNode(VectorInt3 coordinates);
    }
}