using System;
using System.Xml;
using System.Xml.Linq;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Transport.Agent.ProtocolFilter
{
	internal static class SenderFilterPerfCounters
	{
		public static void GetPerfCounterInfo(XElement element)
		{
			if (SenderFilterPerfCounters.AllCounters == null)
			{
				return;
			}
			foreach (ExPerformanceCounter exPerformanceCounter in SenderFilterPerfCounters.AllCounters)
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

		public const string CategoryName = "MSExchange Sender Filter Agent";

		private static readonly ExPerformanceCounter MessagesEvaluatedBySenderFilterPerSecond = new ExPerformanceCounter("MSExchange Sender Filter Agent", "Messages Evaluated by Sender Filter/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalMessagesEvaluatedBySenderFilter = new ExPerformanceCounter("MSExchange Sender Filter Agent", "Messages Evaluated by Sender Filter", string.Empty, null, new ExPerformanceCounter[]
		{
			SenderFilterPerfCounters.MessagesEvaluatedBySenderFilterPerSecond
		});

		private static readonly ExPerformanceCounter MessagesFilteredBySenderFilterPerSecond = new ExPerformanceCounter("MSExchange Sender Filter Agent", "Messages Filtered by Sender Filter/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalMessagesFilteredBySenderFilter = new ExPerformanceCounter("MSExchange Sender Filter Agent", "Messages Filtered by Sender Filter", string.Empty, null, new ExPerformanceCounter[]
		{
			SenderFilterPerfCounters.MessagesFilteredBySenderFilterPerSecond
		});

		public static readonly ExPerformanceCounter TotalPerRecipientSenderBlocks = new ExPerformanceCounter("MSExchange Sender Filter Agent", "Senders Blocked Due to Per-Recipient Blocked Senders", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter[] AllCounters = new ExPerformanceCounter[]
		{
			SenderFilterPerfCounters.TotalMessagesEvaluatedBySenderFilter,
			SenderFilterPerfCounters.TotalMessagesFilteredBySenderFilter,
			SenderFilterPerfCounters.TotalPerRecipientSenderBlocks
		};
	}
}
