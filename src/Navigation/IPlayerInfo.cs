using Eleon.Modding;
using SectorCoordinates = Eleon.Modding.VectorInt3;

namespace GalacticWaez.Navigation
{
    /// <summary>
    /// Provides ID and Position information about a player.
    /// TODO: create an implementation that can provide this information in a multiplayer setting
    /// </summary>
    public interface IPlayerInfo
    {
        IPlayer Player { get; }

        SectorCoordinates GetCurrentStarCoordinates();

        float GetWarpRange();
    }
}