using System;
using System.Xml;
using System.Xml.Linq;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Transport.Agent.ProtocolFilter
{
	internal static class RecipientFilterPerfCounters
	{
		public static void GetPerfCounterInfo(XElement element)
		{
			if (RecipientFilterPerfCounters.AllCounters == null)
			{
				return;
			}
			foreach (ExPerformanceCounter exPerformanceCounter in RecipientFilterPerfCounters.AllCounters)
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

		public const string CategoryName = "MSExchange Recipient Filter Agent";

		private static readonly ExPerformanceCounter RecipientsRejectedByRecipientValidationPerSecond = new ExPerformanceCounter("MSExchange Recipient Filter Agent", "Recipients Rejected by Recipient Validation/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalRecipientsRejectedByRecipientValidation = new ExPerformanceCounter("MSExchange Recipient Filter Agent", "Recipients Rejected by Recipient Validation", string.Empty, null, new ExPerformanceCounter[]
		{
			RecipientFilterPerfCounters.RecipientsRejectedByRecipientValidationPerSecond
		});

		private static readonly ExPerformanceCounter RecipientsRejectedByBlockListPerSecond = new ExPerformanceCounter("MSExchange Recipient Filter Agent", "Recipients Rejected by Block List/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalRecipientsRejectedByBlockList = new ExPerformanceCounter("MSExchange Recipient Filter Agent", "Recipients Rejected by Block List", string.Empty, null, new ExPerformanceCounter[]
		{
			RecipientFilterPerfCounters.RecipientsRejectedByBlockListPerSecond
		});

		public static readonly ExPerformanceCounter[] AllCounters = new ExPerformanceCounter[]
		{
			RecipientFilterPerfCounters.TotalRecipientsRejectedByRecipientValidation,
			RecipientFilterPerfCounters.TotalRecipientsRejectedByBlockList
		};
	}
}
