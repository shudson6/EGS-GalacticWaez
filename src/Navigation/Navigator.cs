using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Eleon.Modding;
using GalacticWaez;
using SectorCoordinates = Eleon.Modding.VectorInt3;

namespace GalacticWaez.Navigation
{
    public class Navigator
    {
        public delegate void DoneCallback(string message);

        private Task<string> navigation = null;
        private DoneCallback doneCallback;
        private PlayerData player;
        private readonly IModApi modApi;
        private readonly Galaxy galaxy;
        private readonly SaveGameDB db;

        public Navigator(IModApi modApi, Galaxy galaxy)
        {
            this.modApi = modApi;
            this.galaxy = galaxy;
            db = new SaveGameDB(modApi);
        }

        public void HandlePathRequest(string request, PlayerData player, DoneCallback doneCallback)
        {
            this.doneCallback = doneCallback;
            this.player = player;
            navigation = Task<string>.Factory.StartNew(Navigate, request);
            modApi.Application.Update += OnUpdateDuringNavigation;
        }
        
        private string Navigate(Object obj)
        {
            var startCoords = StartCoords();
            LYCoordinates goalCoords;
            bool goalIsBookmark;
            if (!GoalCoordinates((string)obj, out goalCoords, out goalIsBookmark))
            {
                return "No bookmark or known star by that name.";
            }
            if (goalCoords.Equals(startCoords))
            {
                return "It appears you are already there.";
            }
            var path = GetPath(startCoords, goalCoords);
            if (path == null)
            {
                return "No path found.";
            }
            if (path.Count() == 1)
            {
                return "Are you already there?";
            }
            if (path.Count() == 2 && goalIsBookmark)
            {
                return "It appears you are already in warp range.";
            }
            return SetWaypoints(path.Skip(1), goalIsBookmark);
        }

        private string SetWaypoints(IEnumerable<LYCoordinates> path, bool goalIsBookmark)
        {
            if (goalIsBookmark)
            {
                path = path.Take(path.Count() - 1);
            }
            var sectorsPath = path.Select(n => n.ToSectorCoordinates());
            int added = db.InsertBookmarks(sectorsPath, player);
            return $"Path found; {added}/{path.Count()} waypoints added.";
        }

        private IEnumerable<LYCoordinates> GetPath(LYCoordinates start, LYCoordinates goal) =>
            AstarPathfinder.FindPath(galaxy.GetNode(start), galaxy.GetNode(goal));

        private LYCoordinates StartCoords() => new LYCoordinates(
            modApi.ClientPlayfield.SolarSystemCoordinates);

        private bool GoalCoordinates(string goalName, out LYCoordinates goalCoords, out bool isBookmark)
        {
            isBookmark = false;
            SectorCoordinates sc;
            if (db.GetBookmarkVector(goalName, out sc))
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
