using Eleon;
using GalacticWaez;

namespace GalacticWaezTests.Fakes
{
    class FakeResponseManager : IResponseManager
    {
        public delegate IResponder ResponderDelegate(MessageData msg);
        private readonly ResponderDelegate DoStuff;
        public FakeResponseManager(ResponderDelegate doStuff) { DoStuff = doStuff; }
        public IResponder CreateResponder(MessageData msg) => DoStuff(msg);
    }
}
