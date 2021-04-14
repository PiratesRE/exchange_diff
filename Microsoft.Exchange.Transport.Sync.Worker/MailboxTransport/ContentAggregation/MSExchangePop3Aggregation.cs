using System;
using System.Xml;
using System.Xml.Linq;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.MailboxTransport.ContentAggregation
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal static class MSExchangePop3Aggregation
	{
		public static void GetPerfCounterInfo(XElement element)
		{
			if (MSExchangePop3Aggregation.AllCounters == null)
			{
				return;
			}
			foreach (ExPerformanceCounter exPerformanceCounter in MSExchangePop3Aggregation.AllCounters)
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

		public const string CategoryName = "MSExchange Transport Sync - Pop";

		private static readonly ExPerformanceCounter RateOfMessageBytesReceived = new ExPerformanceCounter("MSExchange Transport Sync - Pop", "Message Bytes Received/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter MessageBytesReceivedTotal = new ExPerformanceCounter("MSExchange Transport Sync - Pop", "Message Bytes Received Total", string.Empty, null, new ExPerformanceCounter[]
		{
			MSExchangePop3Aggregation.RateOfMessageBytesReceived
		});

		private static readonly ExPerformanceCounter RateOfMessagesReceived = new ExPerformanceCounter("MSExchange Transport Sync - Pop", "Messages Received/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter MessagesReceivedTotal = new ExPerformanceCounter("MSExchange Transport Sync - Pop", "Messages Received Total", string.Empty, null, new ExPerformanceCounter[]
		{
			MSExchangePop3Aggregation.RateOfMessagesReceived
		});

		public static readonly ExPerformanceCounter[] AllCounters = new ExPerformanceCounter[]
		{
			MSExchangePop3Aggregation.MessageBytesReceivedTotal,
			MSExchangePop3Aggregation.MessagesReceivedTotal
		};
	}
}
