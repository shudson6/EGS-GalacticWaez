﻿using Eleon;
using Eleon.Modding;
using GalacticWaez.Navigation;

namespace GalacticWaez.Command
{
    public class CommandHandler 
    {
        private readonly IModApi modApi;
        private readonly SaveGameDB saveGameDB;
        private GalaxyMap galaxy = null;

        public CommandHandler(IModApi modApi, GalaxyMap galaxy)
        {
            this.modApi = modApi;
            this.galaxy = galaxy;
            saveGameDB = new SaveGameDB(modApi.Application.GetPathFor(AppFolder.SaveGame), modApi.Log);
        }

        public void HandleChatCommand(MessageData messageData)
        {
            if (messageData.Text.StartsWith(CommandToken.Introducer))
            {
                string commandText = messageData.Text.Remove(0, CommandToken.Introducer.Length).Trim();
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
                if (tokens.Length == 2 && tokens[0].Equals(CommandToken.Bookmarks))
                {
                    HandleBookmarkRequest(tokens[1]);
                    return;
                }
                modApi.Application.SendChatMessage(new ChatMessage("Invalid Command", 
                    modApi.Application.LocalPlayer));
            }
        }

        private const string HelpText = "Waez commands:\n"
            + "to [mapmarker]: plot a course to [mapmarker] and add mapmarkers for each step\n"
            + "status: find out what Waez is up to\n"
            + "init: initialize Waez. this should happen automatically\n"
            + "clear: remove all map markers that start with Waez_\n"
            + "bookmarks [clear|hide|show]: remove Waez_ bookmarks or hide/show them in HUD (requires exit/resume)\n"
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

        private void HandleBookmarkRequest(string operation)
        {
            string message = $"Modified "
                + saveGameDB.ModifyPathMarkers(modApi.Application.LocalPlayer.Id, operation)
                + " map markers.";
            modApi.Application.SendChatMessage(new ChatMessage(message,
                modApi.Application.LocalPlayer));
            return;
        }

    }
}
