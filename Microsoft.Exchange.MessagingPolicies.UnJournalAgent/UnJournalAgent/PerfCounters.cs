using System;
using System.Xml;
using System.Xml.Linq;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.MessagingPolicies.UnJournalAgent
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

		public const string CategoryName = "MSExchange UnJournaling Agent";

		public static readonly ExPerformanceCounter MessagesProcessedPerHour = new ExPerformanceCounter("MSExchange UnJournaling Agent", "Messages Processed by Unjournaling per hour", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter MessagesUnjournaled = new ExPerformanceCounter("MSExchange UnJournaling Agent", "Messages Unjournaled successfully by Unjournaling", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter MessagesUnjournaledPerHour = new ExPerformanceCounter("MSExchange UnJournaling Agent", "Messages Unjournaled successfully by Unjournaling per hour", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter UsersUnjournaled = new ExPerformanceCounter("MSExchange UnJournaling Agent", "Unjournaled messages recipients by Unjournaling", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter UsersUnjournaledPerHour = new ExPerformanceCounter("MSExchange UnJournaling Agent", "Unjournaled messages recipients by Unjournaling per hour", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter DefectiveJournals = new ExPerformanceCounter("MSExchange UnJournaling Agent", "Messages marked as defective journals by Unjournaling", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter DefectiveJournalsPerHour = new ExPerformanceCounter("MSExchange UnJournaling Agent", "Messages marked as defective journals by Unjournaling per hour", string.Empty, null, new ExPerformanceCounter[0]);

		private static readonly ExPerformanceCounter AverageProcessingTime = new ExPerformanceCounter("MSExchange UnJournaling Agent", "UnJournaling Processing Time per Message", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter ProcessingTime = new ExPerformanceCounter("MSExchange UnJournaling Agent", "UnJournaling Processing Time", string.Empty, null, new ExPerformanceCounter[]
		{
			PerfCounters.AverageProcessingTime
		});

		private static readonly ExPerformanceCounter AverageProcessingTimeBase = new ExPerformanceCounter("MSExchange UnJournaling Agent", "Average unjournaling processing time/message base", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter MessagesProcessed = new ExPerformanceCounter("MSExchange UnJournaling Agent", "Messages Processed by Unjournaling", string.Empty, null, new ExPerformanceCounter[]
		{
			PerfCounters.AverageProcessingTimeBase
		});

		private static readonly ExPerformanceCounter MessagesUnjournaledSizePerSecond = new ExPerformanceCounter("MSExchange UnJournaling Agent", "Messages Unjournaled size per second", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalMessagesUnjournaledSize = new ExPerformanceCounter("MSExchange UnJournaling Agent", "Total Messages Unjournaled Size in bytes.", string.Empty, null, new ExPerformanceCounter[]
		{
			PerfCounters.MessagesUnjournaledSizePerSecond
		});

		public static readonly ExPerformanceCounter PermanentError = new ExPerformanceCounter("MSExchange UnJournaling Agent", "Permanent error by Unjournaling", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter PermanentErrorPerHour = new ExPerformanceCounter("MSExchange UnJournaling Agent", "Permanent error by Unjournaling per hour", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TransientError = new ExPerformanceCounter("MSExchange UnJournaling Agent", "Transient error by Unjournaling", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TransientErrorPerHour = new ExPerformanceCounter("MSExchange UnJournaling Agent", "Transient error by Unjournaling per hour", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter NdrProcessSuccess = new ExPerformanceCounter("MSExchange UnJournaling Agent", "Ndr successfully processed by Unjournaling", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter NdrProcessSuccessPerHour = new ExPerformanceCounter("MSExchange UnJournaling Agent", "Ndr successfully processed by Unjournaling per hour", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter LegacyArchiveJournallingDisabled = new ExPerformanceCounter("MSExchange UnJournaling Agent", "Legacy archive journalling disabled detected by Unjournaling", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter LegacyArchiveJournallingDisabledPerHour = new ExPerformanceCounter("MSExchange UnJournaling Agent", "Legacy archive journalling disabled detected by Unjournaling per hour", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter NonJournalMsgFromLegacyArchiveCustomer = new ExPerformanceCounter("MSExchange UnJournaling Agent", "Teed message detected by Unjournaling", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter NonJournalMsgFromLegacyArchiveCustomerPerHour = new ExPerformanceCounter("MSExchange UnJournaling Agent", "Teed message detected by Unjournaling per hour", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter AlreadyProcessed = new ExPerformanceCounter("MSExchange UnJournaling Agent", "Messages already processed detected by Unjournaling", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter AlreadyProcessedPerHour = new ExPerformanceCounter("MSExchange UnJournaling Agent", "Messages already processed detected by Unjournaling per hour", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter DropJournalReportWithoutNdr = new ExPerformanceCounter("MSExchange UnJournaling Agent", "Messages dropped silently by Unjournaling", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter DropJournalReportWithoutNdrPerHour = new ExPerformanceCounter("MSExchange UnJournaling Agent", "Messages dropped silently by Unjournaling per hour", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter NoUsersResolved = new ExPerformanceCounter("MSExchange UnJournaling Agent", "Messages with user unable to resolved by Unjournaling", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter NoUsersResolvedPerHour = new ExPerformanceCounter("MSExchange UnJournaling Agent", "Messages with user unable to resolved by Unjournaling per hour", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter NdrJournalReport = new ExPerformanceCounter("MSExchange UnJournaling Agent", "Messages ndr back to onpremise address by Unjournaling", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter NdrJournalReportPerHour = new ExPerformanceCounter("MSExchange UnJournaling Agent", "Messages ndr back to onpremise address by Unjournaling per hour", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter[] AllCounters = new ExPerformanceCounter[]
		{
			PerfCounters.MessagesProcessed,
			PerfCounters.MessagesProcessedPerHour,
			PerfCounters.MessagesUnjournaled,
			PerfCounters.MessagesUnjournaledPerHour,
			PerfCounters.UsersUnjournaled,
			PerfCounters.UsersUnjournaledPerHour,
			PerfCounters.TotalMessagesUnjournaledSize,
			PerfCounters.DefectiveJournals,
			PerfCounters.DefectiveJournalsPerHour,
			PerfCounters.ProcessingTime,
			PerfCounters.PermanentError,
			PerfCounters.PermanentErrorPerHour,
			PerfCounters.TransientError,
			PerfCounters.TransientErrorPerHour,
			PerfCounters.NdrProcessSuccess,
			PerfCounters.NdrProcessSuccessPerHour,
			PerfCounters.LegacyArchiveJournallingDisabled,
			PerfCounters.LegacyArchiveJournallingDisabledPerHour,
			PerfCounters.NonJournalMsgFromLegacyArchiveCustomer,
			PerfCounters.NonJournalMsgFromLegacyArchiveCustomerPerHour,
			PerfCounters.AlreadyProcessed,
			PerfCounters.AlreadyProcessedPerHour,
			PerfCounters.DropJournalReportWithoutNdr,
			PerfCounters.DropJournalReportWithoutNdrPerHour,
			PerfCounters.NoUsersResolved,
			PerfCounters.NoUsersResolvedPerHour,
			PerfCounters.NdrJournalReport,
			PerfCounters.NdrJournalReportPerHour
		};
	}
}
