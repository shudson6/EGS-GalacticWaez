using System;
using Eleon.Modding;
using GalacticWaez;

namespace GalacticWaezTests.Fakes
{
    public class FakeStarProvider : IKnownStarProvider
    {
        public bool GetFirstKnownStarPosition(out VectorInt3 pos)
        {
            throw new NotImplementedException();
        }

        public bool GetPosition(string name, out VectorInt3 pos)
        {
            throw new NotImplementedException();
        }
    }
}
