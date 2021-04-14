using System;
using System.Xml;
using System.Xml.Linq;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.MapiHttp.PerformanceCounters
{
	internal static class NspiPerformanceCounters
	{
		public static void GetPerfCounterInfo(XElement element)
		{
			if (NspiPerformanceCounters.AllCounters == null)
			{
				return;
			}
			foreach (ExPerformanceCounter exPerformanceCounter in NspiPerformanceCounters.AllCounters)
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

		public const string CategoryName = "MSExchange MapiHttp Nspi";

		public static readonly ExPerformanceCounter PID = new ExPerformanceCounter("MSExchange MapiHttp Nspi", "PID", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter NspiConnectionsCurrent = new ExPerformanceCounter("MSExchange MapiHttp Nspi", "NSPI Connections Current", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter NspiConnectionsTotal = new ExPerformanceCounter("MSExchange MapiHttp Nspi", "NSPI Connections Total", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter NspiConnectionsRate = new ExPerformanceCounter("MSExchange MapiHttp Nspi", "NSPI Connections/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter NspiRequests = new ExPerformanceCounter("MSExchange MapiHttp Nspi", "NSPI Requests", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter NspiRequestsTotal = new ExPerformanceCounter("MSExchange MapiHttp Nspi", "NSPI Requests Total", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter NspiRequestsAverageLatency = new ExPerformanceCounter("MSExchange MapiHttp Nspi", "NSPI Requests Average Latency", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter NspiRequestsRate = new ExPerformanceCounter("MSExchange MapiHttp Nspi", "NSPI Requests/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter NspiBrowseRequests = new ExPerformanceCounter("MSExchange MapiHttp Nspi", "NSPI Browse Requests", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter NspiBrowseRequestsTotal = new ExPerformanceCounter("MSExchange MapiHttp Nspi", "NSPI Browse Requests Total", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter NspiBrowseRequestsAverageLatency = new ExPerformanceCounter("MSExchange MapiHttp Nspi", "NSPI Browse Requests Average Latency", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter NspiBrowseRequestsRate = new ExPerformanceCounter("MSExchange MapiHttp Nspi", "NSPI Browse Requests/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter RfrRequests = new ExPerformanceCounter("MSExchange MapiHttp Nspi", "Referral Requests", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter RfrRequestsTotal = new ExPerformanceCounter("MSExchange MapiHttp Nspi", "Referral Requests Total", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter RfrRequestsRate = new ExPerformanceCounter("MSExchange MapiHttp Nspi", "Referral Requests/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter RfrRequestsAverageLatency = new ExPerformanceCounter("MSExchange MapiHttp Nspi", "Referral Requests Average Latency", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter ThumbnailPhotoAverageTime = new ExPerformanceCounter("MSExchange MapiHttp Nspi", "ThumbnailPhoto Average Time", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter ThumbnailPhotoAverageTimeBase = new ExPerformanceCounter("MSExchange MapiHttp Nspi", "ThumbnailPhoto Average Time Base", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter ThumbnailPhotoFromMailboxCount = new ExPerformanceCounter("MSExchange MapiHttp Nspi", "ThumbnailPhoto From Mailbox Count", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter ThumbnailPhotoFromDirectoryCount = new ExPerformanceCounter("MSExchange MapiHttp Nspi", "ThumbnailPhoto From Directory Count", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter ThumbnailPhotoNotPresentCount = new ExPerformanceCounter("MSExchange MapiHttp Nspi", "ThumbnailPhoto Not Present Count", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter[] AllCounters = new ExPerformanceCounter[]
		{
			NspiPerformanceCounters.PID,
			NspiPerformanceCounters.NspiConnectionsCurrent,
			NspiPerformanceCounters.NspiConnectionsTotal,
			NspiPerformanceCounters.NspiConnectionsRate,
			NspiPerformanceCounters.NspiRequests,
			NspiPerformanceCounters.NspiRequestsTotal,
			NspiPerformanceCounters.NspiRequestsAverageLatency,
			NspiPerformanceCounters.NspiRequestsRate,
			NspiPerformanceCounters.NspiBrowseRequests,
			NspiPerformanceCounters.NspiBrowseRequestsTotal,
			NspiPerformanceCounters.NspiBrowseRequestsAverageLatency,
			NspiPerformanceCounters.NspiBrowseRequestsRate,
			NspiPerformanceCounters.RfrRequests,
			NspiPerformanceCounters.RfrRequestsTotal,
			NspiPerformanceCounters.RfrRequestsRate,
			NspiPerformanceCounters.RfrRequestsAverageLatency,
			NspiPerformanceCounters.ThumbnailPhotoAverageTime,
			NspiPerformanceCounters.ThumbnailPhotoAverageTimeBase,
			NspiPerformanceCounters.ThumbnailPhotoFromMailboxCount,
			NspiPerformanceCounters.ThumbnailPhotoFromDirectoryCount,
			NspiPerformanceCounters.ThumbnailPhotoNotPresentCount
		};
	}
}
