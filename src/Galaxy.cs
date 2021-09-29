using System;
using System.Linq;
using System.Collections.Generic;
using SectorCoordinates = Eleon.Modding.VectorInt3;

namespace GalacticWaez.Navigation
{
    public class Galaxy
    {
        private readonly IEnumerable<Node> nodes;

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

        public static Galaxy CreateNew(IEnumerable<SectorCoordinates> starPositions, float warpRange)
        {
            var nodes = new List<Node>(starPositions.Count());
            long warpLines = 0;
            foreach (var sp in starPositions)
            {
                var n = new Node(sp);
                foreach (var p in nodes)
                {
                    if (!AreCloseEnough(n, p, warpRange)) continue;
                    float dist = n.DistanceTo(p);
                    if (dist > warpRange) continue;
                    n.Neighbors.Add(p, dist);
                    p.Neighbors.Add(n, dist);
                }
                nodes.Add(n);
                warpLines += n.Neighbors.Count;
            }
            return new Galaxy(nodes, warpLines);
        }

        // checks the difference in a and b coordinates on each separate
        // axis to ensure that a cube of side length range could contain
        // both.
        // saves a lot of Math.sqrt() calls. When tested on a Reforged
        // Eden dump with 43000+ stars, reduced construction time by
        // over 3/4 (from 120+ seconds to fewer than 30).
        private static bool AreCloseEnough(Node a, Node b, float range)
        {
            return Math.Abs(a.Position.x - b.Position.x) <= range
                && Math.Abs(a.Position.y - b.Position.y) <= range
                && Math.Abs(a.Position.z - b.Position.z) <= range;
        }

        public int StarCount { get => nodes.Count(); }
        public long WarpLines { get; }

        private Galaxy(IEnumerable<Node> nodes, long warpLines)
        {
            this.nodes = nodes;
            WarpLines = warpLines;
        }

        /// <summary>
        /// Finds the node that matches the coordinates.
        /// </summary>
        public Node GetNode(LYCoordinates coords)
        {
            foreach (var n in nodes)
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
