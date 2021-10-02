using System;
using System.Collections.Generic;
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

        public bool TryGetVector(int playerId, int playerFacId, string bookmarkName, out VectorInt3 coordinates)
        {
            throw new NotImplementedException();
        }
    }
}
