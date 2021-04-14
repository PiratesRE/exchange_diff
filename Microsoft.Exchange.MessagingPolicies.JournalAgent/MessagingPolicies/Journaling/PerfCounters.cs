using System;
using System.Xml;
using System.Xml.Linq;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.MessagingPolicies.Journaling
{
	internal static class PerfCounters
	{
		public static void GetPerfCounterInfo(XElement element)
		{
			if (PerfCounters.AllCounters == null)
			{
				return;
			}
			foreach (ExPerformanceCounter exPerformanceCounter in PerfCounters.AllCounters)
			{
				try
				{
					element.Add(new XElement(ExPerformanceCounter.GetEncodedName(exPerformanceCounter.CounterName), exPerformanceCounter.NextValue()));
				}
				catch (XmlException ex)
				{
					XElement content = new XElement("Error", ex.Message);
					element.Add(content);
				}
			}
		}

		public const string CategoryName = "MSExchange Journaling Agent";

		private static readonly ExPerformanceCounter UsersJournaledRate = new ExPerformanceCounter("MSExchange Journaling Agent", "Users Journaled/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter UsersJournaled = new ExPerformanceCounter("MSExchange Journaling Agent", "Users Journaled", string.Empty, null, new ExPerformanceCounter[]
		{
			PerfCounters.UsersJournaledRate
		});

		public static readonly ExPerformanceCounter UsersJournaledPerHour = new ExPerformanceCounter("MSExchange Journaling Agent", "Users Journaled/hour", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter ReportsGeneratedWithRMSProtectedMessage = new ExPerformanceCounter("MSExchange Journaling Agent", "Journal Reports with RMS protected messages created", string.Empty, null, new ExPerformanceCounter[0]);

		private static readonly ExPerformanceCounter ReportsGeneratedRate = new ExPerformanceCounter("MSExchange Journaling Agent", "Journal Reports Created/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter ReportsGenerated = new ExPerformanceCounter("MSExchange Journaling Agent", "Journal Reports created Total", string.Empty, null, new ExPerformanceCounter[]
		{
			PerfCounters.ReportsGeneratedRate
		});

		public static readonly ExPerformanceCounter ReportsGeneratedPerHour = new ExPerformanceCounter("MSExchange Journaling Agent", "Journal Reports Created/hour", string.Empty, null, new ExPerformanceCounter[0]);

		private static readonly ExPerformanceCounter AverageProcessingTime = new ExPerformanceCounter("MSExchange Journaling Agent", "Journaling Processing Time per Message", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter ProcessingTime = new ExPerformanceCounter("MSExchange Journaling Agent", "Journaling Processing Time", string.Empty, null, new ExPerformanceCounter[]
		{
			PerfCounters.AverageProcessingTime
		});

		private static readonly ExPerformanceCounter AverageProcessingTimeBase = new ExPerformanceCounter("MSExchange Journaling Agent", "Average journaling processing time/message base", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter MessagesProcessed = new ExPerformanceCounter("MSExchange Journaling Agent", "Messages Processed by Journaling", string.Empty, null, new ExPerformanceCounter[]
		{
			PerfCounters.AverageProcessingTimeBase
		});

		private static readonly ExPerformanceCounter MessagesDeferredWithinJournalAgentRates = new ExPerformanceCounter("MSExchange Journaling Agent", "Messages Deferred Within Journal Agent/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter MessagesDeferredWithinJournalAgent = new ExPerformanceCounter("MSExchange Journaling Agent", "Messages Deferred Within Journal Agent", string.Empty, null, new ExPerformanceCounter[]
		{
			PerfCounters.MessagesDeferredWithinJournalAgentRates
		});

		public static readonly ExPerformanceCounter MessagesDeferredWithinJournalAgentPerHour = new ExPerformanceCounter("MSExchange Journaling Agent", "Messages Deferred Within Journal Agent/hour", string.Empty, null, new ExPerformanceCounter[0]);

		private static readonly ExPerformanceCounter JournalReportsThatCouldNotBeCreatedRates = new ExPerformanceCounter("MSExchange Journaling Agent", "Journal Reports That Could Not Be Created/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter JournalReportsThatCouldNotBeCreated = new ExPerformanceCounter("MSExchange Journaling Agent", "Journal Reports That Could Not Be Created", string.Empty, null, new ExPerformanceCounter[]
		{
			PerfCounters.JournalReportsThatCouldNotBeCreatedRates
		});

		public static readonly ExPerformanceCounter JournalReportsThatCouldNotBeCreatedPerHour = new ExPerformanceCounter("MSExchange Journaling Agent", "Journal Reports That Could Not Be Created/hour", string.Empty, null, new ExPerformanceCounter[0]);

		private static readonly ExPerformanceCounter MessagesDeferredWithinJournalAgentLawfulInterceptRates = new ExPerformanceCounter("MSExchange Journaling Agent", "Messages Deferred Within Journal Agent (Lawful Intercept)/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter MessagesDeferredWithinJournalAgentLawfulIntercept = new ExPerformanceCounter("MSExchange Journaling Agent", "Messages Deferred Within Journal Agent (Lawful Intercept)", string.Empty, null, new ExPerformanceCounter[]
		{
			PerfCounters.MessagesDeferredWithinJournalAgentLawfulInterceptRates
		});

		public static readonly ExPerformanceCounter MessagesDeferredWithinJournalAgentLawfulInterceptPerHour = new ExPerformanceCounter("MSExchange Journaling Agent", "Messages Deferred Within Journal Agent (Lawful Intercept)/hour", string.Empty, null, new ExPerformanceCounter[0]);

		private static readonly ExPerformanceCounter IncomingJournalReportsDroppedRates = new ExPerformanceCounter("MSExchange Journaling Agent", "Incoming Journal Reports (Dropped)/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter IncomingJournalReportsDropped = new ExPerformanceCounter("MSExchange Journaling Agent", "Incoming Journal Reports (Dropped)", string.Empty, null, new ExPerformanceCounter[]
		{
			PerfCounters.IncomingJournalReportsDroppedRates
		});

		public static readonly ExPerformanceCounter IncomingJournalReportsDroppedPerHour = new ExPerformanceCounter("MSExchange Journaling Agent", "Incoming Journal Reports (Dropped)/hour", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter[] AllCounters = new ExPerformanceCounter[]
		{
			PerfCounters.UsersJournaled,
			PerfCounters.UsersJournaledPerHour,
			PerfCounters.ReportsGenerated,
			PerfCounters.ReportsGeneratedWithRMSProtectedMessage,
			PerfCounters.ReportsGeneratedPerHour,
			PerfCounters.MessagesProcessed,
			PerfCounters.ProcessingTime,
			PerfCounters.MessagesDeferredWithinJournalAgent,
			PerfCounters.JournalReportsThatCouldNotBeCreated,
			PerfCounters.MessagesDeferredWithinJournalAgentLawfulIntercept,
			PerfCounters.IncomingJournalReportsDropped,
			PerfCounters.MessagesDeferredWithinJournalAgentPerHour,
			PerfCounters.JournalReportsThatCouldNotBeCreatedPerHour,
			PerfCounters.MessagesDeferredWithinJournalAgentLawfulInterceptPerHour,
			PerfCounters.IncomingJournalReportsDroppedPerHour
		};
	}
}
