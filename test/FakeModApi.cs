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

        private readonly List<(LogType type, string message)> logs = new List<(LogType type, string message)>();
        private readonly IApplication app;

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

        public event GameEventDelegate GameEvent { add { } remove { } }

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

        /// <summary>
        /// Checks whether any message of 
        /// <paramref name="type"/>
        /// in the log starts with the specified
        /// <paramref name="text"/>.
        /// </summary>
        /// <param name="text">text to search for</param>
        /// <param name="type">type of log entry (<see cref="LogType"/>)</param>
        /// <returns>
        /// <c>true</c> if such an entry is found; <c>false</c> otherwise
        /// </returns>
        public bool LogContains(string text, LogType type = LogType.Normal)
        {
            foreach (var l in logs)
            {
                if (l.type == type && l.message.StartsWith(text))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
