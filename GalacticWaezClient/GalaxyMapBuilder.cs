using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using System.Threading;

namespace GalacticWaez
{
    public class GalaxyMapBuilder : IGalaxyMapBuilder
    {
        private readonly LoggingDelegate Log;

        public GalaxyMapBuilder(LoggingDelegate Log) 
        { 
            this.Log = Log ?? delegate { }; 
        }

        /// <summary>
        /// Creates a <see cref="GalaxyMap"/> using star positions retrieved by the given
        /// <see cref="IGalaxyDataSource"/>. Creates an edge (warp line) for each pair of
        /// stars within <c>maxWarpRange</c> of each other.
        /// </summary>
        /// <param name="source">an <see cref="IGalaxyDataSource"/> instance to provide
        /// star position data
        /// </param>
        /// <param name="maxWarpRange">maximum distance between two stars to create 
        /// a warp line (unit is sectors)</param>
        /// <returns>
        /// a new GalaxyMap, or <c>null</c> if data were unavailable
        /// </returns>
        public IGalaxyMap BuildGalaxyMap(IGalaxyDataSource source, float maxWarpRange,
            CancellationToken token = default)
        {
            CheckParams(source, maxWarpRange);
            var positions = source.GetGalaxyData();
            if (positions == null || !positions.Any())
            {
                Log("Galaxy data not available. Cannot create Galaxy Map.");
                return null;
            }
            Log("Constructing galaxy map...");
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
                token.ThrowIfCancellationRequested();
            }
            var g = new GalaxyMap(nodes);
            sw.Stop();
            float time = (float)sw.ElapsedMilliseconds / 1000;
            Log("Constructed galactic highway map: "
                + $"{g.Stars} stars, {g.WarpLines} warp lines. "
                + $"Took {time}s.");
            return g;
        }

        /// <summary> Validates the parameters, throwing exceptions if they are invalid. </summary>
        private void CheckParams(IGalaxyDataSource source, float maxWarpRange)
        {
            if (source == null) throw new ArgumentNullException("IGalaxyDataSource cannot be null.");
            if (maxWarpRange < 0)
                throw new ArgumentOutOfRangeException("BuildGalaxyMap: MaxWarpRange must be positive.");
        }

        /// <summary>
        /// checks the difference in a and b coordinates on each separate
        /// axis to ensure that a cube of side length range could contain
        /// both.
        /// saves a lot of Math.sqrt() calls. When tested on a Reforged
        /// Eden dump with 43000+ stars, reduced construction time by
        /// over 3/4 (from 120+ seconds to fewer than 30).
        /// </summary>
        private static bool AreCloseEnough(GalaxyMap.Node a, GalaxyMap.Node b, float range)
        {
            return Math.Abs(a.Position.x - b.Position.x) <= range
                && Math.Abs(a.Position.y - b.Position.y) <= range
                && Math.Abs(a.Position.z - b.Position.z) <= range;
        }
    }
}
