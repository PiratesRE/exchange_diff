using System;
using System.Xml;
using System.Xml.Linq;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Services.Core
{
	internal static class WsDatacenterPerformanceCounters
	{
		public static void GetPerfCounterInfo(XElement element)
		{
			if (WsDatacenterPerformanceCounters.AllCounters == null)
			{
				return;
			}
			foreach (ExPerformanceCounter exPerformanceCounter in WsDatacenterPerformanceCounters.AllCounters)
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

		public const string CategoryName = "MSExchangeWS:Datacenter";

		private static readonly ExPerformanceCounter RequestsReceivedWithPartnerTokenPerSecond = new ExPerformanceCounter("MSExchangeWS:Datacenter", "Requests Received with Partner Token/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalRequestsReceivedWithPartnerToken = new ExPerformanceCounter("MSExchangeWS:Datacenter", "Total Requests Received with Partner Token", string.Empty, null, new ExPerformanceCounter[]
		{
			WsDatacenterPerformanceCounters.RequestsReceivedWithPartnerTokenPerSecond
		});

		private static readonly ExPerformanceCounter UnauthorizedRequestsReceivedWithPartnerTokenPerSecond = new ExPerformanceCounter("MSExchangeWS:Datacenter", "Unauthorized Requests Received with Partner Token/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalUnauthorizedRequestsReceivedWithPartnerToken = new ExPerformanceCounter("MSExchangeWS:Datacenter", "Total Unauthorized Requests Received with Partner Token", string.Empty, null, new ExPerformanceCounter[]
		{
			WsDatacenterPerformanceCounters.UnauthorizedRequestsReceivedWithPartnerTokenPerSecond
		});

		public static readonly ExPerformanceCounter PartnerTokenCacheEntries = new ExPerformanceCounter("MSExchangeWS:Datacenter", "Partner Token Cache Entries", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter[] AllCounters = new ExPerformanceCounter[]
		{
			WsDatacenterPerformanceCounters.TotalRequestsReceivedWithPartnerToken,
			WsDatacenterPerformanceCounters.TotalUnauthorizedRequestsReceivedWithPartnerToken,
			WsDatacenterPerformanceCounters.PartnerTokenCacheEntries
		};
	}
}
