using System;
using System.Xml;
using System.Xml.Linq;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.InfoWorker.Common.MessageTracking
{
	internal static class InfoWorkerMessageTrackingPerformanceCounters
	{
		public static void GetPerfCounterInfo(XElement element)
		{
			if (InfoWorkerMessageTrackingPerformanceCounters.AllCounters == null)
			{
				return;
			}
			foreach (ExPerformanceCounter exPerformanceCounter in InfoWorkerMessageTrackingPerformanceCounters.AllCounters)
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

		public const string CategoryName = "MSExchange Message Tracking";

		private static readonly ExPerformanceCounter SearchMessageTrackingReportExecutedRate = new ExPerformanceCounter("MSExchange Message Tracking", "Search-MessageTrackingReport task executed/sec", string.Empty, null, new ExPerformanceCounter[0]);

		private static readonly ExPerformanceCounter SearchMessageTrackingReportAverageProcessingTime = new ExPerformanceCounter("MSExchange Message Tracking", "Average Search-MessageTrackingReport Processing Time", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter SearchMessageTrackingReportProcessingTime = new ExPerformanceCounter("MSExchange Message Tracking", "Search-MessageTrackingReport Processing Time", string.Empty, null, new ExPerformanceCounter[]
		{
			InfoWorkerMessageTrackingPerformanceCounters.SearchMessageTrackingReportAverageProcessingTime
		});

		private static readonly ExPerformanceCounter SearchMessageTrackingReportAverageProcessingTimeBase = new ExPerformanceCounter("MSExchange Message Tracking", "Average Search-MessageTrackingReport Processing Time base", string.Empty, null, new ExPerformanceCounter[0]);

		private static readonly ExPerformanceCounter SearchMessageTrackingReportAverageQueries = new ExPerformanceCounter("MSExchange Message Tracking", "Average Queries by Search-MessageTrackingReport", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter SearchMessageTrackingReportQueries = new ExPerformanceCounter("MSExchange Message Tracking", "Total Queries by Search-MessageTrackingReport", string.Empty, null, new ExPerformanceCounter[]
		{
			InfoWorkerMessageTrackingPerformanceCounters.SearchMessageTrackingReportAverageQueries
		});

		private static readonly ExPerformanceCounter SearchMessageTrackingReportAverageQueriesBase = new ExPerformanceCounter("MSExchange Message Tracking", "Average Queries by Search-MessageTrackingReport base", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter SearchMessageTrackingReportExecuted = new ExPerformanceCounter("MSExchange Message Tracking", "Search-MessageTrackingReport task executed", string.Empty, null, new ExPerformanceCounter[]
		{
			InfoWorkerMessageTrackingPerformanceCounters.SearchMessageTrackingReportExecutedRate,
			InfoWorkerMessageTrackingPerformanceCounters.SearchMessageTrackingReportAverageProcessingTimeBase,
			InfoWorkerMessageTrackingPerformanceCounters.SearchMessageTrackingReportAverageQueriesBase
		});

		private static readonly ExPerformanceCounter GetMessageTrackingReportExecutedRate = new ExPerformanceCounter("MSExchange Message Tracking", "Get-MessageTrackingReport Task Executed/Sec", string.Empty, null, new ExPerformanceCounter[0]);

		private static readonly ExPerformanceCounter GetMessageTrackingReportAverageProcessingTime = new ExPerformanceCounter("MSExchange Message Tracking", "Average Get-MessageTrackingReport Processing Time", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter GetMessageTrackingReportProcessingTime = new ExPerformanceCounter("MSExchange Message Tracking", "Get-MessageTrackingReport Processing Time", string.Empty, null, new ExPerformanceCounter[]
		{
			InfoWorkerMessageTrackingPerformanceCounters.GetMessageTrackingReportAverageProcessingTime
		});

		private static readonly ExPerformanceCounter GetMessageTrackingReportAverageProcessingTimeBase = new ExPerformanceCounter("MSExchange Message Tracking", "Average Get-MessageTrackingReport Processing Time Base", string.Empty, null, new ExPerformanceCounter[0]);

		private static readonly ExPerformanceCounter GetMessageTrackingReportAverageQueries = new ExPerformanceCounter("MSExchange Message Tracking", "Average Queries by Get-MessageTrackingReport", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter GetMessageTrackingReportQueries = new ExPerformanceCounter("MSExchange Message Tracking", "Total Queries by Get-MessageTrackingReport", string.Empty, null, new ExPerformanceCounter[]
		{
			InfoWorkerMessageTrackingPerformanceCounters.GetMessageTrackingReportAverageQueries
		});

		private static readonly ExPerformanceCounter GetMessageTrackingReportAverageQueriesBase = new ExPerformanceCounter("MSExchange Message Tracking", "Average Queries by Get-MessageTrackingReport base", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter GetMessageTrackingReportExecuted = new ExPerformanceCounter("MSExchange Message Tracking", "Get-MessageTrackingReport Task Executed", string.Empty, null, new ExPerformanceCounter[]
		{
			InfoWorkerMessageTrackingPerformanceCounters.GetMessageTrackingReportExecutedRate,
			InfoWorkerMessageTrackingPerformanceCounters.GetMessageTrackingReportAverageProcessingTimeBase,
			InfoWorkerMessageTrackingPerformanceCounters.GetMessageTrackingReportAverageQueriesBase
		});

		public static readonly ExPerformanceCounter CurrentRequestDispatcherRequests = new ExPerformanceCounter("MSExchange Message Tracking", "MessageTracking current request dispatcher requests", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter MessageTrackingFailureRateTask = new ExPerformanceCounter("MSExchange Message Tracking", "Percentage of MessageTrackingReport operations (via task) completed with errors", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter MessageTrackingFailureRateWebService = new ExPerformanceCounter("MSExchange Message Tracking", "Percentage of MessageTrackingReport operations (via EWS) completed with errors", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter[] AllCounters = new ExPerformanceCounter[]
		{
			InfoWorkerMessageTrackingPerformanceCounters.SearchMessageTrackingReportExecuted,
			InfoWorkerMessageTrackingPerformanceCounters.SearchMessageTrackingReportProcessingTime,
			InfoWorkerMessageTrackingPerformanceCounters.SearchMessageTrackingReportQueries,
			InfoWorkerMessageTrackingPerformanceCounters.GetMessageTrackingReportExecuted,
			InfoWorkerMessageTrackingPerformanceCounters.GetMessageTrackingReportProcessingTime,
			InfoWorkerMessageTrackingPerformanceCounters.GetMessageTrackingReportQueries,
			InfoWorkerMessageTrackingPerformanceCounters.CurrentRequestDispatcherRequests,
			InfoWorkerMessageTrackingPerformanceCounters.MessageTrackingFailureRateTask,
			InfoWorkerMessageTrackingPerformanceCounters.MessageTrackingFailureRateWebService
		};
	}
}
