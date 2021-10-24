using Eleon.Modding;
using GalacticWaez;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GalacticWaezTests.Fakes
{
    class FakeNavigator : INavigator
    {
        public IGalaxyMap Galaxy => null;
        public IPlayerInfo Player { get; private set; }
        public string Goal { get; private set; }
        public float Range { get; private set; }
        public Task<IEnumerable<VectorInt3>> Navigate(
            IPlayerInfo player, string destination, float playerRange, IResponder response)
        {
            Player = player;
            Goal = destination;
            Range = playerRange;
            return null;
        }
    }
}
