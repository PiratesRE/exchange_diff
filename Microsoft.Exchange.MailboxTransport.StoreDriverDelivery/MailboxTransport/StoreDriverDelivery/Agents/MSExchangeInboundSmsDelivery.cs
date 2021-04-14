using System;
using System.Xml;
using System.Xml.Linq;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.MailboxTransport.StoreDriverDelivery.Agents
{
	internal static class MSExchangeInboundSmsDelivery
	{
		public static void GetPerfCounterInfo(XElement element)
		{
			if (MSExchangeInboundSmsDelivery.AllCounters == null)
			{
				return;
			}
			foreach (ExPerformanceCounter exPerformanceCounter in MSExchangeInboundSmsDelivery.AllCounters)
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

		public const string CategoryName = "MSExchange Inbound SMS Delivery Agent";

		public static readonly ExPerformanceCounter MessageReceived = new ExPerformanceCounter("MSExchange Inbound SMS Delivery Agent", "Number of SMS messages received", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter MaximumMessageProcessingTime = new ExPerformanceCounter("MSExchange Inbound SMS Delivery Agent", "Maximum message processing time", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter AverageMessageProcessingTime = new ExPerformanceCounter("MSExchange Inbound SMS Delivery Agent", "Average message processing time", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter[] AllCounters = new ExPerformanceCounter[]
		{
			MSExchangeInboundSmsDelivery.MessageReceived,
			MSExchangeInboundSmsDelivery.MaximumMessageProcessingTime,
			MSExchangeInboundSmsDelivery.AverageMessageProcessingTime
		};
	}
}
