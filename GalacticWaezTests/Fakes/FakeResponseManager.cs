using Eleon;
using GalacticWaez;

namespace GalacticWaezTests.Fakes
{
    class FakeResponseManager : IResponseManager
    {
        private readonly IResponder responder;
        public FakeResponseManager(IResponder rsp) { responder = rsp; }
        public IResponder CreateResponder(MessageData msg) => responder;
    }
}
