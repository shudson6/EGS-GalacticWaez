namespace GalacticWaez.Navigation
{
    public interface IPlayerTracker
    {
        int GetPlayerId();

        LYCoordinates GetCurrentStarCoordinates();

        float GetWarpRange();
    }
}