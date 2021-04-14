using System;
using System.Xml;
using System.Xml.Linq;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.MailboxTransport.StoreDriverDelivery.Agents
{
	internal static class MeetingSeriesMessageOrdering
	{
		public static void GetPerfCounterInfo(XElement element)
		{
			if (MeetingSeriesMessageOrdering.AllCounters == null)
			{
				return;
			}
			foreach (ExPerformanceCounter exPerformanceCounter in MeetingSeriesMessageOrdering.AllCounters)
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

		public const string CategoryName = "MSExchange Meeting Series Message Ordering";

		public static readonly ExPerformanceCounter MeetingMessages = new ExPerformanceCounter("MSExchange Meeting Series Message Ordering", "Number of Meeting Messages", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter SeriesMeetingInstanceMessages = new ExPerformanceCounter("MSExchange Meeting Series Message Ordering", "Number of Series Meeting Instance Messages", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter ParkedSeriesMeetingInstanceMessages = new ExPerformanceCounter("MSExchange Meeting Series Message Ordering", "Number of Parked Series Meeting Instance Messages", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter[] AllCounters = new ExPerformanceCounter[]
		{
			MeetingSeriesMessageOrdering.MeetingMessages,
			MeetingSeriesMessageOrdering.SeriesMeetingInstanceMessages,
			MeetingSeriesMessageOrdering.ParkedSeriesMeetingInstanceMessages
		};
	}
}
