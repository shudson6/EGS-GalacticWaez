using System.Collections.Generic;
using System.Threading.Tasks;
using Eleon.Modding;

namespace GalacticWaez
{
    public interface INavigator
    { 
        GalaxyMap Galaxy { get; }

        /// <summary>
        /// Looks for path from player's current solar system to the destination system. If found,
        /// adds waypoints to the savegame. Does not add a waypoint for the current system.
        /// <br/>
        /// <c>destination</c> may be either a bookmark (on the galaxy map) or a visited star. If
        /// it is a bookmark, no new marker is added for the destination. If not, a marker is placed
        /// for the final step.
        /// </summary>
        /// <param name="player">not null</param>
        /// <param name="destination">not null</param>
        /// <param name="playerRange">how far the player can jump, in LY. must be positive</param>
        /// <param name="response">object used for communicating result</param>
        Task<IEnumerable<VectorInt3>> Navigate(
            IPlayerInfo player, string destination, float playerRange, IResponder response);
    }
}