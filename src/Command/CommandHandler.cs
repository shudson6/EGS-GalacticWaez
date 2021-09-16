using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Eleon;
using Eleon.Modding;
using GalacticWaez.Navigation;

namespace GalacticWaez.Command
{
    public class CommandHandler
    {
        public enum State
        {
            Uninitialized,
            Initializing,
            Ready,
            Busy
        }

        private readonly IModApi modApi;
        private readonly SaveGameDB saveGameDB;
        Galaxy galaxy = null;

        private State status;

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
                modApi.Application.SendChatMessage(new ChatMessage("Invalid Command", 
                    modApi.Application.LocalPlayer));
            }
        }

        void HandleStatusRequest()
        {
            string message = status.ToString();
            modApi.Application.SendChatMessage(new ChatMessage(message, 
                modApi.Application.LocalPlayer));
        }

        const string HelpText = "Waez commands:\n"
            + "to [mapmarker]: plot a course to [mapmarker] and add mapmarkers for each step\n"
            + "status: find out what Waez is up to\n"
            + "init: initialize Waez. this should happen automatically\n"
            + "clear: remove all map markers that start with Waez_\n"
            + "help: get this help message\n";

        void HandleHelpRequest() => modApi.Application
            .SendChatMessage(new ChatMessage(HelpText, modApi.Application.LocalPlayer));

        void HandleClearRequest()
        {
            string message = $"Removed "
                + saveGameDB.ClearPathMarkers(modApi.Application.LocalPlayer.Id)
                + " map markers.";
            modApi.Application.SendChatMessage(new ChatMessage(message, 
                modApi.Application.LocalPlayer));
        }

        public void Initialize()
        {
            if (status == State.Uninitialized)
            {
                status = State.Initializing;
                new Initializer(modApi).Initialize((galaxy, response) =>
                {
                    this.galaxy = galaxy;
                    status = State.Ready;
                    modApi.Log(response);
                });
            }
            else
            {
                string message = "Cannot init because Waez is " + status.ToString();
                modApi.Application.SendChatMessage(new ChatMessage(message, 
                    modApi.Application.LocalPlayer));
            }
        }

        void HandleNavRequest(string bookmarkName)
        {
            if (status != State.Ready)
            {
                string message = "Unable: Waez is " + status.ToString();
                modApi.Application.SendChatMessage(new ChatMessage(message, 
                    modApi.Application.LocalPlayer));
                return;
            }
            status = State.Busy;
            new Navigator(modApi, galaxy)
                .HandlePathRequest(bookmarkName, modApi.Application.LocalPlayer,
                response =>
                {
                    status = State.Ready;
                    modApi.Application.SendChatMessage(
                        new ChatMessage(response, modApi.Application.LocalPlayer));
                });
        }
    }
}
