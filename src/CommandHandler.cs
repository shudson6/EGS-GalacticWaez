using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Eleon;
using Eleon.Modding;
using static GalacticWaez.CommandToken;

namespace GalacticWaez
{
    class CommandToken
    {
        public const string Introducer = "/waez";
        public const string Init = "init";
        public const string To = "to";
        public const string GetStatus = "status";
    }

    struct InitializationResult
    {
        // TODO: replace with galaxy class when written
        public readonly Galaxy galaxy;
        public readonly int elapsedMillis;
        public InitializationResult(Galaxy galaxy, int millis)
        {
            this.galaxy = galaxy;
            elapsedMillis = millis;
        }
    }

    class CommandHandler
    {
        public enum State
        {
            Uninitialized,
            Initializing,
            Ready,
            Busy
        }

        IModApi modApi;
        Task<InitializationResult> initializer = null;
        Task<string> navigator = null;
        Task messenger;

        private State status;

        public State Status { get => status; }

        public CommandHandler(IModApi modApi)
        {
            this.modApi = modApi;
            status = State.Uninitialized;
        }

        public void SendPlayerMessage(string message)
        {
            var md = new MessageData();
            md.GameTime += 5;
            md.SenderType = SenderType.System;
            md.Channel = MsgChannel.Global;
            md.SenderNameOverride = "Waez";
            md.RecipientEntityId = modApi.Application.LocalPlayer.Id;
            md.RecipientFaction = modApi.Application.LocalPlayer.Faction;
            md.Text = message;
            md.IsTextLocaKey = false;
            modApi.Application.SendChatMessage(md);
        }

        public void HandleChatCommand(MessageData messageData)
        {
            if (messageData.Text.StartsWith(Introducer))
            {
                string commandText = messageData.Text.Remove(0, Introducer.Length).Trim();
                if (commandText.Equals(Init))
                {
                    Initialize();
                    return;
                }
                if (commandText.Equals(GetStatus))
                {
                    HandleStatusRequest();
                    return;
                }
                string[] tokens = commandText.Split(separator: new[] { ' ' }, count: 2);
                if (tokens.Length == 2 && tokens[0].Equals(To))
                {
                    HandleNavRequest(tokens[1]);
                    return;
                }
            }
        }

        void HandleStatusRequest()
        {
            string message = status.ToString();
            modApi.GUI.ShowGameMessage($"Waez is {message}.");
            SendPlayerMessage(message);
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
                SendPlayerMessage("Cannot init because Waez is " + status.ToString());
            }
        }

        InitializationResult BuildGalaxyMap()
        {
            try
            {
                var db = new SaveGameDB(modApi);
                var playerData = db.GetPlayerData();
                var stopWatch = Stopwatch.StartNew();
                var starPositions = new StarFinder(db.GetFirstKnownStarPosition()).Search();
                var galaxy = Galaxy.CreateNew(starPositions, playerData.WarpRange);
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
                var result = initializer.Result;
                modApi.Log("Constructing galactic highway map "
                        + $"({result.galaxy.StarCount} stars and "
                        + $"{result.galaxy.WarpLines} warp lines) "
                        + $"took {(float)result.elapsedMillis / 1000,0:F3}s.");
                status = State.Ready;
                if (modApi.GUI.IsWorldVisible)
                {
                    modApi.GUI.ShowGameMessage("Waez ready.");
                }
            }
        }

        /***********************************************************************
         *
         * Pathfinding and Bookmarks
         *
         **********************************************************************/

        void HandleNavRequest(string bookmarkName)
        {
            status = State.Busy;
            navigator = Task<string>.Factory.StartNew(function: NavigateTo, state: bookmarkName);
            modApi.Application.Update += OnUpdateDuringNavigation;
        }

        string NavigateTo(Object obj)
        {
            string bookmarkName = (string)obj;

            return "Navigation function is not finished yet.";
        }

        void OnUpdateDuringNavigation()
        {
            if (navigator.IsCompleted)
            {
                modApi.Application.Update -= OnUpdateDuringNavigation;
                modApi.Log(navigator.Result);
                status = State.Ready;
                modApi.GUI.ShowGameMessage(navigator.Result);
            }
        }
    }
}
