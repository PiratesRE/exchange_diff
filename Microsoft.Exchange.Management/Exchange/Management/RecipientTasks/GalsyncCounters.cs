using System;
using System.Xml;
using System.Xml.Linq;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Management.RecipientTasks
{
	internal static class GalsyncCounters
	{
		public static void GetPerfCounterInfo(XElement element)
		{
			if (GalsyncCounters.AllCounters == null)
			{
				return;
			}
			foreach (ExPerformanceCounter exPerformanceCounter in GalsyncCounters.AllCounters)
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

		public const string CategoryName = "GALSync";

		public static readonly ExPerformanceCounter NumberOfMailboxesCreated = new ExPerformanceCounter("GALSync", "Number of mailboxes created", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter ClientReportedNumberOfMailboxesCreated = new ExPerformanceCounter("GALSync", "Client reported number of mailboxes created", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter ClientReportedNumberOfMailboxesToCreate = new ExPerformanceCounter("GALSync", "Client reported number of mailboxes to create", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter ClientReportedMailboxCreationElapsedMilliseconds = new ExPerformanceCounter("GALSync", "Client reported total time used for Mailbox creation in milliseconds", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter NumberOfExportSyncRuns = new ExPerformanceCounter("GALSync", "Number of export sync runs", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter NumberOfImportSyncRuns = new ExPerformanceCounter("GALSync", "Number of import sync runs", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter NumberOfSucessfulExportSyncRuns = new ExPerformanceCounter("GALSync", "Number of sucessful export sync runs", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter NumberOfSucessfulImportSyncRuns = new ExPerformanceCounter("GALSync", "Number of sucessful import sync runs", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter NumberOfConnectionErrors = new ExPerformanceCounter("GALSync", "Number of connection errors", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter NumberOfPermissionErrors = new ExPerformanceCounter("GALSync", "Number of permission errors", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter NumberOfLiveIdErrors = new ExPerformanceCounter("GALSync", "Number of LiveId errors", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter NumberOfILMLogicErrors = new ExPerformanceCounter("GALSync", "Number of ILM logic errors", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter NumberOfILMOtherErrors = new ExPerformanceCounter("GALSync", "Number of ILM other errors", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter[] AllCounters = new ExPerformanceCounter[]
		{
			GalsyncCounters.NumberOfMailboxesCreated,
			GalsyncCounters.ClientReportedNumberOfMailboxesCreated,
			GalsyncCounters.ClientReportedNumberOfMailboxesToCreate,
			GalsyncCounters.ClientReportedMailboxCreationElapsedMilliseconds,
			GalsyncCounters.NumberOfExportSyncRuns,
			GalsyncCounters.NumberOfImportSyncRuns,
			GalsyncCounters.NumberOfSucessfulExportSyncRuns,
			GalsyncCounters.NumberOfSucessfulImportSyncRuns,
			GalsyncCounters.NumberOfConnectionErrors,
			GalsyncCounters.NumberOfPermissionErrors,
			GalsyncCounters.NumberOfLiveIdErrors,
			GalsyncCounters.NumberOfILMLogicErrors,
			GalsyncCounters.NumberOfILMOtherErrors
		};
	}
}
