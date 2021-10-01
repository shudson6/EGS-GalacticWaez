namespace GalacticWaez
{
    public struct BookmarkOptions
    {
        public bool IsShared;
        public bool IsWaypoint;
        public bool IsRemove;
        public bool IsShowHud;
        public bool IsCallback;
        public int MaxDistance;
    }
    public interface IBookmarkManager
    {
        bool GetCoordinates(int playerId, int playerFacId, string bookmarkName,
            out Eleon.Modding.VectorInt3 coordinates);
    }
}
