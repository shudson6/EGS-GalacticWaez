using Eleon.Modding;
using System.Collections.Generic;

namespace GalacticWaez
{
    public interface ISaveGameDB
    {
        int ClearPathMarkers(int playerId);
        bool GetBookmarkVector(string bookmarkName, out VectorInt3 coordinates);
        VectorInt3 GetFirstKnownStarPosition();
        float GetLocalPlayerWarpRange();
        float GetPlayerWarpRange(int playerId);
        bool GetSolarSystemCoordinates(string starName, out VectorInt3 coordinates);
        int InsertBookmarks(IEnumerable<VectorInt3> positions, IPlayer player);
    }
}