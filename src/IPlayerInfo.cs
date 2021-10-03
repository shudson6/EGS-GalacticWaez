namespace GalacticWaez
{
    /// <summary>
    /// Provides ID and Position information about a player.
    /// TODO: create an implementation that can provide this information in a multiplayer setting
    /// </summary>
    public interface IPlayerInfo
    {
        int Id { get; }
        int FactionId { get; }
        float WarpRange { get; }

        Eleon.Modding.VectorInt3 GetCurrentStarCoordinates();
    }
}