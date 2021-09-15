using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Eleon;
using Eleon.Modding;
using GalacticWaez.Navigation;

namespace GalacticWaez.Command
{
    struct InitializationResult
    {
        public readonly Galaxy galaxy;
        public readonly int elapsedMillis;
        public InitializationResult(Galaxy galaxy, int millis)
        {
            this.galaxy = galaxy;
            elapsedMillis = millis;
        }
    }

    public class CommandHandler
    {
        public delegate void DoneCallback(string message);

        public enum State
        {
            Uninitialized,
            Initializing,
            Ready,
            Busy
        }

        private readonly IModApi modApi;
        private readonly SaveGameDB saveGameDB;
        Task<InitializationResult> initializer = null;
        Galaxy galaxy = null;

        private State status;
        private PlayerData localPlayerData;

        public State Status { get => status; }

        public CommandHandler(IModApi modApi)
        {
            this.modApi = modApi;
            status = State.Uninitialized;
            saveGameDB = new SaveGameDB(modApi);
        }

        public void HandleChatCommand(MessageData messageData)
        {
            if (messageData.Text.StartsWith(CommandToken.Introducer))
            {
                string commandText = messageData.Text.Remove(0, CommandToken.Introducer.Length).Trim();
                if (commandText.Equals(CommandToken.Init))
                {
                    Initialize();
                    return;
                }
                if (commandText.Equals(CommandToken.GetStatus))
                {
                    HandleStatusRequest();
                    return;
                }
                if (commandText.Equals(CommandToken.Help))
                {
                    HandleHelpRequest();
                    return;
                }
                if (commandText.Equals(CommandToken.Clear))
                {
                    HandleClearRequest();
                    return;
                }
                string[] tokens = commandText.Split(separator: new[] { ' ' }, count: 2);
                if (tokens.Length == 2 && tokens[0].Equals(CommandToken.To))
                {
                    HandleNavRequest(tokens[1]);
                    return;
                }
                modApi.Application.SendChatMessage(new ChatMessage("Invalid Command", localPlayerData.Entity));
            }
        }

        void HandleStatusRequest()
        {
            string message = status.ToString();
            modApi.Application.SendChatMessage(new ChatMessage(message, localPlayerData.Entity));
        }

        const string HelpText = "Waez commands:\n"
            + "to [mapmarker]: plot a course to [mapmarker] and add mapmarkers for each step\n"
            + "status: find out what Waez is up to\n"
            + "init: initialize Waez. this should happen automatically\n"
            + "clear: remove all map markers that start with Waez_\n"
            + "help: get this help message\n";

        void HandleHelpRequest()
        {
            modApi.Application.SendChatMessage(new ChatMessage(HelpText, localPlayerData.Entity));
        }

        void HandleClearRequest()
        {
            string message = $"Removed "
                + saveGameDB.ClearPathMarkers(localPlayerData.Entity.Id)
                + " map markers.";
            modApi.Application.SendChatMessage(new ChatMessage(message, localPlayerData.Entity));
        }

        /***********************************************************************
         *
         * Initialization/Map-Building stuff
         *
         **********************************************************************/

        public void Initialize()
        {
            if (status == State.Uninitialized)
            {
                status = State.Initializing;
                initializer = Task<InitializationResult>.Factory.StartNew(BuildGalaxyMap);
                modApi.Application.Update += OnUpdateDuringInit;
            }
            else
            {
                string message = "Cannot init because Waez is " + status.ToString();
                modApi.Application.SendChatMessage(new ChatMessage(message, localPlayerData.Entity));
            }
        }

        InitializationResult BuildGalaxyMap()
        {
            try
            {
                localPlayerData = saveGameDB.GetPlayerData();
                var stopWatch = Stopwatch.StartNew();
                var starPositions = new StarFinder(saveGameDB.GetFirstKnownStarPosition()).Search();
                var galaxy = Galaxy.CreateNew(starPositions, localPlayerData.WarpRange);
                stopWatch.Stop();
                
                // surely this won't take so long we actually lose data with this downcast :P
                return new InitializationResult(galaxy, (int)stopWatch.ElapsedMilliseconds);
            }
            catch (Exception ex)
            {
                modApi.LogError(ex.Message);
                status = State.Uninitialized;
            }

            return default;
        }

        void OnUpdateDuringInit()
        {
            if (initializer.IsCompleted)
            {
                modApi.Application.Update -= OnUpdateDuringInit;
                galaxy = initializer.Result.galaxy;
                modApi.Log("Constructing galactic highway map "
                        + $"({initializer.Result.galaxy.StarCount} stars and "
                        + $"{initializer.Result.galaxy.WarpLines} warp lines) "
                        + $"took {(float)initializer.Result.elapsedMillis / 1000,0:F3}s.");
                status = State.Ready;
                if (modApi.GUI.IsWorldVisible)
                {
                    modApi.GUI.ShowGameMessage("Waez ready.");
                }
            }
        }

        void HandleNavRequest(string bookmarkName)
        {
            if (status != State.Ready)
            {
                string message = "Unable: Waez is " + status.ToString();
                modApi.Application.SendChatMessage(new ChatMessage(message, localPlayerData.Entity));
                return;
            }
            status = State.Busy;
            new Navigator(modApi, galaxy)
                .HandlePathRequest(bookmarkName, localPlayerData,
                response =>
                {
                    status = State.Ready;
                    modApi.Application.SendChatMessage(
                        new ChatMessage(response, localPlayerData.Entity));
                });
        }
    }
}
