using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Mono.Data.Sqlite;
using Eleon;
using Eleon.Modding;

namespace GalacticWaez
{
    public class GalacticWaezClient : IMod
    {
        IModApi modApi;
        string saveGameDir = null;

        public void Init(IModApi modApi)
        {
            this.modApi = modApi;
            modApi.GameEvent += OnGameEvent;
            modApi.Log("GalacticWaezClient attached.");
        }

        public void Shutdown()
        {
            modApi.GameEvent -= OnGameEvent;
            modApi.Application.ChatMessageSent -= OnChatMessageSent;
            modApi.Log("GalacticWaezClient detached.");
        }

        void OnGameEvent(GameEventType type,
                        object arg1 = null,
                        object arg2 = null,
                        object arg3 = null,
                        object arg4 = null,
                        object arg5 = null
        ) {
            switch (type)
            {
                case GameEventType.GameStarted:
                    if (modApi.Application.Mode == ApplicationMode.SinglePlayer)
                    {
                        saveGameDir = modApi.Application.GetPathFor(AppFolder.SaveGame);
                        modApi.Application.ChatMessageSent += OnChatMessageSent;
                        modApi.Log("Listening for commands.");
                    }
                    break;

                case GameEventType.GameEnded:
                    saveGameDir = null;
                    modApi.Application.ChatMessageSent -= OnChatMessageSent;
                    modApi.Log("Stopped listening for commands.");
                    break;
            }
        }

        void OnChatMessageSent(MessageData messageData)
        {
            if (messageData.Text.StartsWith(CommandToken.Introducer))
            {
                string command = messageData.Text.Substring(CommandToken.Introducer.Length).Trim();
                if (command.Equals(CommandToken.Init))
                {
                    modApi.Log("Initializing galactic highway map...");
                    // TODO: create Initializer class to handle this
                    // and other necessary business for building the map
                    var finder = new StarFinder(new SaveGameDB(modApi).GetFirstKnownStarPosition());
                    var stars = finder.Search();
                    modApi.Log($"Found {stars.Count()} stars.");
                    string starDataDir = $"{saveGameDir}\\Content\\GalacticWaez";
                    Directory.CreateDirectory(starDataDir);
                    StreamWriter writer = new StreamWriter(
                        new FileStream($"{starDataDir}\\stardata.csv",
                        FileMode.Create, FileAccess.Write));
                    foreach (var starPos in stars)
                    {
                        writer.WriteLine($"{starPos.sectorX},{starPos.sectorY},{starPos.sectorZ}");
                    }
                    writer.Close();
                }
            }
        }
    }
}
