using System;
using System.Xml;
using System.Xml.Linq;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.AddressBook.Service.PerformanceCounters
{
	internal static class AddressBookCounters
	{
		public static void GetPerfCounterInfo(XElement element)
		{
			if (AddressBookCounters.AllCounters == null)
			{
				return;
			}
			foreach (ExPerformanceCounter exPerformanceCounter in AddressBookCounters.AllCounters)
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

		public const string CategoryName = "MSExchangeAB";

		public static readonly ExPerformanceCounter PID = new ExPerformanceCounter("MSExchangeAB", "PID", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter NspiConnectionsCurrent = new ExPerformanceCounter("MSExchangeAB", "NSPI Connections Current", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter NspiConnectionsTotal = new ExPerformanceCounter("MSExchangeAB", "NSPI Connections Total", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter NspiConnectionsRate = new ExPerformanceCounter("MSExchangeAB", "NSPI Connections/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter NspiRpcRequests = new ExPerformanceCounter("MSExchangeAB", "NSPI RPC Requests", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter NspiRpcRequestsTotal = new ExPerformanceCounter("MSExchangeAB", "NSPI RPC Requests Total", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter NspiRpcRequestsAverageLatency = new ExPerformanceCounter("MSExchangeAB", "NSPI RPC Requests Average Latency", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter NspiRpcRequestsRate = new ExPerformanceCounter("MSExchangeAB", "NSPI RPC Requests/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter NspiRpcBrowseRequests = new ExPerformanceCounter("MSExchangeAB", "NSPI RPC Browse Requests", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter NspiRpcBrowseRequestsTotal = new ExPerformanceCounter("MSExchangeAB", "NSPI RPC Browse Requests Total", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter NspiRpcBrowseRequestsAverageLatency = new ExPerformanceCounter("MSExchangeAB", "NSPI RPC Browse Requests Average Latency", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter NspiRpcBrowseRequestsRate = new ExPerformanceCounter("MSExchangeAB", "NSPI RPC Browse Requests/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter RfrRpcRequests = new ExPerformanceCounter("MSExchangeAB", "Referral RPC Requests", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter RfrRpcRequestsTotal = new ExPerformanceCounter("MSExchangeAB", "Referral RPC Requests Total", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter RfrRpcRequestsRate = new ExPerformanceCounter("MSExchangeAB", "Referral RPC Requests/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter RfrRpcRequestsAverageLatency = new ExPerformanceCounter("MSExchangeAB", "Referral RPC Requests Average Latency", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter ThumbnailPhotoAverageTime = new ExPerformanceCounter("MSExchangeAB", "ThumbnailPhoto Average Time", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter ThumbnailPhotoAverageTimeBase = new ExPerformanceCounter("MSExchangeAB", "ThumbnailPhoto Average Time Base", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter ThumbnailPhotoFromMailboxCount = new ExPerformanceCounter("MSExchangeAB", "ThumbnailPhoto From Mailbox Count", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter ThumbnailPhotoFromDirectoryCount = new ExPerformanceCounter("MSExchangeAB", "ThumbnailPhoto From Directory Count", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter ThumbnailPhotoNotPresentCount = new ExPerformanceCounter("MSExchangeAB", "ThumbnailPhoto Not Present Count", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter[] AllCounters = new ExPerformanceCounter[]
		{
			AddressBookCounters.PID,
			AddressBookCounters.NspiConnectionsCurrent,
			AddressBookCounters.NspiConnectionsTotal,
			AddressBookCounters.NspiConnectionsRate,
			AddressBookCounters.NspiRpcRequests,
			AddressBookCounters.NspiRpcRequestsTotal,
			AddressBookCounters.NspiRpcRequestsAverageLatency,
			AddressBookCounters.NspiRpcRequestsRate,
			AddressBookCounters.NspiRpcBrowseRequests,
			AddressBookCounters.NspiRpcBrowseRequestsTotal,
			AddressBookCounters.NspiRpcBrowseRequestsAverageLatency,
			AddressBookCounters.NspiRpcBrowseRequestsRate,
			AddressBookCounters.RfrRpcRequests,
			AddressBookCounters.RfrRpcRequestsTotal,
			AddressBookCounters.RfrRpcRequestsRate,
			AddressBookCounters.RfrRpcRequestsAverageLatency,
			AddressBookCounters.ThumbnailPhotoAverageTime,
			AddressBookCounters.ThumbnailPhotoAverageTimeBase,
			AddressBookCounters.ThumbnailPhotoFromMailboxCount,
			AddressBookCounters.ThumbnailPhotoFromDirectoryCount,
			AddressBookCounters.ThumbnailPhotoNotPresentCount
		};
	}
}
