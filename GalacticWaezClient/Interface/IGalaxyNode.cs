using Eleon.Modding;
using System.Collections.Generic;

namespace GalacticWaez
{
    public interface IGalaxyNode
    {
        Dictionary<IGalaxyNode, float> Neighbors { get; }
        VectorInt3 Position { get; }

        float DistanceTo(IGalaxyNode other);
    }
}