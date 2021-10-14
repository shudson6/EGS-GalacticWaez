using System;
using System.Collections.Generic;
using System.Linq;
using Eleon.Modding;

namespace GalacticWaez
{
    public class GalaxyMap : IGalaxyMap
    {
        public class Node : IGalaxyNode
        {
            // in test runs (some vanilla, some reforged eden), each star is in
            // warp range of over 60 others, on average, and that's without
            // warp upgrades. that doesn't make for a large dictionary, but this
            // needs to populate tens of thousands of them quickly
            // also, msdn says the capacity should not be divisible by a small prime
            public const int InitialNeighborCapacity = 131;
            public VectorInt3 Position { get; }
            public Dictionary<IGalaxyNode, float> Neighbors { get; }

            public Node(VectorInt3 position)
            {
                Position = position;
                Neighbors = new Dictionary<IGalaxyNode, float>(InitialNeighborCapacity);
            }


            public float DistanceTo(IGalaxyNode other)
            {
                float dx = Position.x - other.Position.x;
                float dy = Position.y - other.Position.y;
                float dz = Position.z - other.Position.z;
                return (float)Math.Sqrt(dx * dx + dy * dy + dz * dz);
            }
        }

        public IEnumerable<VectorInt3> StarPositions => Nodes.Select(n => n.Position);

        /// <summary>
        /// Number of stars in the galaxy.
        /// </summary>
        public int Stars => StarPositions.Count();

        /// <summary>
        /// Count of directed connections between stars, i.e. total possible warp jumps.
        /// </summary>
        public int WarpLines { get; }
        public int Range { get; }

        private readonly IEnumerable<IGalaxyNode> Nodes;

        public GalaxyMap(IEnumerable<Node> nodes, int range) 
        { 
            Nodes = nodes;
            WarpLines = Nodes.Aggregate(0, (acc, n) => acc + n.Neighbors.Count);
            Range = range;
        }

        /// <summary>
        /// Finds the node that matches the coordinates.
        /// </summary>
        public IGalaxyNode GetNode(VectorInt3 coordinates)
        {
            foreach (var n in Nodes)
            {
                if (n.Position.Equals(coordinates))
                {
                    return n;
                }
            }
            return null;
        }
    }
}
