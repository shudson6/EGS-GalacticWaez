using System.Collections.Generic;
using System.Linq;

namespace GalacticWaez.Navigation
{
    // TODO: many of the logging calls (or all of them) should be chat messages
    public class Navigator : INavigator
    {
        private struct NavResult
        {
            public IEnumerable<LYCoordinates> path;
            public string message;
        }

        private readonly GalaxyMap Galaxy;
        private readonly IPathfinder Pathfinder;
        private readonly IBookmarkManager Bookmarks;
        private readonly IKnownStarProvider KnownStars;
        private readonly LoggingDelegate Log;

        public Navigator(GalaxyMap galaxy, IPathfinder pathfinder, 
            IBookmarkManager bookmarkManager, IKnownStarProvider starProvider, LoggingDelegate log)
        {
            Pathfinder = pathfinder;
            Log = log;
            Galaxy = galaxy;
            Bookmarks = bookmarkManager;
            KnownStars = starProvider;
        }

        public void Navigate(IPlayerInfo player, string destination, float playerRange, IResponder response)
        {
            var start = Galaxy.GetNode(player.GetCurrentStarCoordinates());
            var goal = GoalNode(player.Player.Id, player.Player.Faction.Id, destination, out bool isBookmark);
            if (goal == null)
            {
                response?.Send($"No bookmark or known star by name {destination}");
                return;
            }
            if (start.Equals(goal))
            {
                response?.Send("It appears you are already there.");
                return;
            }
            var path = Pathfinder.FindPath(start, goal, playerRange);
            if (path == null)
            {
                response?.Send("No path found.");
                return;
            }
            if (path.Count() == 1)
            {
                // this is impossible, i think
                response?.Send("Are you already there?");
                return;
            }
            if (path.Count() == 2 && isBookmark)
            {
                response?.Send("It appears you are already in warp range.");
                return;
            }
            int set = SetWaypoints(path.Skip(1), player.Player.Id, isBookmark);
        }

        private int SetWaypoints(IEnumerable<LYCoordinates> path, int playerId, bool goalIsBookmark)
        {
            return 0;
            //if (goalIsBookmark)
            //{
            //    path = path.Take(path.Count() - 1);
            //}
            //var sectorsPath = path.Select(n => n.ToSectorCoordinates());
            //int added = db.InsertBookmarks(sectorsPath, playerId, modApi.Application.GameTicks);
            //string text = $"Path found; {added}/{path.Count()} waypoints added.";
            //modApi.Log(text);
            //return new NavResult { path = path, message = text };
        }

        private GalaxyMap.Node GoalNode(int playerId, int playerFacId, string goalName, out bool isBookmark)
        {
            isBookmark = Bookmarks.TryGetVector(playerId, playerFacId, goalName, out var coords);
            if (!isBookmark && !KnownStars.GetPosition(goalName, out coords))
            {
                return null;
            }
            return Galaxy.GetNode(coords);
        }
    }
}
