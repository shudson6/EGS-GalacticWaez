using System.Collections.Generic;
using Eleon.Modding;
using GalacticWaez;

namespace GalacticWaezTests.Fakes
{
    class FakeStorage : IGalaxyStorage
    {
        public bool Exists => throw new System.NotImplementedException();

        public bool StoreGalaxyData(IEnumerable<VectorInt3> positions)
        {
            throw new System.NotImplementedException();
        }
    }
}
