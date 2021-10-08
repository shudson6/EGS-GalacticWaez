using System;
using Eleon.Modding;
using GalacticWaez;

namespace GalacticWaezTests.Fakes
{
    public class FakePlayerInfo : IPlayerInfo
    {
        public int Id => throw new NotImplementedException();

        public int FactionId => throw new NotImplementedException();

        public float WarpRange => throw new NotImplementedException();

        public VectorInt3 StarCoordinates => throw new NotImplementedException();

        public string Name => throw new NotImplementedException();

        public string PlayfieldName => throw new NotImplementedException();
    }

    public class NavTestPlayerInfo : IPlayerInfo
    {
        private readonly VectorInt3 starVector;
        public int Id { get; }

        public int FactionId { get; }

        public float WarpRange { get; }

        public string Name => $"Player_{Id}";

        public VectorInt3 StarCoordinates => starVector;

        public string PlayfieldName => throw new NotImplementedException();

        public NavTestPlayerInfo(int id, int facid, VectorInt3 forStar, float range)
        {
            WarpRange = range;
            starVector = forStar;
            Id = id;
            FactionId = facid;
        }
        public VectorInt3 GetStarCoordinates() => starVector;
    }
}
