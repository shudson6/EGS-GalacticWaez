using System;
using System.Collections.Generic;
using System.Linq;
using SectorCoordinates = Eleon.Modding.VectorInt3;

namespace GalacticWaez
{
    public class GalaxyMap
    {
        public class Node
        {
            // in test runs (some vanilla, some reforged eden), each star is in
            // warp range of over 60 others, on average, and that's without
            // warp upgrades. that doesn't make for a large dictionary, but this
            // needs to populate tens of thousands of them quickly
            // also, msdn says the capacity should not be divisible by a small prime
            public const int InitialNeighborCapacity = 131;
            public LYCoordinates Position { get; }
            public Dictionary<Node, float> Neighbors { get; }

            public Node(LYCoordinates position)
            {
                Position = position;
                Neighbors = new Dictionary<Node, float>(InitialNeighborCapacity);
            }

            public Node(SectorCoordinates position) : this(new LYCoordinates(position))
            { }

            public float DistanceTo(Node other)
            {
                int dx = Position.x - other.Position.x;
                int dy = Position.y - other.Position.y;
                int dz = Position.z - other.Position.z;
                return (float)Math.Sqrt(dx * dx + dy * dy + dz * dz);
            }
        }

        public IEnumerable<Node> Nodes { get; }
        public int Stars => Nodes.Count();

        /// <summary>
        /// 
        /// </summary>
        public int WarpLines => Nodes.Aggregate(0, (acc, n) => acc + n.Neighbors.Count);

        public GalaxyMap(IEnumerable<Node> nodes) { Nodes = nodes; }

        /// <summary>
        /// Finds the node that matches the coordinates.
        /// </summary>
        public Node GetNode(LYCoordinates coords)
        {
            foreach (var n in Nodes)
            {
                if (n.Position.Equals(coords))
                {
                    return n;
                }
            }
            return null;
        }
    }
}
