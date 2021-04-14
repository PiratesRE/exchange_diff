using System;
using System.Xml;
using System.Xml.Linq;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.UM.UMCore
{
	internal static class AvailabilityCounters
	{
		public static void GetPerfCounterInfo(XElement element)
		{
			if (AvailabilityCounters.AllCounters == null)
			{
				return;
			}
			foreach (ExPerformanceCounter exPerformanceCounter in AvailabilityCounters.AllCounters)
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

		public const string CategoryName = "MSExchangeUMAvailability";

		public static readonly ExPerformanceCounter DirectoryAccessFailures = new ExPerformanceCounter("MSExchangeUMAvailability", "Directory Access Failures", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter WorkerProcessRecycled = new ExPerformanceCounter("MSExchangeUMAvailability", "Worker Process Recycled", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter UMSerivceCallsRejected = new ExPerformanceCounter("MSExchangeUMAvailability", "Total Inbound Calls Rejected by the UM Service", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter RecentUMSerivceCallsRejected = new ExPerformanceCounter("MSExchangeUMAvailability", "% of Inbound Calls Rejected by the UM Service Over the Last Hour", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter UMWorkerProcessCallsRejected = new ExPerformanceCounter("MSExchangeUMAvailability", "Total Inbound Calls Rejected by the UM Worker Process", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter RecentUMWorkerProcessCallsRejected = new ExPerformanceCounter("MSExchangeUMAvailability", "% of Inbound Calls Rejected by the UM Worker Process Over the Last Hour", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalWorkerProcessCallCount = new ExPerformanceCounter("MSExchangeUMAvailability", "Total Worker Process Call Count", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter CallsDisconnectedOnIrrecoverableInternalError = new ExPerformanceCounter("MSExchangeUMAvailability", "Calls Disconnected on Irrecoverable Internal Error", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter IncompleteSignalingInformation = new ExPerformanceCounter("MSExchangeUMAvailability", "Incomplete Signaling Information", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter CallsDisconnectedOnIrrecoverableExternalError = new ExPerformanceCounter("MSExchangeUMAvailability", "Calls Disconnected by UM on Irrecoverable External Error", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalQueuedMessages = new ExPerformanceCounter("MSExchangeUMAvailability", "Total Queued Messages", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter SpokenNameAccessed = new ExPerformanceCounter("MSExchangeUMAvailability", "Spoken Name Accessed", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter NameTTSed = new ExPerformanceCounter("MSExchangeUMAvailability", "Name TTSed", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter UMPipelineSLA = new ExPerformanceCounter("MSExchangeUMAvailability", "% of Messages Successfully Processed Over the Last Hour", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter UMPipelineAverageLatency = new ExPerformanceCounter("MSExchangeUMAvailability", "Average Processing Time in seconds per Message (Over the Last 50 Messages)", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter PercentageFailedMailboxAccess = new ExPerformanceCounter("MSExchangeUMAvailability", "Failed Mailbox Connection Attempts %", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter PercentageFailedMailboxAccess_Base = new ExPerformanceCounter("MSExchangeUMAvailability", "Base counter for Failed Mailbox connection attempts %", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter RecentPercentageFailedMailboxAccess = new ExPerformanceCounter("MSExchangeUMAvailability", "% of Failed Mailbox Connection Attempts Over the Last Hour", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter PercentageCustomPromptDownloadFailures = new ExPerformanceCounter("MSExchangeUMAvailability", "Custom Prompt Download Failures %", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter PercentageCustomPromptDownloadFailures_Base = new ExPerformanceCounter("MSExchangeUMAvailability", "Custom Prompt Download Failures % Base", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter PercentageCARDownloadFailures = new ExPerformanceCounter("MSExchangeUMAvailability", "Call Answering Rules Download Failures %", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter PercentageCARDownloadFailures_Base = new ExPerformanceCounter("MSExchangeUMAvailability", "Call Answering Rules Download Failures % Base", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter PercentageTranscriptionFailures = new ExPerformanceCounter("MSExchangeUMAvailability", "Average Voice Message Transcription Failures %", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter PercentageTranscriptionFailures_Base = new ExPerformanceCounter("MSExchangeUMAvailability", "Base Counter for Average Voice Message Transcription Failures %", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter PercentageHubTransportAccessFailures = new ExPerformanceCounter("MSExchangeUMAvailability", "Average Hub Transport Access Failures %", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter PercentageHubTransportAccessFailures_Base = new ExPerformanceCounter("MSExchangeUMAvailability", "Base Counter for Average Hub Transport Access Failures %", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter RecentPercentagePartnerTranscriptionFailures = new ExPerformanceCounter("MSExchangeUMAvailability", "% of Partner Voice Message Transcription Failures Over the Last Hour", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter PercentagePartnerTranscriptionFailures = new ExPerformanceCounter("MSExchangeUMAvailability", "Average Partner Voice Message Transcription Failures %", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter PercentagePartnerTranscriptionFailures_Base = new ExPerformanceCounter("MSExchangeUMAvailability", "Base counter for Average Partner Voice Message Transcription Failures %", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter[] AllCounters = new ExPerformanceCounter[]
		{
			AvailabilityCounters.DirectoryAccessFailures,
			AvailabilityCounters.WorkerProcessRecycled,
			AvailabilityCounters.UMSerivceCallsRejected,
			AvailabilityCounters.RecentUMSerivceCallsRejected,
			AvailabilityCounters.UMWorkerProcessCallsRejected,
			AvailabilityCounters.RecentUMWorkerProcessCallsRejected,
			AvailabilityCounters.TotalWorkerProcessCallCount,
			AvailabilityCounters.CallsDisconnectedOnIrrecoverableInternalError,
			AvailabilityCounters.IncompleteSignalingInformation,
			AvailabilityCounters.CallsDisconnectedOnIrrecoverableExternalError,
			AvailabilityCounters.TotalQueuedMessages,
			AvailabilityCounters.SpokenNameAccessed,
			AvailabilityCounters.NameTTSed,
			AvailabilityCounters.UMPipelineSLA,
			AvailabilityCounters.UMPipelineAverageLatency,
			AvailabilityCounters.PercentageFailedMailboxAccess,
			AvailabilityCounters.PercentageFailedMailboxAccess_Base,
			AvailabilityCounters.RecentPercentageFailedMailboxAccess,
			AvailabilityCounters.PercentageCustomPromptDownloadFailures,
			AvailabilityCounters.PercentageCustomPromptDownloadFailures_Base,
			AvailabilityCounters.PercentageCARDownloadFailures,
			AvailabilityCounters.PercentageCARDownloadFailures_Base,
			AvailabilityCounters.PercentageTranscriptionFailures,
			AvailabilityCounters.PercentageTranscriptionFailures_Base,
			AvailabilityCounters.PercentageHubTransportAccessFailures,
			AvailabilityCounters.PercentageHubTransportAccessFailures_Base,
			AvailabilityCounters.RecentPercentagePartnerTranscriptionFailures,
			AvailabilityCounters.PercentagePartnerTranscriptionFailures,
			AvailabilityCounters.PercentagePartnerTranscriptionFailures_Base
		};
	}
}
