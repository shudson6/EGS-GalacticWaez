using System;
using System.Collections.Generic;
using System.Threading;
using Eleon.Modding;

namespace GalacticWaez
{
    public delegate IEnumerable<VectorInt3> Pathfinder(
        GalaxyMap.Node start,
        GalaxyMap.Node goal,
        float warpRange
        );

    public class AstarPathfinder : IPathfinder
    {
        class PathNode
        {
            public IGalaxyNode Star { get; }
            public PathNode Previous { get; }
            public float PathCost { get; }
            public PathNode(IGalaxyNode star, PathNode previous)
            {
                Star = star;
                Previous = previous;
                PathCost = previous?.Star.Neighbors[star] + previous?.PathCost ?? 0;
            }
        }

        public IEnumerable<VectorInt3>
        FindPath(IGalaxyNode start, IGalaxyNode goal, float warpRange, CancellationToken token = default)
        {
            if (start == null || goal == null)
                throw new ArgumentNullException("FindPath: start or goal");
            if (warpRange < 1)
                throw new ArgumentOutOfRangeException("FindPath: warpRange must be positive");

            var visitedStars = new Dictionary<VectorInt3, PathNode>();
            var minheap = new Minheap<PathNode>();
            minheap.Insert(new PathNode(start, null), 0);
            PathNode goalNode = null;
            while (minheap.Count > 0)
            {
                var current = minheap.RemoveMin();
                if (visitedStars.TryGetValue(current.Star.Position, out PathNode existingPath)
                    && current.PathCost >= existingPath.PathCost)
                {
                    continue;
                }
                visitedStars[current.Star.Position] = current;
                foreach (var kv in current.Star.Neighbors)
                {
                    token.ThrowIfCancellationRequested();
                    if (kv.Value > warpRange)
                        continue;

                    var neighbor = new PathNode(kv.Key, current);
                    if (kv.Key == goal)
                    {
                        goalNode = neighbor;
                        minheap.Clear();
                        break;
                    }
                    minheap.Insert(neighbor, neighbor.PathCost + neighbor.Star.DistanceTo(goal));
                }
            }
            return ListifyPath(goalNode);
        }

        private static IEnumerable<VectorInt3> ListifyPath(PathNode node)
        {
            if (node == null) return null;
            var path = new List<VectorInt3>();
            for (; node != null; node = node.Previous)
            {
                path.Add(node.Star.Position);
            }
            path.Reverse();
            return path;
        }
    }
}
