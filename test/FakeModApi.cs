using System;
using System.Collections.Generic;
using Eleon.Modding;

namespace GalacticWaezTests
{
    public class FakeModApi : IModApi
    {
        public enum LogType
        {
            Normal,
            Warning,
            Error
        }

        private List<(LogType type, string message)> logs = new List<(LogType type, string message)>();
        private IApplication app;

        public FakeModApi(IApplication fakeApp)
        {
            app = fakeApp;
        }

        public IReadOnlyList<(LogType type, string message)> Logs => logs;

        public IPlayfield ClientPlayfield { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public INetwork Network => throw new NotImplementedException();

        public IGui GUI => throw new NotImplementedException();

        public IPda PDA { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public IScript Scripting => throw new NotImplementedException();

        public ISoundPlayer SoundPlayer => throw new NotImplementedException();

        public IApplication Application => app;

        public event GameEventDelegate GameEvent;

        public void Log(string text)
        {
            logs.Add((LogType.Normal, text));
        }

        public void LogError(string text)
        {
            logs.Add((LogType.Error, text));
        }

        public void LogWarning(string text)
        {
            logs.Add((LogType.Warning, text));
        }

        public bool LogContains(string message, LogType type = LogType.Normal)
        {
            foreach (var l in logs)
            {
                if (l.type == type && l.message.StartsWith(message))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
