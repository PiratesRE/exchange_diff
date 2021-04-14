using System;
using System.Xml;
using System.Xml.Linq;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.InfoWorker.Common.Sharing
{
	internal static class PerformanceCounters
	{
		public static void GetPerfCounterInfo(XElement element)
		{
			if (PerformanceCounters.AllCounters == null)
			{
				return;
			}
			foreach (ExPerformanceCounter exPerformanceCounter in PerformanceCounters.AllCounters)
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

		public const string CategoryName = "MSExchange Sharing Engine";

		public static readonly ExPerformanceCounter CalendarItemsSynced = new ExPerformanceCounter("MSExchange Sharing Engine", "Calendar Items Synchronized", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter ContactItemsSynced = new ExPerformanceCounter("MSExchange Sharing Engine", "Contact Items Synchronized", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter FolderSynchronizationFailures = new ExPerformanceCounter("MSExchange Sharing Engine", "Folder Synchronization Failures", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter FoldersProcessedSynchronously = new ExPerformanceCounter("MSExchange Sharing Engine", "Folders Processed Synchronously", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter SynchronizationTimeouts = new ExPerformanceCounter("MSExchange Sharing Engine", "Folders Synchronization Timeouts", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter LastFolderSynchronizationTime = new ExPerformanceCounter("MSExchange Sharing Engine", "Last Folder Synchronization Time (in seconds)", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter AverageFolderSynchronizationTime = new ExPerformanceCounter("MSExchange Sharing Engine", "Average Folder Synchronization Time (in seconds)", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter AverageFolderSynchronizationTimeBase = new ExPerformanceCounter("MSExchange Sharing Engine", "Base for Average Time to Synchronize a Folder", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter AverageExternalAuthenticationTokenRequestTime = new ExPerformanceCounter("MSExchange Sharing Engine", "Average Time to Request a Token for an External Authentication", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter AverageExternalAuthenticationTokenRequestTimeBase = new ExPerformanceCounter("MSExchange Sharing Engine", "Base for Average Time to Request a Token for an External Authentication", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter SuccessfulExternalAuthenticationTokenRequests = new ExPerformanceCounter("MSExchange Sharing Engine", "Number of Successful Token Requests for External Authentication", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter FailedExternalAuthenticationTokenRequests = new ExPerformanceCounter("MSExchange Sharing Engine", "Number of Failed Token Requests for External Authentication", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter[] AllCounters = new ExPerformanceCounter[]
		{
			PerformanceCounters.CalendarItemsSynced,
			PerformanceCounters.ContactItemsSynced,
			PerformanceCounters.FolderSynchronizationFailures,
			PerformanceCounters.FoldersProcessedSynchronously,
			PerformanceCounters.SynchronizationTimeouts,
			PerformanceCounters.LastFolderSynchronizationTime,
			PerformanceCounters.AverageFolderSynchronizationTime,
			PerformanceCounters.AverageFolderSynchronizationTimeBase,
			PerformanceCounters.AverageExternalAuthenticationTokenRequestTime,
			PerformanceCounters.AverageExternalAuthenticationTokenRequestTimeBase,
			PerformanceCounters.SuccessfulExternalAuthenticationTokenRequests,
			PerformanceCounters.FailedExternalAuthenticationTokenRequests
		};
	}
}
