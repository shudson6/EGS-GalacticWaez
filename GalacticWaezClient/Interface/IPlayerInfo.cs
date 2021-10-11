namespace GalacticWaez
{
    /// <summary>
    /// Provides ID and Position information about a player.
    /// </summary>
    public interface IPlayerInfo
    {
        string Name { get; }
        int Id { get; }
        int FactionId { get; }

        /// <summary>
        /// The range this player is able to jump. Given in sectors, _not_ LY.
        /// </summary>
        float WarpRange { get; }

        Eleon.Modding.VectorInt3 StarCoordinates { get; }

        string PlayfieldName { get; }

        string StarName { get; }
    }
}