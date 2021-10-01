namespace GalacticWaez.Navigation
{
    /// <summary>
    /// Provides ID and Position information about a player.
    /// TODO: create an implementation that can provide this information in a multiplayer setting
    /// </summary>
    public interface IPlayerInfo
    {
        Eleon.Modding.IPlayer Player { get; }

        Eleon.Modding.VectorInt3 GetCurrentStarCoordinates();

        float GetWarpRange();
    }
}