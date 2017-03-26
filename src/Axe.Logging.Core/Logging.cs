using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace Axe.Logging.Core
{
    public static class Logging
    {
        private const string LOG_ENTRY_KEY = "AxeLogging";

        public static T Mark<T>(this T exception, LogEntry log) where T : Exception
        {
            exception.Data.Add(LOG_ENTRY_KEY, log);
            return exception;
        }

        public static LogEntry GetLogEntry(this Exception exception, int maxLevel)
        {
            var currentLevel = 1;
            var exceptionWithLogEntry = GetExceptionWithLogEntry(exception, maxLevel, currentLevel);
            return exceptionWithLogEntry.Data[LOG_ENTRY_KEY] as LogEntry;
        }

        [SuppressMessage("ReSharper", "TailRecursiveCall")]
        private static Exception GetExceptionWithLogEntry(Exception exception, int maxLevel, int currentLevel)
        {
            return (exception.Data[LOG_ENTRY_KEY] != null  ||  exception.InnerException == null || currentLevel >= maxLevel) ? 
                exception : 
                GetExceptionWithLogEntry(exception.InnerException, maxLevel, currentLevel + 1);
        }
    }

    [Serializable]
    public class LogEntry
    {
       
        public LogEntry(DateTime time, string entry, object user, object data, Level level)
        {
            Time = time;
            Entry = entry;
            User = user;
            Data = data;
            Level = level;
        }

        public DateTime Time { get;}
        public string Entry { get; }
        public object User { get; }
        public object Data { get; }
        public Level Level { get; }
    }


    public enum Level
    {
        DefinedByBusiness,
        IKnowItWillHappen,
        Unknown
    }
}
