using System;
using System.Xml;
using System.Xml.Linq;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.MailboxTransport.ContentAggregation
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal static class MSExchangeImapAggregation
	{
		public static void GetPerfCounterInfo(XElement element)
		{
			if (MSExchangeImapAggregation.AllCounters == null)
			{
				return;
			}
			foreach (ExPerformanceCounter exPerformanceCounter in MSExchangeImapAggregation.AllCounters)
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

		public const string CategoryName = "MSExchange Transport Sync - IMAP";

		private static readonly ExPerformanceCounter RateOfBytesDownloaded = new ExPerformanceCounter("MSExchange Transport Sync - IMAP", "Bytes Downloaded/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalBytesDownloaded = new ExPerformanceCounter("MSExchange Transport Sync - IMAP", "Total bytes downloaded", string.Empty, null, new ExPerformanceCounter[]
		{
			MSExchangeImapAggregation.RateOfBytesDownloaded
		});

		private static readonly ExPerformanceCounter RateOfBytesUploaded = new ExPerformanceCounter("MSExchange Transport Sync - IMAP", "Bytes Uploaded/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalBytesUploaded = new ExPerformanceCounter("MSExchange Transport Sync - IMAP", "Total bytes uploaded", string.Empty, null, new ExPerformanceCounter[]
		{
			MSExchangeImapAggregation.RateOfBytesUploaded
		});

		private static readonly ExPerformanceCounter RateOfMessagesDownloaded = new ExPerformanceCounter("MSExchange Transport Sync - IMAP", "Messages Downloaded/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalMessagesDownloaded = new ExPerformanceCounter("MSExchange Transport Sync - IMAP", "Total Messages Downloaded", string.Empty, null, new ExPerformanceCounter[]
		{
			MSExchangeImapAggregation.RateOfMessagesDownloaded
		});

		private static readonly ExPerformanceCounter RateOfMessagesUploaded = new ExPerformanceCounter("MSExchange Transport Sync - IMAP", "Messages Uploaded/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalMessagesUploaded = new ExPerformanceCounter("MSExchange Transport Sync - IMAP", "Total Messages Uploaded", string.Empty, null, new ExPerformanceCounter[]
		{
			MSExchangeImapAggregation.RateOfMessagesUploaded
		});

		public static readonly ExPerformanceCounter[] AllCounters = new ExPerformanceCounter[]
		{
			MSExchangeImapAggregation.TotalBytesDownloaded,
			MSExchangeImapAggregation.TotalBytesUploaded,
			MSExchangeImapAggregation.TotalMessagesDownloaded,
			MSExchangeImapAggregation.TotalMessagesUploaded
		};
	}
}
