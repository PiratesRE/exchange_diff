using System;
using System.Xml;
using System.Xml.Linq;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Transport
{
	internal static class DeliveryFailurePerfCounters
	{
		public static void GetPerfCounterInfo(XElement element)
		{
			if (DeliveryFailurePerfCounters.AllCounters == null)
			{
				return;
			}
			foreach (ExPerformanceCounter exPerformanceCounter in DeliveryFailurePerfCounters.AllCounters)
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

		public const string CategoryName = "MSExchangeTransport Delivery Failures";

		public static readonly ExPerformanceCounter Routing_5_4_4 = new ExPerformanceCounter("MSExchangeTransport Delivery Failures", "Routing: Percentage of 5.4.4 Failures", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter Routing_Total = new ExPerformanceCounter("MSExchangeTransport Delivery Failures", "Routing: Total Delivery Failures", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter Resolver_5_1_4 = new ExPerformanceCounter("MSExchangeTransport Delivery Failures", "Resolver: Percentage of 5.1.4 Failures", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter Resolver_5_2_0 = new ExPerformanceCounter("MSExchangeTransport Delivery Failures", "Resolver: Percentage of 5.2.0 Failures", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter Resolver_5_2_4 = new ExPerformanceCounter("MSExchangeTransport Delivery Failures", "Resolver: Percentage of 5.2.4 Failures", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter Resolver_5_4_6 = new ExPerformanceCounter("MSExchangeTransport Delivery Failures", "Resolver: Percentage of 5.4.6 Failures", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter Resolver_Total = new ExPerformanceCounter("MSExchangeTransport Delivery Failures", "Resolver: Total Delivery Failures", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter Delivery_SMTP_5_6_0 = new ExPerformanceCounter("MSExchangeTransport Delivery Failures", "SMTP: Percentage of 5.6.0 Failures", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter Delivery_SMTP_Total = new ExPerformanceCounter("MSExchangeTransport Delivery Failures", "SMTP: Total Delivery Failures", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter Delivery_StoreDriver_5_2_0 = new ExPerformanceCounter("MSExchangeTransport Delivery Failures", "Store Driver: Percentage of 5.2.0 Failures", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter Delivery_StoreDriver_5_6_0 = new ExPerformanceCounter("MSExchangeTransport Delivery Failures", "Store Driver: Percentage of 5.6.0 Failures", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter Delivery_StoreDriver_Total = new ExPerformanceCounter("MSExchangeTransport Delivery Failures", "Store Driver: Total Delivery Failures", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter Delivery_DeliveryAgent_Total = new ExPerformanceCounter("MSExchangeTransport Delivery Failures", "Delivery Agent: Total Delivery Failures", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter Delivery_ForeignConnector_Total = new ExPerformanceCounter("MSExchangeTransport Delivery Failures", "Foreign Connector: Total Delivery Failures", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter[] AllCounters = new ExPerformanceCounter[]
		{
			DeliveryFailurePerfCounters.Routing_5_4_4,
			DeliveryFailurePerfCounters.Routing_Total,
			DeliveryFailurePerfCounters.Resolver_5_1_4,
			DeliveryFailurePerfCounters.Resolver_5_2_0,
			DeliveryFailurePerfCounters.Resolver_5_2_4,
			DeliveryFailurePerfCounters.Resolver_5_4_6,
			DeliveryFailurePerfCounters.Resolver_Total,
			DeliveryFailurePerfCounters.Delivery_SMTP_5_6_0,
			DeliveryFailurePerfCounters.Delivery_SMTP_Total,
			DeliveryFailurePerfCounters.Delivery_StoreDriver_5_2_0,
			DeliveryFailurePerfCounters.Delivery_StoreDriver_5_6_0,
			DeliveryFailurePerfCounters.Delivery_StoreDriver_Total,
			DeliveryFailurePerfCounters.Delivery_DeliveryAgent_Total,
			DeliveryFailurePerfCounters.Delivery_ForeignConnector_Total
		};
	}
}
