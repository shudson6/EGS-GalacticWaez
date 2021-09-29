using System.Collections.Generic;

namespace GalacticWaez.Navigation
{
    public delegate IEnumerable<LYCoordinates> Pathfinder(
        GalaxyMap.Node start,
        GalaxyMap.Node goal,
        float warpRange
        );

    public class AstarPathfinder
    {
        class PathNode
        {
            public GalaxyMap.Node Star { get; }
            public PathNode Previous { get; }
            public float PathCost { get; }
            public PathNode(GalaxyMap.Node star, PathNode previous)
            {
                Star = star;
                Previous = previous;
                PathCost = previous?.Star.Neighbors[star] + previous?.PathCost ?? 0;
            }
        }

        /**
         * Find a path from start to goal. The warpRange parameter can be used
         * to limit the distance of any single jump. Values of warpRange larger
         * than the distance used to build the Galaxy have no effect.
         */
        public static IEnumerable<LYCoordinates> 
        FindPath(GalaxyMap.Node start, GalaxyMap.Node goal, float warpRange)
        {
            var visitedStars = new Dictionary<LYCoordinates, PathNode>();
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

        private static IEnumerable<LYCoordinates> ListifyPath(PathNode node)
        {
            if (node == null) return null;
            var path = new List<LYCoordinates>();
            for(; node != null; node = node.Previous)
            {
                path.Add(node.Star.Position);
            }
            path.Reverse();
            return path;
        }
    }
}
