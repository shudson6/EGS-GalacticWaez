using Eleon.Modding;
using System.Collections.Generic;

namespace GalacticWaez
{
    public interface ISaveGameDB
    {
        int ClearPathMarkers(int playerId);
        bool GetBookmarkVector(int playerId, string bookmarkName, out VectorInt3 coordinates);
        int InsertBookmarks(IEnumerable<VectorInt3> positions, int playerId, ulong gameTime);
    }
}