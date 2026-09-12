using System;
using System.IO;

namespace WebControls.Diagnostics
{
    public enum LogLevel
    {
        All = 0,
        Debug = 1,
        Info = 2,
        Message = 3,
        Warning = 4,
        Error = 5,
        None = 99,
    }

    /// <summary>
    /// Minimal console + file logger. Drop-in replacement for the proprietary
    /// LogManager assembly the framework originally depended on; every call
    /// shape used by the controls (format string with optional level) is kept.
    /// Set LOG_DIR to write a per-run log file; console output is always on.
    /// </summary>
    public static class Logger
    {
        private static readonly object Sync = new object();
        private static StreamWriter _file;

        public static LogLevel LoggingLevel { get; set; } = LogLevel.All;
        public static string DefaultSessionName { get; set; } = "Automation";

        public static void StartLogging(bool toConsole, LogLevel level)
        {
            LoggingLevel = level;
            var dir = Environment.GetEnvironmentVariable("LOG_DIR");
            if (string.IsNullOrWhiteSpace(dir)) return;
            lock (Sync)
            {
                Directory.CreateDirectory(dir);
                var path = Path.Combine(dir, $"{DefaultSessionName}_{DateTime.Now:yyyyMMdd_HHmmss}.log");
                _file = new StreamWriter(path, append: true) { AutoFlush = true };
            }
        }

        public static void AddNewSession(string sessionName, LogLevel level)
        {
            // Sessions were a feature of the original assembly; a single sink is enough here.
            LogMessage(LogLevel.Debug, "Log session '{0}' requested at level {1}", sessionName, level);
        }

        public static void LogMessage(string format, params object[] args) =>
            LogMessage(LogLevel.Message, format, args);

        public static void LogMessage(LogLevel level, string format, params object[] args)
        {
            if (level < LoggingLevel) return;
            string text;
            try
            {
                text = args == null || args.Length == 0 ? format : string.Format(format, args);
            }
            catch (FormatException)
            {
                // Some call sites pass a template with placeholders but no arguments.
                text = format;
            }

            var line = $"{DateTime.Now:HH:mm:ss.fff} [{level,-7}] {text}";
            lock (Sync)
            {
                Console.WriteLine(line);
                _file?.WriteLine(line);
            }
        }
    }
}
