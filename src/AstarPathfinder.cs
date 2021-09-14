using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Eleon.Modding;

namespace GalacticWaez
{
    class AstarPathfinder
    {
        class PathNode
        {
            public Galaxy.Node Star { get; }
            public PathNode Previous { get; }
            public float PathCost { get; }
            public PathNode(Galaxy.Node star, PathNode previous)
            {
                Star = star;
                Previous = previous;
                PathCost = previous?.Star.Neighbors[star] + previous?.PathCost ?? 0;
            }
        }

        public static IEnumerable<LYCoordinates> FindPath(Galaxy.Node start, Galaxy.Node goal)
        {
            var visitedStars = new Dictionary<LYCoordinates, PathNode>();
            var minheap = new Minheap<PathNode>();
            minheap.Insert(new PathNode(start, null), 0);
            PathNode goalNode = null;
            while (minheap.Count > 0)
            {
                var current = minheap.RemoveMin();
                PathNode existingPath;
                if (visitedStars.TryGetValue(current.Star.Position, out existingPath)
                    && current.PathCost >= existingPath.PathCost)
                {
                    continue;
                }
                visitedStars[current.Star.Position] = current;
                foreach (var kv in current.Star.Neighbors)
                {
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

        static IEnumerable<LYCoordinates> ListifyPath(PathNode node)
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
