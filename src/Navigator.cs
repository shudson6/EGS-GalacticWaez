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
        private struct NavResult
        {
            public IEnumerable<LYCoordinates> path;
            public string message;
        }

        private Task<NavResult> navigation = null;
        private NavigatorCallback doneCallback;
        private readonly IModApi modApi;
        private readonly ISaveGameDB db;
        private readonly GalaxyMap galaxy;

        private string destination;
        private IPlayerInfo playerInfo;
        private Pathfinder findPath;

        public Navigator(IModApi modApi, GalaxyMap galaxy)
            : this(modApi, galaxy, new SaveGameDB(modApi)) { }

        public Navigator(IModApi modApi, GalaxyMap galaxy, ISaveGameDB db)
        {
            this.modApi = modApi;
            this.galaxy = galaxy;
            this.db = db;
        }

        public void HandlePathRequest(string dest, IPlayerInfo playerTracker, 
            Pathfinder findPath, NavigatorCallback doneCallback)
        {
            destination = dest;
            this.playerInfo = playerTracker;
            this.findPath = findPath;
            this.doneCallback = doneCallback;
            navigation = Task<NavResult>.Factory.StartNew(Navigate);
            modApi.Application.Update += OnUpdateDuringNavigation;
        }

        private NavResult Navigate()
        {
            string text;
            var startCoords = new LYCoordinates(playerInfo.GetCurrentStarCoordinates());
            if (!GoalCoordinates(destination,
                out LYCoordinates goalCoords, 
                out bool goalIsBookmark))
            {
                text = $"No bookmark or known star by name {destination}";
                modApi.LogWarning(text);
                return new NavResult { path = null, message = text };
            }
            if (goalCoords.Equals(startCoords))
            {
                text = "It appears you are already there.";
                modApi.Log(text);
                modApi.Application.SendChatMessage(new ChatMessage(text, modApi.Application.LocalPlayer));
                return new NavResult { path = null, message = text };
            }
            float range = playerInfo.GetWarpRange();
            var path = GetPath(startCoords, goalCoords, range);
            if (path == null)
            {
                text = "No path found.";
                modApi.Log(text);
                modApi.Application.SendChatMessage(new ChatMessage(text, modApi.Application.LocalPlayer));
                return new NavResult { path = null, message = text };
            }
            if (path.Count() == 1)
            {
                text = "Are you already there?";
                modApi.Log(text);
                modApi.Application.SendChatMessage(new ChatMessage(text, modApi.Application.LocalPlayer));
                return new NavResult { path = null, message = text };
            }
            if (path.Count() == 2 && goalIsBookmark)
            {
                text = "It appears you are already in warp range.";
                modApi.Log(text);
                modApi.Application.SendChatMessage(new ChatMessage(text, modApi.Application.LocalPlayer));
                return new NavResult { path = null, message = text };
            }
            return SetWaypoints(path.Skip(1), playerInfo.Player.Id, goalIsBookmark);
        }

        private NavResult
            SetWaypoints(IEnumerable<LYCoordinates> path, int playerId, bool goalIsBookmark)
        {
            if (goalIsBookmark)
            {
                path = path.Take(path.Count() - 1);
            }
            var sectorsPath = path.Select(n => n.ToSectorCoordinates());
            int added = db.InsertBookmarks(sectorsPath, playerId);
            string text = $"Path found; {added}/{path.Count()} waypoints added.";
            modApi.Log(text);
            return new NavResult { path = path, message = text };
        }

        private IEnumerable<LYCoordinates> GetPath(LYCoordinates start, LYCoordinates goal, float range)
            => findPath(galaxy.GetNode(start), galaxy.GetNode(goal), range);

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
            doneCallback(navigation.Result.path, navigation.Result.message);
        }
    }
}
