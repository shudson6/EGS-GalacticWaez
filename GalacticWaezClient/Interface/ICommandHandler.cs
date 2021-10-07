using Eleon;

namespace GalacticWaez
{
    public interface ICommandHandler
    {
        /// <summary>
        /// Interpret a command, perform some action, and provide a response.
        /// Return value indicates whether the instance took action on the command, providing
        /// a signal to the caller that the command was valid.
        /// <br/>Implementations should smoothly handle <c>null</c> values for any parameter
        /// without throwing.
        /// </summary>
        /// <param name="cmdToken">the command</param>
        /// <param name="args">any parameters for the command</param>
        /// <param name="player">the player who issued the command</param>
        /// <param name="responder"><see cref="IResponder"/> instance for sending responses
        /// to that player</param>
        /// <returns>
        /// <c>true</c> if the instance acted the command, <c>false</c> otherwise
        /// </returns>
        bool HandleCommand(string cmdToken, string args, IPlayerInfo player, IResponder responder);
    }
}