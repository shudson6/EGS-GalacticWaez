using System;
using System.Collections.Generic;
using System.Timers;
using Eleon;
using Eleon.Modding;

namespace GalacticWaezTests
{
    public class FakeApplication : IApplication
    {
        private readonly string RootDir;
        Timer timer;

        public FakeApplication(string rootDir)
        {
            RootDir = rootDir;
            // simulate update cycle by firing every 20ms (50fps)
            timer = new Timer(20);
            timer.Elapsed += (src, e) => Update();
            timer.AutoReset = true;
            timer.Enabled = true;
        }

        public GameState State => throw new NotImplementedException();

        public ApplicationMode Mode => throw new NotImplementedException();

        public IPlayer LocalPlayer => throw new NotImplementedException();

        public ulong GameTicks => throw new NotImplementedException();

        public event PlayfieldDelegate OnPlayfieldLoaded;
        public event PlayfieldDelegate OnPlayfieldUnloading;
        public event UpdateDelegate Update;
        public event UpdateDelegate FixedUpdate;
        public event GamEnteredEventHandler GameEntered;
        public event ChatMessageSentEventHandler ChatMessageSent;

        public IPlayfieldDescr[] GetAllPlayfields()
        {
            throw new NotImplementedException();
        }

        public Dictionary<string, int> GetBlockAndItemMapping()
        {
            throw new NotImplementedException();
        }

        public string GetPathFor(AppFolder appFolder)
        {
            return RootDir;
        }

        public Dictionary<int, List<string>> GetPfServerInfos()
        {
            throw new NotImplementedException();
        }

        public PlayerData? GetPlayerDataFor(int playerEntityId)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<int> GetPlayerEntityIds()
        {
            throw new NotImplementedException();
        }

        public bool GetStructure(int entityId, Action<Eleon.Modding.GlobalStructureInfo> resultCallback)
        {
            throw new NotImplementedException();
        }

        public bool GetStructures(string playfieldName, FactionData? factionData, EntityType? entityType, Action<IEnumerable<Eleon.Modding.GlobalStructureInfo>> resultCallback)
        {
            throw new NotImplementedException();
        }

        public void SendChatMessage(MessageData chatMsgData)
        {
            throw new NotImplementedException();
        }

        public bool ShowDialogBox(int playerEntityId, DialogConfig config, DialogActionHandler actionHandler, int customValue)
        {
            throw new NotImplementedException();
        }
    }
}
