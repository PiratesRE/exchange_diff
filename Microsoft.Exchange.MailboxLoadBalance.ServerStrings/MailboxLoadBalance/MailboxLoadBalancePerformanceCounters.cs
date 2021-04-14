using System;
using System.Xml;
using System.Xml.Linq;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.MailboxLoadBalance
{
	internal static class MailboxLoadBalancePerformanceCounters
	{
		public static void GetPerfCounterInfo(XElement element)
		{
			if (MailboxLoadBalancePerformanceCounters.AllCounters == null)
			{
				return;
			}
			foreach (ExPerformanceCounter exPerformanceCounter in MailboxLoadBalancePerformanceCounters.AllCounters)
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

		public const string CategoryName = "MSExchange Mailbox Load Balancing";

		public static readonly ExPerformanceCounter InjectionRequests = new ExPerformanceCounter("MSExchange Mailbox Load Balancing", "Injection requests", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter CacheEntries = new ExPerformanceCounter("MSExchange Mailbox Load Balancing", "Cache entries", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter[] AllCounters = new ExPerformanceCounter[]
		{
			MailboxLoadBalancePerformanceCounters.InjectionRequests,
			MailboxLoadBalancePerformanceCounters.CacheEntries
		};
	}
}
