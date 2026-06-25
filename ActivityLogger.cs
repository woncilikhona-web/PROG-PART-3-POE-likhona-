using System;
using System.Collections.Generic;
using System.Linq;

namespace CyberLikh
{
    public class ActivityLogger
    {
        private List<string> _log = new List<string>();

        public void Log(string action)
        {
            string entry = DateTime.Now.ToString("[HH:mm] ") + action;
            _log.Add(entry);
        }

        public string GetRecentLog(int count = 10)
        {
            var recent = _log.Skip(Math.Max(0, _log.Count - count)).ToList();
            if (recent.Count == 0)
                return "No activity recorded yet.";

            var lines = new List<string> { "Here's a summary of recent actions:" };
            for (int i = 0; i < recent.Count; i++)
                lines.Add($"{i + 1}. {recent[i]}");

            return string.Join("\n", lines);
        }

        public string GetFullLog()
        {
            if (_log.Count == 0)
                return "No activity recorded yet.";

            var lines = new List<string> { "Full activity history:" };
            for (int i = 0; i < _log.Count; i++)
                lines.Add($"{i + 1}. {_log[i]}");

            return string.Join("\n", lines);
        }

        public int GetCount() => _log.Count;
    }
}