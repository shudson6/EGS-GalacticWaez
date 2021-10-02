using System;
using System.Collections.Generic;
using System.Linq;

namespace GalacticWaez.Navigation
{
    // TODO: many of the logging calls (or all of them) should be chat messages
    public class Navigator : INavigator
    {
        private readonly GalaxyMap Galaxy;
        private readonly IPathfinder Pathfinder;
        private readonly IBookmarkManager Bookmarks;
        private readonly IKnownStarProvider KnownStars;
        private readonly LoggingDelegate Log;
        private readonly Func<ulong> GetTicks;

        public Navigator(GalaxyMap galaxy, IPathfinder pathfinder, 
            IBookmarkManager bookmarkManager, IKnownStarProvider starProvider, 
            LoggingDelegate log, Func<ulong> getTicks)
        {
            Pathfinder = pathfinder;
            Log = log;
            Galaxy = galaxy;
            Bookmarks = bookmarkManager;
            KnownStars = starProvider;
            GetTicks = getTicks;
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
            // we don't need the first step; we're already there
            path = path.Skip(1);
            // if there's already a bookmark on the goal, we don't need another one
            if (isBookmark)
                path = path.Take(path.Count() - 1);
            // respond with how many bookmarks got added vs expected
            int expected = path.Count();
            int actual = SetWaypoints(path, player.Player.Id, player.Player.Faction.Id);
            response?.Send($"Path found; {actual}/{expected} waypoints added.");
            return;
        }

        private int SetWaypoints(IEnumerable<LYCoordinates> path, int playerId, int playerFacId)
        {
            var bmdata = new BookmarkData
            {
                PlayerId = playerId,
                PlayerFacId = playerFacId,
                FacGroup = 1,
                Icon = 2,
                IsShared = false,
                IsWaypoint = true,
                IsRemove = true,
                IsShowHud = true,
                GameTime = GetTicks(),
                MaxDistance = -1
            };
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
