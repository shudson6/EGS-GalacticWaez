using System;
using GalacticWaez;

namespace GalacticWaezTests.Fakes
{
    public class FakePlayerProvider : IPlayerProvider
    {
        public delegate IPlayerInfo GetPlayerDelegate(int playerId);

        private readonly GetPlayerDelegate DoStuff;

        public FakePlayerProvider(GetPlayerDelegate doStuff) { DoStuff = doStuff; }
        public IPlayerInfo GetPlayerInfo(int playerId) => DoStuff(playerId);
    }
}
