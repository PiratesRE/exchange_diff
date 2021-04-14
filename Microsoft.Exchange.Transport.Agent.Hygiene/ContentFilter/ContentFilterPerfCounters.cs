using System;
using System.Xml;
using System.Xml.Linq;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Transport.Agent.ContentFilter
{
	internal static class ContentFilterPerfCounters
	{
		public static void GetPerfCounterInfo(XElement element)
		{
			if (ContentFilterPerfCounters.AllCounters == null)
			{
				return;
			}
			foreach (ExPerformanceCounter exPerformanceCounter in ContentFilterPerfCounters.AllCounters)
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

		public const string CategoryName = "MSExchange Content Filter Agent";

		public static readonly ExPerformanceCounter TotalMessagesNotScanned = new ExPerformanceCounter("MSExchange Content Filter Agent", "Messages that Bypassed Scanning", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalMessagesWithPreExistingSCL = new ExPerformanceCounter("MSExchange Content Filter Agent", "Messages with a Preexisting SCL", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalMessagesThatCauseFilterFailure = new ExPerformanceCounter("MSExchange Content Filter Agent", "Messages with SCL Unknown", string.Empty, null, new ExPerformanceCounter[0]);

		private static readonly ExPerformanceCounter MessagesScannedPerSecond = new ExPerformanceCounter("MSExchange Content Filter Agent", "Messages Scanned Per Second", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalMessagesScanned = new ExPerformanceCounter("MSExchange Content Filter Agent", "Messages Scanned", string.Empty, null, new ExPerformanceCounter[]
		{
			ContentFilterPerfCounters.MessagesScannedPerSecond
		});

		public static readonly ExPerformanceCounter MessagesAtSCL0 = new ExPerformanceCounter("MSExchange Content Filter Agent", "Messages with SCL 0", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter MessagesAtSCL1 = new ExPerformanceCounter("MSExchange Content Filter Agent", "Messages with SCL 1", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter MessagesAtSCL2 = new ExPerformanceCounter("MSExchange Content Filter Agent", "Messages with SCL 2", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter MessagesAtSCL3 = new ExPerformanceCounter("MSExchange Content Filter Agent", "Messages with SCL 3", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter MessagesAtSCL4 = new ExPerformanceCounter("MSExchange Content Filter Agent", "Messages with SCL 4", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter MessagesAtSCL5 = new ExPerformanceCounter("MSExchange Content Filter Agent", "Messages with SCL 5", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter MessagesAtSCL6 = new ExPerformanceCounter("MSExchange Content Filter Agent", "Messages with SCL 6", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter MessagesAtSCL7 = new ExPerformanceCounter("MSExchange Content Filter Agent", "Messages with SCL 7", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter MessagesAtSCL8 = new ExPerformanceCounter("MSExchange Content Filter Agent", "Messages with SCL 8", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter MessagesAtSCL9 = new ExPerformanceCounter("MSExchange Content Filter Agent", "Messages with SCL 9", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalMessagesDeleted = new ExPerformanceCounter("MSExchange Content Filter Agent", "Messages Deleted", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalMessagesRejected = new ExPerformanceCounter("MSExchange Content Filter Agent", "Messages Rejected", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalMessagesQuarantined = new ExPerformanceCounter("MSExchange Content Filter Agent", "Messages Quarantined", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalMessagesNotScannedDueToOrgSafeSender = new ExPerformanceCounter("MSExchange Content Filter Agent", "Messages that Bypassed Scanning due to an organization-wide Safe Sender", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalBypassedRecipientsDueToPerRecipientSafeSender = new ExPerformanceCounter("MSExchange Content Filter Agent", "Bypassed recipients due to per-recipient Safe Senders", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalBypassedRecipientsDueToPerRecipientSafeRecipient = new ExPerformanceCounter("MSExchange Content Filter Agent", "Bypassed recipients due to per-recipient Safe Recipients", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalMessagesWithValidPostmarks = new ExPerformanceCounter("MSExchange Content Filter Agent", "Messages that include an Outlook E-mail Postmark that validated successfully", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalMessagesWithInvalidPostmarks = new ExPerformanceCounter("MSExchange Content Filter Agent", "Messages that include an Outlook E-mail Postmark that did not validate", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter[] AllCounters = new ExPerformanceCounter[]
		{
			ContentFilterPerfCounters.TotalMessagesNotScanned,
			ContentFilterPerfCounters.TotalMessagesWithPreExistingSCL,
			ContentFilterPerfCounters.TotalMessagesScanned,
			ContentFilterPerfCounters.TotalMessagesThatCauseFilterFailure,
			ContentFilterPerfCounters.MessagesAtSCL0,
			ContentFilterPerfCounters.MessagesAtSCL1,
			ContentFilterPerfCounters.MessagesAtSCL2,
			ContentFilterPerfCounters.MessagesAtSCL3,
			ContentFilterPerfCounters.MessagesAtSCL4,
			ContentFilterPerfCounters.MessagesAtSCL5,
			ContentFilterPerfCounters.MessagesAtSCL6,
			ContentFilterPerfCounters.MessagesAtSCL7,
			ContentFilterPerfCounters.MessagesAtSCL8,
			ContentFilterPerfCounters.MessagesAtSCL9,
			ContentFilterPerfCounters.TotalMessagesDeleted,
			ContentFilterPerfCounters.TotalMessagesRejected,
			ContentFilterPerfCounters.TotalMessagesQuarantined,
			ContentFilterPerfCounters.TotalMessagesNotScannedDueToOrgSafeSender,
			ContentFilterPerfCounters.TotalBypassedRecipientsDueToPerRecipientSafeSender,
			ContentFilterPerfCounters.TotalBypassedRecipientsDueToPerRecipientSafeRecipient,
			ContentFilterPerfCounters.TotalMessagesWithValidPostmarks,
			ContentFilterPerfCounters.TotalMessagesWithInvalidPostmarks
		};
	}
}
