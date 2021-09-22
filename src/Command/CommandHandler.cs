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
            Busy,
            InitFailed
        }

        private readonly IModApi modApi;
        private readonly SaveGameDB saveGameDB;
        private Galaxy galaxy = null;

        public State Status { get; private set; }

        public CommandHandler(IModApi modApi)
        {
            this.modApi = modApi;
            Status = State.Uninitialized;
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

        private void HandleStatusRequest()
        {
            string message = Status.ToString();
            modApi.Application.SendChatMessage(new ChatMessage(message, 
                modApi.Application.LocalPlayer));
        }

        private const string HelpText = "Waez commands:\n"
            + "to [mapmarker]: plot a course to [mapmarker] and add mapmarkers for each step\n"
            + "status: find out what Waez is up to\n"
            + "init: initialize Waez. this should happen automatically\n"
            + "clear: remove all map markers that start with Waez_\n"
            + "help: get this help message\n";

        private void HandleHelpRequest() => modApi.Application
            .SendChatMessage(new ChatMessage(HelpText, modApi.Application.LocalPlayer));

        private void HandleClearRequest()
        {
            string message = $"Removed "
                + saveGameDB.ClearPathMarkers(modApi.Application.LocalPlayer.Id)
                + " map markers.";
            modApi.Application.SendChatMessage(new ChatMessage(message, 
                modApi.Application.LocalPlayer));
        }

        public void Initialize()
        {
            if (Status != State.Uninitialized)
            {
                string message = "Cannot init because Waez is " + Status.ToString();
                modApi.Application.SendChatMessage(new ChatMessage(message, 
                    modApi.Application.LocalPlayer));
                return;
            }
            DoInit();
        }

        private void HandleRestartRequest()
        {
            if (Status != State.Ready)
            {
                string message = "Cannot restart because Waez is " + Status.ToString();
                modApi.Application.SendChatMessage(new ChatMessage(message, 
                    modApi.Application.LocalPlayer));
                return;
            }
            DoInit();
        }

        private void DoInit()
        {
            Status = State.Initializing;
            new Initializer(modApi).Initialize(Initializer.Source.Normal, 
                (galaxy, exception) =>
            {
                if (galaxy != null)
                {
                    this.galaxy = galaxy;
                    Status = State.Ready;
                    modApi.GUI.ShowGameMessage("Waez is ready.");
                    return;
                }
                if (exception != null)
                {
                    foreach (var ex in exception.InnerExceptions)
                    {
                        modApi.LogError(ex.Message);
                    }
                }
                Status = State.InitFailed;
                modApi.Log("Initialization failed.");
            });
        }

        private void HandleNavRequest(string bookmarkName)
        {
            if (Status != State.Ready)
            {
                string message = "Unable: Waez is " + Status.ToString();
                modApi.Application.SendChatMessage(new ChatMessage(message, 
                    modApi.Application.LocalPlayer));
                return;
            }
            Status = State.Busy;
            new Navigator(modApi, galaxy)
                .HandlePathRequest(bookmarkName, modApi.Application.LocalPlayer,
                response =>
                {
                    Status = State.Ready;
                    modApi.Application.SendChatMessage(
                        new ChatMessage(response, modApi.Application.LocalPlayer));
                });
        }
    }
}
