using System;
using System.Collections.Generic;
using System.Linq;
using Eleon.Modding;

namespace GalacticWaez
{
    public class GalaxyMap : IGalaxyMap
    {
        public class Node
        {
            // in test runs (some vanilla, some reforged eden), each star is in
            // warp range of over 60 others, on average, and that's without
            // warp upgrades. that doesn't make for a large dictionary, but this
            // needs to populate tens of thousands of them quickly
            // also, msdn says the capacity should not be divisible by a small prime
            public const int InitialNeighborCapacity = 131;
            public VectorInt3 Position { get; }
            public Dictionary<Node, float> Neighbors { get; }

            public Node(VectorInt3 position)
            {
                Position = position;
                Neighbors = new Dictionary<Node, float>(InitialNeighborCapacity);
            }


            public float DistanceTo(Node other)
            {
                float dx = Position.x - other.Position.x;
                float dy = Position.y - other.Position.y;
                float dz = Position.z - other.Position.z;
                return (float)Math.Sqrt(dx * dx + dy * dy + dz * dz);
            }
        }

        public IEnumerable<Node> Nodes { get; }

        /// <summary>
        /// Number of stars in the galaxy.
        /// </summary>
        public int Stars => Nodes.Count();

        /// <summary>
        /// Count of directed connections between stars, i.e. total possible warp jumps.
        /// </summary>
        public int WarpLines => Nodes.Aggregate(0, (acc, n) => acc + n.Neighbors.Count);

        public GalaxyMap(IEnumerable<Node> nodes) { Nodes = nodes; }

        /// <summary>
        /// Finds the node that matches the coordinates.
        /// </summary>
        public Node GetNode(VectorInt3 coordinates)
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
