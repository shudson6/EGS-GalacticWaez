using System;
using System.Collections.Generic;
using Eleon;
using Eleon.Modding;

namespace GalacticWaezTests
{
    public class FakeApplication : IApplication
    {
        private readonly string RootDir;

        public FakeApplication(string rootDir)
        {
            RootDir = rootDir;
        }

        public GameState State => throw new NotImplementedException();

        public ApplicationMode Mode => throw new NotImplementedException();

        public IPlayer LocalPlayer => new FakePlayer(1337);

        public ulong GameTicks => throw new NotImplementedException();

        public event PlayfieldDelegate OnPlayfieldLoaded { add { } remove { } }
        public event PlayfieldDelegate OnPlayfieldUnloading { add { } remove { } }
        public event UpdateDelegate Update;
        public event UpdateDelegate FixedUpdate { add { } remove { } }
        public event GamEnteredEventHandler GameEntered { add { } remove { } }
        public event ChatMessageSentEventHandler ChatMessageSent { add { } remove { } }

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
        { /* TODO: keep these like FakeModApi does logs */ }

        public bool ShowDialogBox(int playerEntityId, DialogConfig config, DialogActionHandler actionHandler, int customValue)
        {
            throw new NotImplementedException();
        }

        public void FireUpdate()
        {
            Update();
        }
    }
}
