using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Eleon.Modding;
using SectorCoordinates = Eleon.Modding.VectorInt3;

namespace GalacticWaez.Navigation
{
    // TODO: many of the logging calls (or all of them) should be chat messages
    public class Navigator : INavigator
    {
        private struct NavRequest
        {
            public readonly IPlayerTracker player;
            public readonly string dest;

            public NavRequest(IPlayerTracker player, string dest)
            {
                this.player = player;
                this.dest = dest;
            }
        }

        private Task<IEnumerable<LYCoordinates>> navigation = null;
        private NavigatorCallback doneCallback;
        private readonly IModApi modApi;
        private readonly ISaveGameDB db;
        private readonly Galaxy galaxy;

        public Navigator(IModApi modApi, Galaxy galaxy)
            : this(modApi, galaxy, new SaveGameDB(modApi)) { }

        public Navigator(IModApi modApi, Galaxy galaxy, ISaveGameDB db)
        {
            this.modApi = modApi;
            this.galaxy = galaxy;
            this.db = db;
        }

        public void HandlePathRequest(string dest, IPlayerTracker playerTracker, 
            NavigatorCallback doneCallback)
        {
            this.doneCallback = doneCallback;
            navigation = Task<IEnumerable<LYCoordinates>>.Factory.StartNew(Navigate,
                new NavRequest(playerTracker, dest));
            modApi.Application.Update += OnUpdateDuringNavigation;
        }

        private IEnumerable<LYCoordinates> Navigate(Object obj)
        {
            var request = (NavRequest)obj;
            var startCoords = request.player.GetCurrentStarCoordinates();
            if (!GoalCoordinates(request.dest, 
                out LYCoordinates goalCoords, 
                out bool goalIsBookmark))
            {
                modApi.LogWarning($"No bookmark or known star by name {request.dest}");
            }
            if (goalCoords.Equals(startCoords))
            {
                modApi.Log("It appears you are already there.");
            }
            float range = request.player.GetWarpRange();
            var path = GetPath(startCoords, goalCoords, range);
            if (path == null)
            {
                modApi.Log("No path found.");
            }
            if (path.Count() == 1)
            {
                modApi.Log("Are you already there?");
            }
            if (path.Count() == 2 && goalIsBookmark)
            {
                modApi.Log("It appears you are already in warp range.");
            }
            return SetWaypoints(path.Skip(1), request.player.GetPlayerId(), goalIsBookmark);
        }

        private IEnumerable<LYCoordinates>
            SetWaypoints(IEnumerable<LYCoordinates> path, int playerId, bool goalIsBookmark)
        {
            if (goalIsBookmark)
            {
                path = path.Take(path.Count() - 1);
            }
            var sectorsPath = path.Select(n => n.ToSectorCoordinates());
            int added = db.InsertBookmarks(sectorsPath, playerId);
            modApi.Log($"Path found; {added}/{path.Count()} waypoints added.");
            return path;
        }

        private IEnumerable<LYCoordinates> GetPath(LYCoordinates start, LYCoordinates goal, float range)
            => AstarPathfinder.FindPath(galaxy.GetNode(start), galaxy.GetNode(goal), range);

        private bool GoalCoordinates(string goalName, out LYCoordinates goalCoords, out bool isBookmark)
        {
            isBookmark = false;
            if (db.GetBookmarkVector(goalName, out SectorCoordinates sc))
            {
                goalCoords = new LYCoordinates(sc);
                isBookmark = true;
                return true;
            }
            if (db.GetSolarSystemCoordinates(goalName, out sc))
            {
                goalCoords = new LYCoordinates(sc);
                return true;
            }
            goalCoords = default;
            return false;
        }

        private void OnUpdateDuringNavigation()
        {
            if (!navigation.IsCompleted) return;

            modApi.Application.Update -= OnUpdateDuringNavigation;
            doneCallback(navigation.Result);
        }
    }
}
