using System;
using System.Linq;
using System.Collections.Generic;
using Eleon.Modding;
using static GalacticWaez.Const;

namespace GalacticWaez
{
    public class Galaxy
    {
        private readonly IEnumerable<Node> nodes;

        public class Node
        {
            public VectorInt3 Position { get; }
            public Dictionary<Node, float> Neighbors { get; }

            public Node(VectorInt3 position, Dictionary<Node, float> neighbors)
            {
                Position = position;
                Neighbors = neighbors;
            }

            public float DistanceTo(Node other)
            {
                int dx = Position.x - other.Position.x;
                int dy = Position.y - other.Position.y;
                int dz = Position.z - other.Position.z;
                return (float)Math.Sqrt(dx * dx + dy * dy + dz * dz);
            }
        }

        public static Galaxy CreateNew(IEnumerable<VectorInt3> starPositions, float warpRange)
        {
            var nodes = new List<Node>();
            long warpLines = 0;
            foreach (var sp in starPositions)
            {
                var n = new Node(new VectorInt3(sp.x / SectorsPerLY,
                        sp.y / SectorsPerLY, 
                        sp.z / SectorsPerLY),
                        new Dictionary<Node, float>());
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
        // Eden save with 43000+ stars, reduced construction time by
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

        // finds the node that matches the coordinates.
        // provide sector coordinates
        public Node GetNode(VectorInt3 coords)
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
