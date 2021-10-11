using System;
using System.Collections.Generic;
using System.Linq;
using Eleon.Modding;
using GalacticWaez;

namespace GalacticWaezTests.Fakes
{
    class FakeBookmarkManager : IBookmarkManager
    {
        public int InsertBookmarks(IEnumerable<VectorInt3> coordinates, BookmarkData data)
        {
            throw new NotImplementedException();
        }

        public int ModifyPathMarkers(int playerId, string action)
        {
            throw new NotImplementedException();
        }

        public bool TryGetVector(int playerId, int playerFacId, string bookmarkName, out VectorInt3 coordinates)
        {
            throw new NotImplementedException();
        }
    }

    public class HappyBookmarkManager : IBookmarkManager
    {
        private readonly VectorInt3 bookmarkVector;
        public int Inserted { get; private set; } = 0;

        public HappyBookmarkManager(VectorInt3 forBookmark) => bookmarkVector = forBookmark;

        public int InsertBookmarks(IEnumerable<VectorInt3> coordinates, BookmarkData data)
        {
            Inserted = coordinates.Count();
            return Inserted;
        }

        public bool TryGetVector(int playerId, int playerFacId, string bookmarkName, out VectorInt3 coordinates)
        {
            coordinates = bookmarkVector;
            return true;
        }

        public int ModifyPathMarkers(int playerId, string action) => 3;
    }

    public class NotFoundBookmarkManager : IBookmarkManager
    {
        public int Inserted { get; private set; }
        public int InsertBookmarks(IEnumerable<VectorInt3> coordinates, BookmarkData data)
        {
            Inserted = coordinates.Count();
            return Inserted;
        }

        public int ModifyPathMarkers(int playerId, string action) => 0;

        public bool TryGetVector(int playerId, int playerFacId, string bookmarkName, out VectorInt3 coordinates)
        {
            coordinates = default;
            return false;
        }
    }
}
