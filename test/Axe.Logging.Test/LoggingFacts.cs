using System;
using System.Runtime.Serialization;
using Axe.Logging.Core;
using Xunit;

namespace Axe.Logging.Test
{
    public class LoggingFacts
    {
        [Fact]
        public void shold_can_mark_log_entry_for_any_exception()
        {
            var logEntryDefinedByBusiness = CreateLogEntry();
            var exceptionDefinedByBusiness = new Exception().Mark(logEntryDefinedByBusiness);

            var logEntryIKnowItWillHappen =  CreateLogEntry(Level.IKnowItWillHappen);
            var exceptionIKnowItWillHappen = new SerializationException().Mark(logEntryIKnowItWillHappen);

            var logEntryUnKnown = CreateLogEntry(Level.Unknown);
            var exceptionUnKnow = new AggregateException().Mark(logEntryUnKnown);

            Assert.Equal(logEntryDefinedByBusiness, exceptionDefinedByBusiness.Data["AxeLogging"]);
            Assert.Equal(logEntryIKnowItWillHappen, exceptionIKnowItWillHappen.Data["AxeLogging"]);
            Assert.Equal(logEntryUnKnown, exceptionUnKnow.Data["AxeLogging"]);
        }

        [Fact]
        public void should_get_log_entry_of_exception()
        {
            var logEntry = CreateLogEntry();
            Exception exception = new Exception().Mark(logEntry);

            Assert.Equal(logEntry, exception.GetLogEntry(1)[0]);
        }

        [Fact]
        public void should_get_log_entry_of_exception_when_log_entry_existed_in_inner_exception()
        {
            var logEntry = CreateLogEntry();
            var innerException = new Exception("inner exception").Mark(logEntry);
            Exception exception = new Exception("exception 1", innerException);

            Assert.Equal(logEntry, exception.GetLogEntry(2)[0]);
        }

        [Fact]
        public void should_get_log_entry_of_exception_in_certain_levels_given_max_level()
        {
            var logEntry = CreateLogEntry();

            Exception logEntryInExceptionLevel2 = new Exception("exception1 level1", new Exception("exception1 level 2").Mark(logEntry));
            Exception logEntryInExceptionLevel3 = new Exception("exception2 level 1", new Exception("exception2 level 2", new Exception("exception2 level 3").Mark(logEntry)));

            Assert.Equal(logEntry, logEntryInExceptionLevel2.GetLogEntry(2)[0]);
            Assert.Equal(0, logEntryInExceptionLevel3.GetLogEntry(2).Length);
        }

        [Fact]
        public void should_get_log_entry_of_aggreate_exception()
        {
            var id = Guid.NewGuid();
            var logEntry1 = CreateLogEntry(Level.DefinedByBusiness, id);
            var logEntry2 = CreateLogEntry(Level.IKnowItWillHappen, id);

            Exception exception1 = new Exception("exception1").Mark(logEntry1);
            Exception exception2 = new Exception("exception2").Mark(logEntry2);
            var aggregateException = new AggregateException(exception1, exception2);

            var logEntries = aggregateException.GetLogEntry(2);
            Assert.Equal(logEntry1, logEntries[0]);
            Assert.Equal(logEntry2, logEntries[1]);
        }

        [Fact]
        public void should_apply_max_level_constaraint_to_each_child_for_exception_contains_aggreate_exception()
        {
            
            var id = Guid.NewGuid();
            var logEntry1 = CreateLogEntry(Level.IKnowItWillHappen, id);
            var logEntry2 = CreateLogEntry(Level.Unknown, id);

            Exception exception1 = new Exception("exception level 3").Mark(logEntry1);
            Exception exception2 = new Exception("exception level 3", new Exception("exception level 4").Mark(logEntry2));
            var aggregateException = new Exception("exception level 1", new AggregateException(exception1, exception2));

            Assert.Equal(1,aggregateException.GetLogEntry(3).Length);
            Assert.Equal(logEntry1, aggregateException.GetLogEntry(3)[0]); 
        }

        [Fact]
        public void should_get_all_log_entries_when_exception_and_inner_exception_both_have_log_entry()
        {
            var id = Guid.NewGuid();
            var logEntryUnknown = CreateLogEntry(Level.Unknown, id);
            var logEntryDefinedByBusiness = CreateLogEntry(Level.DefinedByBusiness, id);
            var logEntryIKnowItWillHappen = CreateLogEntry(Level.IKnowItWillHappen);

            Exception exception1 = new Exception("exception level 3").Mark(logEntryDefinedByBusiness);
            Exception exception2 = new Exception("exception level 3").Mark(logEntryIKnowItWillHappen);
            var aggregateException = new Exception("exception level 1", new AggregateException(exception1, exception2)).Mark(logEntryUnknown);

            var logEntries = aggregateException.GetLogEntry(3);
            Assert.Equal(3, logEntries.Length);
            Assert.Equal(logEntryUnknown, logEntries[0]);
            Assert.Equal(logEntryDefinedByBusiness, logEntries[1]);
            Assert.Equal(logEntryIKnowItWillHappen, logEntries[2]);
        }

        private static LogEntry CreateLogEntry(Level level = Level.DefinedByBusiness, Guid id = default(Guid))
        {
            var time = DateTime.UtcNow;
            var entry = "This is Entry";
            var user = new {Id = 1};
            var data = new {Country = "China"};
            var logEntryId = id == default(Guid) ? Guid.NewGuid() : id;

            return new LogEntry(logEntryId, time, entry, user, data, level);
        }
    }

}
