using System;
using System.Xml;
using System.Xml.Linq;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.MessagingPolicies.RmSvcAgent
{
	internal static class E4eAgentPerfCounters
	{
		public static void GetPerfCounterInfo(XElement element)
		{
			if (E4eAgentPerfCounters.AllCounters == null)
			{
				return;
			}
			foreach (ExPerformanceCounter exPerformanceCounter in E4eAgentPerfCounters.AllCounters)
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

		public const string CategoryName = "MSExchange E4E Agent";

		private static readonly ExPerformanceCounter RateOfEncryptionSuccess = new ExPerformanceCounter("MSExchange E4E Agent", "Encryption Success Count Per Second", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter EncryptionSuccessCount = new ExPerformanceCounter("MSExchange E4E Agent", "Encryption Success Count", string.Empty, null, new ExPerformanceCounter[]
		{
			E4eAgentPerfCounters.RateOfEncryptionSuccess
		});

		private static readonly ExPerformanceCounter RateOfAfterEncryptionSuccess = new ExPerformanceCounter("MSExchange E4E Agent", "After Encryption Success Count Per Second", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter AfterEncryptionSuccessCount = new ExPerformanceCounter("MSExchange E4E Agent", "After Encryption Success Count", string.Empty, null, new ExPerformanceCounter[]
		{
			E4eAgentPerfCounters.RateOfAfterEncryptionSuccess
		});

		private static readonly ExPerformanceCounter RateOfReEncryptionSuccess = new ExPerformanceCounter("MSExchange E4E Agent", "Re-Encryption Success Count Per Second", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter ReEncryptionSuccessCount = new ExPerformanceCounter("MSExchange E4E Agent", "Re-Encryption Success Count", string.Empty, null, new ExPerformanceCounter[]
		{
			E4eAgentPerfCounters.RateOfReEncryptionSuccess
		});

		private static readonly ExPerformanceCounter RateOfEncryptionFailure = new ExPerformanceCounter("MSExchange E4E Agent", "Encryption Failure Count Per Second", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter EncryptionFailureCount = new ExPerformanceCounter("MSExchange E4E Agent", "Encryption Failure Count", string.Empty, null, new ExPerformanceCounter[]
		{
			E4eAgentPerfCounters.RateOfEncryptionFailure
		});

		private static readonly ExPerformanceCounter RateOfAfterEncryptionFailure = new ExPerformanceCounter("MSExchange E4E Agent", "After Encryption Failure Count Per Second", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter AfterEncryptionFailureCount = new ExPerformanceCounter("MSExchange E4E Agent", "After Encryption Failure Count", string.Empty, null, new ExPerformanceCounter[]
		{
			E4eAgentPerfCounters.RateOfAfterEncryptionFailure
		});

		private static readonly ExPerformanceCounter RateOfReEncryptionFailure = new ExPerformanceCounter("MSExchange E4E Agent", "Re-Encryption Failure Count Per Second", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter ReEncryptionFailureCount = new ExPerformanceCounter("MSExchange E4E Agent", "Re-Encryption Failure Count", string.Empty, null, new ExPerformanceCounter[]
		{
			E4eAgentPerfCounters.RateOfReEncryptionFailure
		});

		private static readonly ExPerformanceCounter RateOfDecryptionSuccess = new ExPerformanceCounter("MSExchange E4E Agent", "Decryption Success Count Per Second", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter DecryptionSuccessCount = new ExPerformanceCounter("MSExchange E4E Agent", "Decryption Success Count", string.Empty, null, new ExPerformanceCounter[]
		{
			E4eAgentPerfCounters.RateOfDecryptionSuccess
		});

		private static readonly ExPerformanceCounter RateOfDecryptionFailure = new ExPerformanceCounter("MSExchange E4E Agent", "Decryption Failure Count Per Second", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter DecryptionFailureCount = new ExPerformanceCounter("MSExchange E4E Agent", "Decryption Failure Count", string.Empty, null, new ExPerformanceCounter[]
		{
			E4eAgentPerfCounters.RateOfDecryptionFailure
		});

		public static readonly ExPerformanceCounter[] AllCounters = new ExPerformanceCounter[]
		{
			E4eAgentPerfCounters.EncryptionSuccessCount,
			E4eAgentPerfCounters.AfterEncryptionSuccessCount,
			E4eAgentPerfCounters.ReEncryptionSuccessCount,
			E4eAgentPerfCounters.EncryptionFailureCount,
			E4eAgentPerfCounters.AfterEncryptionFailureCount,
			E4eAgentPerfCounters.ReEncryptionFailureCount,
			E4eAgentPerfCounters.DecryptionSuccessCount,
			E4eAgentPerfCounters.DecryptionFailureCount
		};
	}
}
