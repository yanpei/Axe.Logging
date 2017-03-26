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

            Assert.Equal(exceptionDefinedByBusiness.Data["AxeLogging"], logEntryDefinedByBusiness);
            Assert.Equal(exceptionIKnowItWillHappen.Data["AxeLogging"], logEntryIKnowItWillHappen);
            Assert.Equal(exceptionUnKnow.Data["AxeLogging"], logEntryUnKnown);
        }

    }

}
