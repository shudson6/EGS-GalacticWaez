using GalacticWaez;
using System.Collections.Generic;

namespace GalacticWaezTests.Fakes
{
    class FakeResponder : IResponder
    {
        public delegate void SendDelegate(string text);

        private readonly SendDelegate DoStuff;
        public FakeResponder(SendDelegate doStuff = null) { DoStuff = doStuff; }
        public void Send(string text) => DoStuff(text);
    }

    class TestResponder : IResponder
    {
        public List<string> Messages = new List<string>();
        public void Send(string text)
        {
            Messages.Add(text);
        }
    }
}
