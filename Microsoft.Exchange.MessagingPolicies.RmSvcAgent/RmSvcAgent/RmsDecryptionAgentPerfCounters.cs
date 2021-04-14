using System;
using System.Xml;
using System.Xml.Linq;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.MessagingPolicies.RmSvcAgent
{
	internal static class RmsDecryptionAgentPerfCounters
	{
		public static void GetPerfCounterInfo(XElement element)
		{
			if (RmsDecryptionAgentPerfCounters.AllCounters == null)
			{
				return;
			}
			foreach (ExPerformanceCounter exPerformanceCounter in RmsDecryptionAgentPerfCounters.AllCounters)
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

		public const string CategoryName = "MSExchange RMS Decryption Agent";

		public static readonly ExPerformanceCounter MessageDecrypted = new ExPerformanceCounter("MSExchange RMS Decryption Agent", "Message successfully decrypted by RMS Decryption Agent", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter MessageFailedToDecrypt = new ExPerformanceCounter("MSExchange RMS Decryption Agent", "Message failed to be decrypted by RMS Decryption Agent", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter Percentile95FailedToDecrypt = new ExPerformanceCounter("MSExchange RMS Decryption Agent", "Over 5% of messages failed to decrypt in the last 30 minutes", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter[] AllCounters = new ExPerformanceCounter[]
		{
			RmsDecryptionAgentPerfCounters.MessageDecrypted,
			RmsDecryptionAgentPerfCounters.MessageFailedToDecrypt,
			RmsDecryptionAgentPerfCounters.Percentile95FailedToDecrypt
		};
	}
}
