using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Assistants
{
	internal sealed class PerformanceCountersPerDatabaseInstance : PerformanceCounterInstance
	{
		internal PerformanceCountersPerDatabaseInstance(string instanceName, PerformanceCountersPerDatabaseInstance autoUpdateTotalInstance) : base(instanceName, "MSExchange Assistants - Per Database")
		{
			bool flag = false;
			List<ExPerformanceCounter> list = new List<ExPerformanceCounter>();
			try
			{
				this.EventsInQueueCurrent = new ExPerformanceCounter(base.CategoryName, "Events in queue", instanceName, true, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.EventsInQueueCurrent, new ExPerformanceCounter[0]);
				list.Add(this.EventsInQueueCurrent);
				this.AverageEventProcessingTime = new ExPerformanceCounter(base.CategoryName, "Average Event Processing Time In seconds", instanceName, true, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.AverageEventProcessingTime, new ExPerformanceCounter[0]);
				list.Add(this.AverageEventProcessingTime);
				this.AverageEventProcessingTimeBase = new ExPerformanceCounter(base.CategoryName, "Average Event Processing Time Base", instanceName, true, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.AverageEventProcessingTimeBase, new ExPerformanceCounter[0]);
				list.Add(this.AverageEventProcessingTimeBase);
				ExPerformanceCounter exPerformanceCounter = new ExPerformanceCounter(base.CategoryName, "Events Polled/sec", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter);
				this.EventsPolled = new ExPerformanceCounter(base.CategoryName, "Events Polled", instanceName, true, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.EventsPolled, new ExPerformanceCounter[]
				{
					exPerformanceCounter
				});
				list.Add(this.EventsPolled);
				this.PollingDelay = new ExPerformanceCounter(base.CategoryName, "Polling Delay", instanceName, true, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.PollingDelay, new ExPerformanceCounter[0]);
				list.Add(this.PollingDelay);
				this.HighestEventPolled = new ExPerformanceCounter(base.CategoryName, "Highest Event Counter Polled", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.HighestEventPolled);
				this.ElapsedTimeSinceLastEventPolled = new ExPerformanceCounter(base.CategoryName, "Elapsed Time Since Last Event Polled", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.ElapsedTimeSinceLastEventPolled);
				this.ElapsedTimeSinceLastEventPollingAttempt = new ExPerformanceCounter(base.CategoryName, "Elapsed Time Since Last Event Polling Attempt", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.ElapsedTimeSinceLastEventPollingAttempt);
				this.ElapsedTimeSinceLastDatabaseStatusUpdateAttempt = new ExPerformanceCounter(base.CategoryName, "Elapsed Time Since Last Database Status Update Attempt", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.ElapsedTimeSinceLastDatabaseStatusUpdateAttempt);
				this.InterestingEventsBase = new ExPerformanceCounter(base.CategoryName, "Base Counter for Percentage of Interesting Events", instanceName, true, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.InterestingEventsBase, new ExPerformanceCounter[0]);
				list.Add(this.InterestingEventsBase);
				this.EventsInterestingToMultipleAsssitants = new ExPerformanceCounter(base.CategoryName, "Events Interesting to Multiple Asssitants", instanceName, true, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.EventsInterestingToMultipleAsssitants, new ExPerformanceCounter[0]);
				list.Add(this.EventsInterestingToMultipleAsssitants);
				ExPerformanceCounter exPerformanceCounter2 = new ExPerformanceCounter(base.CategoryName, "Base Counter for Events Interesting to Multiple Asssitants", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter2);
				this.InterestingEvents = new ExPerformanceCounter(base.CategoryName, "Percentage of Interesting Events", instanceName, true, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.InterestingEvents, new ExPerformanceCounter[]
				{
					exPerformanceCounter2
				});
				list.Add(this.InterestingEvents);
				this.MailboxDispatchers = new ExPerformanceCounter(base.CategoryName, "Mailbox Dispatchers", instanceName, true, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.MailboxDispatchers, new ExPerformanceCounter[0]);
				list.Add(this.MailboxDispatchers);
				this.MailboxSessionsInUseByDispatchers = new ExPerformanceCounter(base.CategoryName, "Mailbox Sessions In Use By Dispatchers", instanceName, true, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.MailboxSessionsInUseByDispatchers, new ExPerformanceCounter[0]);
				list.Add(this.MailboxSessionsInUseByDispatchers);
				this.AverageMailboxProcessingTime = new ExPerformanceCounter(base.CategoryName, "Average Mailbox Processing Time In seconds", instanceName, true, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.AverageMailboxProcessingTime, new ExPerformanceCounter[0]);
				list.Add(this.AverageMailboxProcessingTime);
				this.AverageMailboxProcessingTimeBase = new ExPerformanceCounter(base.CategoryName, "Average Mailbox Processing Time In seconds Base", instanceName, true, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.AverageMailboxProcessingTimeBase, new ExPerformanceCounter[0]);
				list.Add(this.AverageMailboxProcessingTimeBase);
				ExPerformanceCounter exPerformanceCounter3 = new ExPerformanceCounter(base.CategoryName, "Mailboxes processed/sec", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter3);
				this.MailboxesProcessed = new ExPerformanceCounter(base.CategoryName, "Mailboxes Processed", instanceName, true, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.MailboxesProcessed, new ExPerformanceCounter[]
				{
					exPerformanceCounter3
				});
				list.Add(this.MailboxesProcessed);
				this.NumberOfThreadsUsed = new ExPerformanceCounter(base.CategoryName, "Number of Threads Used", instanceName, true, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.NumberOfThreadsUsed, new ExPerformanceCounter[0]);
				list.Add(this.NumberOfThreadsUsed);
				this.HealthMeasure = new ExPerformanceCounter(base.CategoryName, "Health Measure", instanceName, true, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.HealthMeasure, new ExPerformanceCounter[0]);
				list.Add(this.HealthMeasure);
				long num = this.EventsInQueueCurrent.RawValue;
				num += 1L;
				flag = true;
			}
			finally
			{
				if (!flag)
				{
					foreach (ExPerformanceCounter exPerformanceCounter4 in list)
					{
						exPerformanceCounter4.Close();
					}
				}
			}
			this.counters = list.ToArray();
		}

		internal PerformanceCountersPerDatabaseInstance(string instanceName) : base(instanceName, "MSExchange Assistants - Per Database")
		{
			bool flag = false;
			List<ExPerformanceCounter> list = new List<ExPerformanceCounter>();
			try
			{
				this.EventsInQueueCurrent = new ExPerformanceCounter(base.CategoryName, "Events in queue", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.EventsInQueueCurrent);
				this.AverageEventProcessingTime = new ExPerformanceCounter(base.CategoryName, "Average Event Processing Time In seconds", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.AverageEventProcessingTime);
				this.AverageEventProcessingTimeBase = new ExPerformanceCounter(base.CategoryName, "Average Event Processing Time Base", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.AverageEventProcessingTimeBase);
				ExPerformanceCounter exPerformanceCounter = new ExPerformanceCounter(base.CategoryName, "Events Polled/sec", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter);
				this.EventsPolled = new ExPerformanceCounter(base.CategoryName, "Events Polled", instanceName, true, null, new ExPerformanceCounter[]
				{
					exPerformanceCounter
				});
				list.Add(this.EventsPolled);
				this.PollingDelay = new ExPerformanceCounter(base.CategoryName, "Polling Delay", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.PollingDelay);
				this.HighestEventPolled = new ExPerformanceCounter(base.CategoryName, "Highest Event Counter Polled", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.HighestEventPolled);
				this.ElapsedTimeSinceLastEventPolled = new ExPerformanceCounter(base.CategoryName, "Elapsed Time Since Last Event Polled", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.ElapsedTimeSinceLastEventPolled);
				this.ElapsedTimeSinceLastEventPollingAttempt = new ExPerformanceCounter(base.CategoryName, "Elapsed Time Since Last Event Polling Attempt", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.ElapsedTimeSinceLastEventPollingAttempt);
				this.ElapsedTimeSinceLastDatabaseStatusUpdateAttempt = new ExPerformanceCounter(base.CategoryName, "Elapsed Time Since Last Database Status Update Attempt", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.ElapsedTimeSinceLastDatabaseStatusUpdateAttempt);
				this.InterestingEventsBase = new ExPerformanceCounter(base.CategoryName, "Base Counter for Percentage of Interesting Events", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.InterestingEventsBase);
				this.EventsInterestingToMultipleAsssitants = new ExPerformanceCounter(base.CategoryName, "Events Interesting to Multiple Asssitants", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.EventsInterestingToMultipleAsssitants);
				ExPerformanceCounter exPerformanceCounter2 = new ExPerformanceCounter(base.CategoryName, "Base Counter for Events Interesting to Multiple Asssitants", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter2);
				this.InterestingEvents = new ExPerformanceCounter(base.CategoryName, "Percentage of Interesting Events", instanceName, true, null, new ExPerformanceCounter[]
				{
					exPerformanceCounter2
				});
				list.Add(this.InterestingEvents);
				this.MailboxDispatchers = new ExPerformanceCounter(base.CategoryName, "Mailbox Dispatchers", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.MailboxDispatchers);
				this.MailboxSessionsInUseByDispatchers = new ExPerformanceCounter(base.CategoryName, "Mailbox Sessions In Use By Dispatchers", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.MailboxSessionsInUseByDispatchers);
				this.AverageMailboxProcessingTime = new ExPerformanceCounter(base.CategoryName, "Average Mailbox Processing Time In seconds", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.AverageMailboxProcessingTime);
				this.AverageMailboxProcessingTimeBase = new ExPerformanceCounter(base.CategoryName, "Average Mailbox Processing Time In seconds Base", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.AverageMailboxProcessingTimeBase);
				ExPerformanceCounter exPerformanceCounter3 = new ExPerformanceCounter(base.CategoryName, "Mailboxes processed/sec", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter3);
				this.MailboxesProcessed = new ExPerformanceCounter(base.CategoryName, "Mailboxes Processed", instanceName, true, null, new ExPerformanceCounter[]
				{
					exPerformanceCounter3
				});
				list.Add(this.MailboxesProcessed);
				this.NumberOfThreadsUsed = new ExPerformanceCounter(base.CategoryName, "Number of Threads Used", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.NumberOfThreadsUsed);
				this.HealthMeasure = new ExPerformanceCounter(base.CategoryName, "Health Measure", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.HealthMeasure);
				long num = this.EventsInQueueCurrent.RawValue;
				num += 1L;
				flag = true;
			}
			finally
			{
				if (!flag)
				{
					foreach (ExPerformanceCounter exPerformanceCounter4 in list)
					{
						exPerformanceCounter4.Close();
					}
				}
			}
			this.counters = list.ToArray();
		}

		public override void GetPerfCounterDiagnosticsInfo(XElement topElement)
		{
			XElement xelement = null;
			foreach (ExPerformanceCounter exPerformanceCounter in this.counters)
			{
				try
				{
					if (xelement == null)
					{
						xelement = new XElement(ExPerformanceCounter.GetEncodedName(exPerformanceCounter.InstanceName));
						topElement.Add(xelement);
					}
					xelement.Add(new XElement(ExPerformanceCounter.GetEncodedName(exPerformanceCounter.CounterName), exPerformanceCounter.NextValue()));
				}
				catch (XmlException ex)
				{
					XElement content = new XElement("Error", ex.Message);
					topElement.Add(content);
				}
			}
		}

		public readonly ExPerformanceCounter EventsInQueueCurrent;

		public readonly ExPerformanceCounter AverageEventProcessingTime;

		public readonly ExPerformanceCounter AverageEventProcessingTimeBase;

		public readonly ExPerformanceCounter EventsPolled;

		public readonly ExPerformanceCounter PollingDelay;

		public readonly ExPerformanceCounter HighestEventPolled;

		public readonly ExPerformanceCounter ElapsedTimeSinceLastEventPolled;

		public readonly ExPerformanceCounter ElapsedTimeSinceLastEventPollingAttempt;

		public readonly ExPerformanceCounter ElapsedTimeSinceLastDatabaseStatusUpdateAttempt;

		public readonly ExPerformanceCounter InterestingEvents;

		public readonly ExPerformanceCounter InterestingEventsBase;

		public readonly ExPerformanceCounter EventsInterestingToMultipleAsssitants;

		public readonly ExPerformanceCounter MailboxDispatchers;

		public readonly ExPerformanceCounter MailboxSessionsInUseByDispatchers;

		public readonly ExPerformanceCounter AverageMailboxProcessingTime;

		public readonly ExPerformanceCounter AverageMailboxProcessingTimeBase;

		public readonly ExPerformanceCounter MailboxesProcessed;

		public readonly ExPerformanceCounter NumberOfThreadsUsed;

		public readonly ExPerformanceCounter HealthMeasure;
	}
}
