using System;
using System.Collections.Generic;
using Eleon.Modding;

namespace GalacticWaezTests
{
    public class FakeGUI : IGui
    {
        private List<(string text, int prio, float duration)> messages =
            new List<(string text, int prio, float duration)>();

        public IReadOnlyList<(string text, int prio, float duration)> Messages => messages;

        public bool IsWorldVisible => throw new NotImplementedException();

        public bool ShowDialog(DialogConfig config, DialogActionHandler handler, int customValue)
        {
            throw new NotImplementedException();
        }

        public void ShowGameMessage(string text, int prio = 0, float duration = 3)
        {
            messages.Add((text, prio, duration));
        }
    }
}
