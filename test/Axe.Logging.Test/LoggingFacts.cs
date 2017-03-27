using System;
using System.Linq;
using Axe.Logging.Core;
using Xunit;

namespace Axe.Logging.Test
{
    public class LoggingFacts
    {
        [Fact]
        public void shold_can_mark_log_entry_for_any_exception()
        {
            LogEntry doNotCare1 = CreateLogEntry(Level.Unknown);
            var aggregateException = new AggregateException().Mark(doNotCare1);

            LogEntry doNotCare2 = CreateLogEntry();
            var exception = new Exception().Mark(doNotCare2);

            Assert.Equal(doNotCare1, aggregateException.GetLogEntry(1).Single());
            Assert.Equal(doNotCare2, exception.GetLogEntry(1).Single());
        }

        [Fact]
        public void should_get_log_entry_of_exception()
        {
            LogEntry doNotCare = CreateLogEntry();
            var exception = new Exception().Mark(doNotCare);

            Assert.Equal(doNotCare, exception.GetLogEntry(1).Single());
        }

        [Fact]
        public void should_get_log_entry_of_exception_when_log_entry_existed_in_inner_exception()
        {
            LogEntry doNotCare = CreateLogEntry();
            var innerException = new Exception("inner exception").Mark(doNotCare);
            var exception = new Exception("exception 1", innerException);

            Assert.Equal(doNotCare, exception.GetLogEntry(2).Single());
        }

        [Fact]
        public void should_get_log_entry_of_exception_in_certain_levels_given_max_level()
        {
            LogEntry logEntry1 = CreateLogEntry();
            LogEntry logEntry2 = CreateLogEntry(Level.IKnowItWillHappen);

            var logEntryInExceptionLevel2 = new Exception("exception1 level1", new Exception("exception1 level 2").Mark(logEntry1));
            var logEntryInExceptionLevel3 = new Exception("exception2 level 1", new Exception("exception2 level 2", new Exception("exception2 level 3").Mark(logEntry2)));

            Assert.Equal(logEntry1, logEntryInExceptionLevel2.GetLogEntry(2).Single());
            Assert.Equal(0, logEntryInExceptionLevel3.GetLogEntry(2).Length);
        }

        [Fact]
        public void should_get_log_entry_of_aggreate_exception()
        {
            var doNotCareId = Guid.NewGuid();
            LogEntry doNotCare1 = CreateLogEntry(Level.DefinedByBusiness, doNotCareId);
            LogEntry doNotCare2 = CreateLogEntry(Level.IKnowItWillHappen, doNotCareId);

            var exception1 = new Exception("exception1").Mark(doNotCare1);
            var exception2 = new Exception("exception2").Mark(doNotCare2);
            var aggregateException = new AggregateException(exception1, exception2);

            LogEntry[] logEntries = aggregateException.GetLogEntry(2);

            Assert.Equal(doNotCare1, logEntries[0]);
            Assert.Equal(doNotCare2, logEntries[1]);
        }

        [Fact]
        public void should_apply_max_level_constaraint_to_each_child_for_exception_contains_aggreate_exception()
        {
            var id = Guid.NewGuid();
            LogEntry logEntry1 = CreateLogEntry(Level.IKnowItWillHappen, id);
            LogEntry logEntry2 = CreateLogEntry(Level.Unknown, id);

            var exception1 = new Exception("exception level 3").Mark(logEntry1);
            var exception2 = new Exception("exception level 3", new Exception("exception level 4").Mark(logEntry2));
            var aggregateException = new Exception("exception level 1", new AggregateException(exception1, exception2));

            Assert.Equal(logEntry1, aggregateException.GetLogEntry(3).Single());
        }

        [Fact]
        public void should_get_all_log_entries_when_exception_and_inner_exception_both_have_log_entry()
        {
            var id = Guid.NewGuid();
            LogEntry doNotCare1 = CreateLogEntry(Level.Unknown, id);
            LogEntry doNotCare2 = CreateLogEntry(Level.DefinedByBusiness, id);
            LogEntry doNotCare3 = CreateLogEntry(Level.IKnowItWillHappen, id);

            var exception1 = new Exception("exception level 3").Mark(doNotCare2);
            var exception2 = new Exception("exception level 3").Mark(doNotCare3);
            var aggregateException = new Exception("exception level 1", new AggregateException(exception1, exception2)).Mark(doNotCare1);

            var logEntries = aggregateException.GetLogEntry(3);

            Assert.Equal(3, logEntries.Length);
            Assert.Equal(doNotCare1, logEntries[0]);
            Assert.Equal(doNotCare2, logEntries[1]);
            Assert.Equal(doNotCare3, logEntries[2]);
        }

        private static LogEntry CreateLogEntry(Level level = Level.DefinedByBusiness, Guid id = default(Guid))
        {
            var time = DateTime.UtcNow;
            const string entry = "This is Entry";
            var user = new {Id = 1};
            var data = new {Country = "China"};
            var logEntryId = id == default(Guid) ? Guid.NewGuid() : id;

            return new LogEntry(logEntryId, time, entry, user, data, level);
        }
    }

}
