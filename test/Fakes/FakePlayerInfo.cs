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
}
