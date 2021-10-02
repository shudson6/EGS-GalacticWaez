using GalacticWaez;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace GalacticWaezTests.Fakes
{
    class FakeResponder : IResponder
    {
        public void Send(string text)
        {
            throw new System.NotImplementedException();
        }
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
