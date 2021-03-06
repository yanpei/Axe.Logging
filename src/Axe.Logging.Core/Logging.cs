﻿using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Axe.Logging.Core
{
    public static class Logging
    {
        private const string LOG_ENTRY_KEY = "AxeLogging";

        public static T Mark<T>(this T exception, LogEntry log) where T : Exception
        {
            if (exception == null)
            {
                throw new ArgumentNullException(nameof(exception));
            }

            exception.Data.Add(LOG_ENTRY_KEY, log);
            return exception;
        }

        public static LogEntry[] GetLogEntry(this Exception exception, int maxLevel)
        {
            var currentLevel = 1;
            var logEntries = new List<LogEntry>();
            GetExceptionWithLogEntry(exception, maxLevel, currentLevel, ref logEntries);

            if (logEntries.Count == 0)
            {
                logEntries.Add(new LogEntry(Guid.NewGuid(), DateTime.UtcNow, default(string), null, null, Level.Unknown));
            }

            return logEntries.ToArray();
        }

        [SuppressMessage("ReSharper", "PossibleNullReferenceException")]
        private static void GetExceptionWithLogEntry(Exception exception, int maxLevel, int currentLevel, ref List<LogEntry> list)
        {
            if (currentLevel <= maxLevel)
            {
                if (exception.Data[LOG_ENTRY_KEY] != null)
                {
                    list.Add(exception.Data[LOG_ENTRY_KEY] as LogEntry);
                }

                if (exception.GetType() == new AggregateException().GetType())
                {
                    var aggregateException = exception as AggregateException;
                    foreach (var aggregateExceptionInnerException in aggregateException.InnerExceptions)
                    {
                        GetExceptionWithLogEntry(aggregateExceptionInnerException, maxLevel, currentLevel + 1, ref list);
                    }
                }
                else if (exception.InnerException != null)
                {
                    GetExceptionWithLogEntry(exception.InnerException, maxLevel, currentLevel + 1, ref list);
                }
            }
        }
    }

    [Serializable]
    public class LogEntry
    {
       
        public LogEntry(Guid id, DateTime time, string entry, object user, object data, Level level)
        {
            Id = id;
            Time = time;
            Entry = entry;
            User = user;
            Data = data;
            Level = level;
        }

        public Guid Id { get; }
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
