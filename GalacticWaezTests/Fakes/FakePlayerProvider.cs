using System;
using GalacticWaez;

namespace GalacticWaezTests.Fakes
{
    public class FakePlayerProvider : IPlayerProvider
    {
        private readonly IPlayerInfo player;
        public FakePlayerProvider(IPlayerInfo player) { this.player = player; }
        public IPlayerInfo GetPlayerInfo(int playerId) => player;
    }
}
