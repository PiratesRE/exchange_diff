using System;
using System.Xml;
using System.Xml.Linq;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.MessagingPolicies.RmSvcAgent
{
	internal static class ApaAgentPerfCounters
	{
		public static void GetPerfCounterInfo(XElement element)
		{
			if (ApaAgentPerfCounters.AllCounters == null)
			{
				return;
			}
			foreach (ExPerformanceCounter exPerformanceCounter in ApaAgentPerfCounters.AllCounters)
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

		public const string CategoryName = "MSExchange Encryption Agent";

		private static readonly ExPerformanceCounter RateOfMessagesEncrypted = new ExPerformanceCounter("MSExchange Encryption Agent", "Messages encrypted for policy/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalMessagesEncrypted = new ExPerformanceCounter("MSExchange Encryption Agent", "Total messages encrypted for policy", string.Empty, null, new ExPerformanceCounter[]
		{
			ApaAgentPerfCounters.RateOfMessagesEncrypted
		});

		private static readonly ExPerformanceCounter RateOfMessagesFailed = new ExPerformanceCounter("MSExchange Encryption Agent", "Messages failed to encrypt for policy/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalMessagesFailed = new ExPerformanceCounter("MSExchange Encryption Agent", "Total messages failed to encrypt for policy", string.Empty, null, new ExPerformanceCounter[]
		{
			ApaAgentPerfCounters.RateOfMessagesFailed
		});

		private static readonly ExPerformanceCounter RateOfDeferrals = new ExPerformanceCounter("MSExchange Encryption Agent", "Deferrals of messages for policy/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalDeferrals = new ExPerformanceCounter("MSExchange Encryption Agent", "Total deferrals of messages for policy", string.Empty, null, new ExPerformanceCounter[]
		{
			ApaAgentPerfCounters.RateOfDeferrals
		});

		public static readonly ExPerformanceCounter Percentile95FailedToEncrypt = new ExPerformanceCounter("MSExchange Encryption Agent", "Over 5% of messages failing Transport Policy Encryption in the last 30 minutes", string.Empty, null, new ExPerformanceCounter[0]);

		private static readonly ExPerformanceCounter RateOfMessagesReencrypted = new ExPerformanceCounter("MSExchange Encryption Agent", "Messages reencrypted/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalMessagesReencrypted = new ExPerformanceCounter("MSExchange Encryption Agent", "Total messages reencrypted", string.Empty, null, new ExPerformanceCounter[]
		{
			ApaAgentPerfCounters.RateOfMessagesReencrypted
		});

		private static readonly ExPerformanceCounter RateOfMessagesFailedToReencrypt = new ExPerformanceCounter("MSExchange Encryption Agent", "Messages failed to reencrypt/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalMessagesFailedToReencrypt = new ExPerformanceCounter("MSExchange Encryption Agent", "Total messages failed to reencrypt", string.Empty, null, new ExPerformanceCounter[]
		{
			ApaAgentPerfCounters.RateOfMessagesFailedToReencrypt
		});

		public static readonly ExPerformanceCounter[] AllCounters = new ExPerformanceCounter[]
		{
			ApaAgentPerfCounters.TotalMessagesEncrypted,
			ApaAgentPerfCounters.TotalMessagesFailed,
			ApaAgentPerfCounters.TotalDeferrals,
			ApaAgentPerfCounters.Percentile95FailedToEncrypt,
			ApaAgentPerfCounters.TotalMessagesReencrypted,
			ApaAgentPerfCounters.TotalMessagesFailedToReencrypt
		};
	}
}
