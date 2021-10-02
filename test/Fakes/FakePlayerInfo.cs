using System;
using Eleon.Modding;

namespace GalacticWaezTests.Fakes
{
    public class FakePlayerInfo : GalacticWaez.IPlayerInfo
    {
        public IPlayer Player => throw new NotImplementedException();

        public VectorInt3 GetCurrentStarCoordinates()
        {
            throw new NotImplementedException();
        }

        public float GetWarpRange()
        {
            throw new NotImplementedException();
        }
    }

    public class NavTestPlayerInfo : GalacticWaez.IPlayerInfo
    {
        private readonly VectorInt3 starVector;
        private readonly float range;
        public IPlayer Player { get; private set; }
        public NavTestPlayerInfo(IPlayer player, VectorInt3 forStar, float range)
        {
            this.range = range;
            starVector = forStar;
            Player = player;
        }
        public VectorInt3 GetCurrentStarCoordinates() => starVector;
        public float GetWarpRange() => range;
    }
}
