using System;
using System.Collections.Generic;
using Eleon.Modding;

namespace GalacticWaez
{
    public struct BookmarkData
    {
        public int PlayerId;
        public int PlayerFacId;
        public int FacGroup;
        public int Icon;
        public bool IsShared;
        public bool IsWaypoint;
        public bool IsRemove;
        public bool IsShowHud;
        public ulong GameTime;
        public int MaxDistance;
    }

    public interface IBookmarkManager
    {
        /// <summary>
        /// Retrieves coordinates for a requested bookmark, if it exists and belongs to
        /// the specified player or faction. In case it belongs to the faction but not the
        /// player, its isshared attribute must be set.
        /// <br/>
        /// If the bookmark is retrieved, <c>coordinates</c> is set accordingly and return
        /// is <c>true</c>. If not, <c>coordinates</c> is <c>default(VectorInt3)</c> and
        /// return is <c>false.</c>
        /// </summary>
        /// <param name="playerId">of the player owning the marker</param>
        /// <param name="playerFacId">of the faction owning the marker</param>
        /// <param name="bookmarkName">name of the marker to find</param>
        /// <param name="coordinates"></param>
        /// <returns>
        /// <c>true</c> on successful retrieval
        /// <br/>
        /// <c>false</c> otherwise
        /// </returns>
        /// <remarks>
        /// Implementations should avoid throwing and smoothly handle null values.
        /// <br/>
        /// Because the default VectorInt3 (0, 0, 0) may well be a valid star position, please
        /// pay attention to the return value.
        /// </remarks>
        bool TryGetVector(int playerId, int playerFacId, string bookmarkName, out VectorInt3 coordinates);

        /// <summary>
        /// Inserts a bookmark for each <c>VectorInt3</c> in <c>coordinates</c>.
        /// </summary>
        /// <param name="coordinates"></param>
        /// <param name="data"></param>
        /// <returns>
        /// the number of bookmarks inserted
        /// </returns>
        /// <remarks>
        /// Implementations should avoid throwing and smoothly handle null values.
        /// </remarks>
        int InsertBookmarks(IEnumerable<VectorInt3> coordinates, BookmarkData data);

        /// <summary>
        /// Modifies existing 'Waez_*' bookmarks. Action may be "show", "hide", or "clear".
        /// <br/><c>show</c> makes them visible on the HUD.
        /// <br/><c>hide</c> makes them not visible on the HUD.
        /// <br/><c>clear</c> erases them.
        /// </summary>
        /// <param name="playerId"></param>
        /// <param name="action"></param>
        /// <returns>
        /// The number of bookmarks changed.
        /// </returns>
        /// <remarks>
        /// Implementations should avoid throwing and smoothly handle null values.
        /// </remarks>
        int ModifyPathMarkers(int playerId, string action);
    }
}
