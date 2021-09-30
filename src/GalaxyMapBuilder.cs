using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;

namespace GalacticWaez
{
    public class GalaxyMapBuilder
    {
        private readonly LoggingDelegate Log;

        public GalaxyMapBuilder(LoggingDelegate Log) { this.Log = Log; }

        public GalaxyMap BuildGalaxyMap(IGalaxyDataSource source, float maxWarpRange)
        {
            var positions = source.GetGalaxyData();
            if (positions == null || !positions.Any())
            {
                Log("Galaxy data not available. Cannot create Galaxy Map.");
                return null;
            }
            var nodes = new List<GalaxyMap.Node>(positions.Count());
            var sw = Stopwatch.StartNew();
            foreach (var p in positions)
            {
                var current = new GalaxyMap.Node(p);
                foreach (var n in nodes)
                {
                    if (!AreCloseEnough(n, current, maxWarpRange)) continue;
                    float dist = n.DistanceTo(current);
                    if (dist > maxWarpRange) continue;
                    n.Neighbors.Add(current, dist);
                    current.Neighbors.Add(n, dist);
                }
                nodes.Add(current);
            }
            var g = new GalaxyMap(nodes);
            sw.Stop();
            float time = (float)sw.ElapsedMilliseconds / 1000;
            Log("Constructed galactic highway map: "
                + $"{g.Stars} stars, {g.WarpLines} warp lines. "
                + $"Took {time}s.");
            return g;
        }

        // checks the difference in a and b coordinates on each separate
        // axis to ensure that a cube of side length range could contain
        // both.
        // saves a lot of Math.sqrt() calls. When tested on a Reforged
        // Eden dump with 43000+ stars, reduced construction time by
        // over 3/4 (from 120+ seconds to fewer than 30).
        private static bool AreCloseEnough(GalaxyMap.Node a, GalaxyMap.Node b, float range)
        {
            return Math.Abs(a.Position.x - b.Position.x) <= range
                && Math.Abs(a.Position.y - b.Position.y) <= range
                && Math.Abs(a.Position.z - b.Position.z) <= range;
        }
}
}
