using GalacticWaez;

namespace GalacticWaezTests.Fakes
{
    class FakeNavigator : INavigator
    {
        public IPlayerInfo Player { get; private set; }
        public string Goal { get; private set; }
        public float Range { get; private set; }
        public void Navigate(IPlayerInfo player, string destination, float playerRange, IResponder response)
        {
            Player = player;
            Goal = destination;
            Range = playerRange;
        }
    }
}
