﻿namespace GalacticWaez
{
    /// <summary>
    /// Provides ID and Position information about a player.
    /// TODO: create an implementation that can provide this information in a multiplayer setting
    /// </summary>
    public interface IPlayerInfo
    {
        int Id { get; }
        int FactionId { get; }

        /// <summary>
        /// The range this player is able to jump. Given in sectors, _not_ LY.
        /// </summary>
        float WarpRange { get; }

        Eleon.Modding.VectorInt3 GetCurrentStarCoordinates();
    }
}