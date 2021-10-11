using Eleon.Modding;

namespace GalacticWaez
{
    public interface IKnownStarProvider
    {
        /// <summary>
        /// Retrieves the position of the first star in the savegame
        /// (often, but not necessarily, the starting system).
        /// <br/>
        /// If this method returns <c>false</c>, the value in <c>pos</c>
        /// is not valid.
        /// </summary>
        /// <param name="pos"><c>VectorInt3</c> to hold the result</param>
        /// <returns>
        /// <c>true</c> on success, <c>false</c> on failure.
        /// </returns>
        bool GetFirstKnownStarPosition(out VectorInt3 pos);

        /// <summary>
        /// Retrieves the position of the specified star, if it exists in
        /// the savegame.         
        /// <br/>
        /// If this method returns <c>false</c>, the value in <c>pos</c>
        /// is not valid.
        /// </summary>
        /// <param name="name">name of the star being sought</param>
        /// <param name="pos"><c>VectorInt3</c> to hold the result</param>
        /// <returns>
        /// <c>true</c> on success, <c>false</c> on failure.
        /// </returns>
        /// <remarks>
        /// Note that stars do not appear in the savegame
        /// until they have been discovered in the game, so the success or
        /// failure of this method does not indicate the existence or
        /// otherwise of the star.
        /// </remarks>
        bool GetPosition(string name, out VectorInt3 pos);
    }
}
