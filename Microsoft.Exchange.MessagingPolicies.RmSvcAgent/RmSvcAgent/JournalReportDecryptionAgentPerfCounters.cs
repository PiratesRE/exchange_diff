using System;
using System.Xml;
using System.Xml.Linq;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.MessagingPolicies.RmSvcAgent
{
	internal static class JournalReportDecryptionAgentPerfCounters
	{
		public static void GetPerfCounterInfo(XElement element)
		{
			if (JournalReportDecryptionAgentPerfCounters.AllCounters == null)
			{
				return;
			}
			foreach (ExPerformanceCounter exPerformanceCounter in JournalReportDecryptionAgentPerfCounters.AllCounters)
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

		public const string CategoryName = "MSExchange Journal Report Decryption Agent";

		private static readonly ExPerformanceCounter RateOfJRDecrypted = new ExPerformanceCounter("MSExchange Journal Report Decryption Agent", "Journal Reports decrypted/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalJRDecrypted = new ExPerformanceCounter("MSExchange Journal Report Decryption Agent", "Total Journal Reports decrypted", string.Empty, null, new ExPerformanceCounter[]
		{
			JournalReportDecryptionAgentPerfCounters.RateOfJRDecrypted
		});

		private static readonly ExPerformanceCounter RateOfJRFailed = new ExPerformanceCounter("MSExchange Journal Report Decryption Agent", "Journal Reports failed to decrypt/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalJRFailed = new ExPerformanceCounter("MSExchange Journal Report Decryption Agent", "Total Journal Reports failed to decrypt", string.Empty, null, new ExPerformanceCounter[]
		{
			JournalReportDecryptionAgentPerfCounters.RateOfJRFailed
		});

		private static readonly ExPerformanceCounter RateOfDeferrals = new ExPerformanceCounter("MSExchange Journal Report Decryption Agent", "Deferrals of Journal Reports/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalDeferrals = new ExPerformanceCounter("MSExchange Journal Report Decryption Agent", "Total deferrals of Journal Reports", string.Empty, null, new ExPerformanceCounter[]
		{
			JournalReportDecryptionAgentPerfCounters.RateOfDeferrals
		});

		public static readonly ExPerformanceCounter[] AllCounters = new ExPerformanceCounter[]
		{
			JournalReportDecryptionAgentPerfCounters.TotalJRDecrypted,
			JournalReportDecryptionAgentPerfCounters.TotalJRFailed,
			JournalReportDecryptionAgentPerfCounters.TotalDeferrals
		};
	}
}
