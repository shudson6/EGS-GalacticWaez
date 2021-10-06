using System;
using Eleon.Modding;
using GalacticWaez;

namespace GalacticWaezTests.Fakes
{
    public class FakeStarProvider : IKnownStarProvider
    {
        private readonly VectorInt3 starPos;
        public FakeStarProvider(VectorInt3 pos = default) => starPos = pos;
        public bool GetFirstKnownStarPosition(out VectorInt3 pos)
        {
            pos = starPos;
            return true;
        }

        public bool GetPosition(string name, out VectorInt3 pos)
        {
            pos = starPos;
            return true;
        }
    }

    public class NotFoundStarProvider : IKnownStarProvider
    {
        public bool GetFirstKnownStarPosition(out VectorInt3 pos)
        {
            pos = default;
            return false;
        }

        public bool GetPosition(string name, out VectorInt3 pos)
        {
            pos = default;
            return false;
        }
    }
}
