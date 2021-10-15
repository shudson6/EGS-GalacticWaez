using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Eleon.Modding;

namespace GalacticWaez
{
    public class Navigator : INavigator
    {
        private readonly IPathfinder Pathfinder;
        private readonly IBookmarkManager Bookmarks;
        private readonly IKnownStarProvider KnownStars;
        private readonly LoggingDelegate Log;
        private readonly Func<ulong> GetTicks;
        private readonly int TimeoutMillis;

        public IGalaxyMap Galaxy { get; }

        public Navigator(IGalaxyMap galaxy, IPathfinder pathfinder, 
            IBookmarkManager bookmarkManager, IKnownStarProvider starProvider, 
            LoggingDelegate log, Func<ulong> getTicks, int timeoutMillis)
        {
            Galaxy = galaxy ??
                throw new ArgumentNullException("Navigator: Galaxy");
            Pathfinder = pathfinder ??
                throw new ArgumentNullException("Navigator: Pathfinder");
            Log = log ?? delegate { };
            Bookmarks = bookmarkManager ??
                throw new ArgumentNullException("Navigator: BookmarkManager");
            KnownStars = starProvider ??
                throw new ArgumentNullException("Navigator: KnownStarProvider");
            GetTicks = getTicks ??
                throw new ArgumentNullException("Navigator: GetTicks");
            if (timeoutMillis <= 0)
                throw new ArgumentOutOfRangeException("Navigate: timeoutMillis must be > 0");
            TimeoutMillis = timeoutMillis;
        }

        public Task<IEnumerable<VectorInt3>> Navigate(
            IPlayerInfo player, string destination, float playerRange, IResponder response)
        {
            if (player == null)
                throw new ArgumentNullException("Navigate: player");
            if (destination == null)
                throw new ArgumentNullException("Navigate: destination");
            if (playerRange <= 0)
                throw new ArgumentOutOfRangeException("Navigate: playerRange must be positive");

            var source = new CancellationTokenSource();
            var token = source.Token;
            var task = Task.Factory.StartNew(
                () => DoNavigate(player, destination, playerRange, response, token), token);
            task.ContinueWith((nav) =>
            {
                if (nav.IsCanceled)
                    response.Send($"Navigation to {destination} failed due to timeout.");
            });
            source.CancelAfter(TimeoutMillis);
            return task;
        }

        public IEnumerable<VectorInt3> DoNavigate(
            IPlayerInfo player, string destination, float playerRange, 
            IResponder response, CancellationToken token)
        {
            var start = Galaxy.GetNode(player.StarCoordinates);
            var goal = GoalNode(player.Id, player.FactionId, destination, out bool isBookmark);
            if (goal == null)
            {
                response?.Send($"No bookmark or known star by name {destination}");
                return null;
            }
            if (start.Equals(goal))
            {
                response?.Send("It appears you are already there.");
                return null;
            }
            var path = Pathfinder.FindPath(start, goal, playerRange, token);
            if (path == null)
            {
                response?.Send("No path found.");
                return null;
            }
            if (path.Count() == 1)
            {
                // this is impossible, i think
                response?.Send("Are you already there?");
                return null;
            }
            if (path.Count() == 2 && isBookmark)
            {
                response?.Send("It appears you are already in warp range.");
                return null;
            }
            // we don't need the first step; we're already there
            var bookmarks = path.Skip(1);
            // if there's already a bookmark on the goal, we don't need another one
            if (isBookmark)
                bookmarks = bookmarks.Take(bookmarks.Count() - 1);
            // respond with how many bookmarks got added vs expected
            int expected = bookmarks.Count();
            int actual = SetWaypoints(bookmarks, player.Id, player.FactionId);
            string message = $"Path found; {actual}/{expected} waypoints added.";
            response?.Send(message);
            if (response == null)
                Log(message);
            return path;
        }

        private int SetWaypoints(IEnumerable<VectorInt3> path, int playerId, int playerFacId)
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
            return Bookmarks.InsertBookmarks(path, bmdata);
        }

        private IGalaxyNode GoalNode(int playerId, int playerFacId, string goalName, out bool isBookmark)
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
