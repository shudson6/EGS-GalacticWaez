using System;
using Eleon.Modding;

namespace GalacticWaezTests.Fakes
{
    public class FakePlayerInfo : GalacticWaez.IPlayerInfo
    {
        public int Id => throw new NotImplementedException();

        public int FactionId => throw new NotImplementedException();

        public float WarpRange => throw new NotImplementedException();

        public VectorInt3 GetCurrentStarCoordinates()
        {
            throw new NotImplementedException();
        }
    }

    public class NavTestPlayerInfo : GalacticWaez.IPlayerInfo
    {
        private readonly VectorInt3 starVector;
        public int Id { get; }

        public int FactionId { get; }

        public float WarpRange { get; }

        public NavTestPlayerInfo(int id, int facid, VectorInt3 forStar, float range)
        {
            WarpRange = range;
            starVector = forStar;
            Id = id;
            FactionId = facid;
        }
        public VectorInt3 GetCurrentStarCoordinates() => starVector;
    }
}
