using System;
using System.Diagnostics;
using Axe.Logging.Core;
using Xunit;

namespace Axe.Logging.Test
{
    public class LoggingFacts
    {
        [Fact]
        public void shold_can_mark_log_entry_for_any_exception()
        {
            var time = DateTime.UtcNow;
            var entry = "http://localhost:8080 Post";
            var user = new {Id = 1};
            var data = new {Country="China"};

            var logEntryDefinedByBusiness = new LogEntry(time, entry, user, data, Level.DefinedByBusiness);
            Exception exceptionDefinedByBusiness = new Exception().Mark(logEntryDefinedByBusiness);

            var logEntryIKnowItWillHappen = new LogEntry(time, entry, user, data, Level.IKnowItWillHappen);
            Exception exceptionIKnowItWillHappen = new Exception().Mark(logEntryIKnowItWillHappen);

            var logEntryUnKnown = new LogEntry(time, entry, user, data, Level.Unknown);
            Exception exceptionUnKnow = new Exception().Mark(logEntryUnKnown);

            Assert.Equal(logEntryDefinedByBusiness, exceptionDefinedByBusiness.Data["AxeLogging"]);
            Assert.Equal(logEntryIKnowItWillHappen, exceptionIKnowItWillHappen.Data["AxeLogging"]);
            Assert.Equal(logEntryUnKnown, exceptionUnKnow.Data["AxeLogging"]);
        }

        [Fact]
        public void should_get_log_entry_of_exception()
        {
            var time = DateTime.UtcNow;
            var entry = "http://localhost:8080 Post";
            var user = new { Id = 1 };
            var data = new { Country = "China" };

            var logEntry = new LogEntry(time, entry, user, data, Level.DefinedByBusiness);
            Exception exception = new Exception().Mark(logEntry);

            Assert.Equal(logEntry, exception.GetLogEntry(1));
        }

        [Fact]
        public void should_get_log_entry_of_exception_when_log_entry_existed_in_inner_exception()
        {
            var time = DateTime.UtcNow;
            var entry = "http://localhost:8080 Post";
            var user = new { Id = 1 };
            var data = new { Country = "China" };

            var logEntry = new LogEntry(time, entry, user, data, Level.DefinedByBusiness);
            var innerException = new Exception("inner exception").Mark(logEntry);
            Exception exception = new Exception("exception 1", innerException);

            Assert.Equal(logEntry, exception.GetLogEntry(2));
        }

        [Fact]
        public void should_get_log_entry_of_exception_in_certain_levels_given_max_level()
        {
            var time = DateTime.UtcNow;
            var entry = "http://localhost:8080 Post";
            var user = new { Id = 1 };
            var data = new { Country = "China" };
            var logEntry = new LogEntry(time, entry, user, data, Level.DefinedByBusiness);

            Exception logEntryInExceptionLevel2 = new Exception("exception1 level1", new Exception("exception1 level 2").Mark(logEntry));
            Exception logEntryInExceptionLevel3 = new Exception("exception2 level 1", new Exception("exception2 level 2", new Exception("exception2 level 3").Mark(logEntry)));

            Assert.Equal(logEntry, logEntryInExceptionLevel2.GetLogEntry(2));
            Assert.Equal(null, logEntryInExceptionLevel3.GetLogEntry(2));
        }


    }

}
