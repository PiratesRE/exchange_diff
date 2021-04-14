using System;
using System.Xml;
using System.Xml.Linq;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.MailboxTransport.ContentAggregation
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal static class MSExchangeDeltaSyncAggregation
	{
		public static void GetPerfCounterInfo(XElement element)
		{
			if (MSExchangeDeltaSyncAggregation.AllCounters == null)
			{
				return;
			}
			foreach (ExPerformanceCounter exPerformanceCounter in MSExchangeDeltaSyncAggregation.AllCounters)
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

		public const string CategoryName = "MSExchange Transport Sync - Hotmail";

		private static readonly ExPerformanceCounter RateOfBytesDownloaded = new ExPerformanceCounter("MSExchange Transport Sync - Hotmail", "Bytes Downloaded/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalBytesDownloaded = new ExPerformanceCounter("MSExchange Transport Sync - Hotmail", "Total bytes downloaded", string.Empty, null, new ExPerformanceCounter[]
		{
			MSExchangeDeltaSyncAggregation.RateOfBytesDownloaded
		});

		private static readonly ExPerformanceCounter RateOfBytesUploaded = new ExPerformanceCounter("MSExchange Transport Sync - Hotmail", "Bytes Uploaded/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalBytesUploaded = new ExPerformanceCounter("MSExchange Transport Sync - Hotmail", "Total bytes uploaded", string.Empty, null, new ExPerformanceCounter[]
		{
			MSExchangeDeltaSyncAggregation.RateOfBytesUploaded
		});

		private static readonly ExPerformanceCounter RateOfMessagesDownloaded = new ExPerformanceCounter("MSExchange Transport Sync - Hotmail", "Messages Downloaded/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalMessagesDownloaded = new ExPerformanceCounter("MSExchange Transport Sync - Hotmail", "Total Messages Downloaded", string.Empty, null, new ExPerformanceCounter[]
		{
			MSExchangeDeltaSyncAggregation.RateOfMessagesDownloaded
		});

		private static readonly ExPerformanceCounter RateOfMessagesUploaded = new ExPerformanceCounter("MSExchange Transport Sync - Hotmail", "Messages Uploaded/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalMessagesUploaded = new ExPerformanceCounter("MSExchange Transport Sync - Hotmail", "Total Messages Uploaded", string.Empty, null, new ExPerformanceCounter[]
		{
			MSExchangeDeltaSyncAggregation.RateOfMessagesUploaded
		});

		public static readonly ExPerformanceCounter[] AllCounters = new ExPerformanceCounter[]
		{
			MSExchangeDeltaSyncAggregation.TotalBytesDownloaded,
			MSExchangeDeltaSyncAggregation.TotalBytesUploaded,
			MSExchangeDeltaSyncAggregation.TotalMessagesDownloaded,
			MSExchangeDeltaSyncAggregation.TotalMessagesUploaded
		};
	}
}
