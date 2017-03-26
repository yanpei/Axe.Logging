using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace Axe.Logging.Core
{
    public static class Logging
    {
        public static T Mark<T>(this T exception, LogEntry log) where T : Exception
        {
            exception.Data.Add("AxeLogging", log);
            return exception;
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
