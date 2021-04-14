using System;
using System.Xml;
using System.Xml.Linq;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.TextMessaging
{
	internal static class ExSmsCounters
	{
		public static void GetPerfCounterInfo(XElement element)
		{
			if (ExSmsCounters.AllCounters == null)
			{
				return;
			}
			foreach (ExPerformanceCounter exPerformanceCounter in ExSmsCounters.AllCounters)
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

		public const string CategoryName = "MSExchange Text Messaging";

		private static readonly ExPerformanceCounter RateOfTextMessagesSent = new ExPerformanceCounter("MSExchange Text Messaging", "Text Messages Sent/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter NumberOfTextMessagesSent = new ExPerformanceCounter("MSExchange Text Messaging", "Total Text Messages Sent", string.Empty, null, new ExPerformanceCounter[]
		{
			ExSmsCounters.RateOfTextMessagesSent
		});

		private static readonly ExPerformanceCounter RateOfTextMessagesSentViaEas = new ExPerformanceCounter("MSExchange Text Messaging", "Text Messages Sent via Exchange ActiveSync/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter NumberOfTextMessagesSentViaEas = new ExPerformanceCounter("MSExchange Text Messaging", "Total Text Messages Sent via Exchange ActiveSync", string.Empty, null, new ExPerformanceCounter[]
		{
			ExSmsCounters.RateOfTextMessagesSentViaEas
		});

		private static readonly ExPerformanceCounter RateOfTextMessagesSentViaSmtp = new ExPerformanceCounter("MSExchange Text Messaging", "Text messages Sent via SMTP/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter NumberOfTextMessagesSentViaSmtp = new ExPerformanceCounter("MSExchange Text Messaging", "Total Messages Sent via SMTP", string.Empty, null, new ExPerformanceCounter[]
		{
			ExSmsCounters.RateOfTextMessagesSentViaSmtp
		});

		public static readonly ExPerformanceCounter AverageDeliveryLatency = new ExPerformanceCounter("MSExchange Text Messaging", "Average Text Message Delivery Latency (milliseconds)", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter PendingDelivery = new ExPerformanceCounter("MSExchange Text Messaging", "Text Messages Pending Delivery", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter[] AllCounters = new ExPerformanceCounter[]
		{
			ExSmsCounters.NumberOfTextMessagesSent,
			ExSmsCounters.NumberOfTextMessagesSentViaEas,
			ExSmsCounters.NumberOfTextMessagesSentViaSmtp,
			ExSmsCounters.AverageDeliveryLatency,
			ExSmsCounters.PendingDelivery
		};
	}
}
