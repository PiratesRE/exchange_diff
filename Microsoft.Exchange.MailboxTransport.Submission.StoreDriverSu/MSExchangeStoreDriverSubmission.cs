using System;
using System.Xml;
using System.Xml.Linq;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.MailboxTransport.Submission.StoreDriverSubmission
{
	internal static class MSExchangeStoreDriverSubmission
	{
		public static void GetPerfCounterInfo(XElement element)
		{
			if (MSExchangeStoreDriverSubmission.AllCounters == null)
			{
				return;
			}
			foreach (ExPerformanceCounter exPerformanceCounter in MSExchangeStoreDriverSubmission.AllCounters)
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

		public const string CategoryName = "MSExchange Submission Store Driver";

		private static readonly ExPerformanceCounter SubmittedMailItemsPerSecond = new ExPerformanceCounter("MSExchange Submission Store Driver", "Outbound: Submitted Mail Items Per Second", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter SubmittedMailItems = new ExPerformanceCounter("MSExchange Submission Store Driver", "Outbound: Submitted Mail Items", string.Empty, null, new ExPerformanceCounter[]
		{
			MSExchangeStoreDriverSubmission.SubmittedMailItemsPerSecond
		});

		public static readonly ExPerformanceCounter TotalRecipients = new ExPerformanceCounter("MSExchange Submission Store Driver", "Outbound: TotalRecipients", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter[] AllCounters = new ExPerformanceCounter[]
		{
			MSExchangeStoreDriverSubmission.SubmittedMailItems,
			MSExchangeStoreDriverSubmission.TotalRecipients
		};
	}
}
