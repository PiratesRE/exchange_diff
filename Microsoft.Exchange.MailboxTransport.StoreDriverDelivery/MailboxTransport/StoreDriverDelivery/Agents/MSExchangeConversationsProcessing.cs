using System;
using System.Xml;
using System.Xml.Linq;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.MailboxTransport.StoreDriverDelivery.Agents
{
	internal static class MSExchangeConversationsProcessing
	{
		public static void GetPerfCounterInfo(XElement element)
		{
			if (MSExchangeConversationsProcessing.AllCounters == null)
			{
				return;
			}
			foreach (ExPerformanceCounter exPerformanceCounter in MSExchangeConversationsProcessing.AllCounters)
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

		public const string CategoryName = "MSExchange Conversations Transport Agent";

		public static readonly ExPerformanceCounter AverageMessageProcessingTime = new ExPerformanceCounter("MSExchange Conversations Transport Agent", "Average message processing time", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter LastMessageProcessingTime = new ExPerformanceCounter("MSExchange Conversations Transport Agent", "Last message processing time", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter[] AllCounters = new ExPerformanceCounter[]
		{
			MSExchangeConversationsProcessing.AverageMessageProcessingTime,
			MSExchangeConversationsProcessing.LastMessageProcessingTime
		};
	}
}
