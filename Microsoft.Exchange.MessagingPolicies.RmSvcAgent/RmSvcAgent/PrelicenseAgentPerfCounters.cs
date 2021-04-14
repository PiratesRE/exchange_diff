using System;
using System.Xml;
using System.Xml.Linq;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.MessagingPolicies.RmSvcAgent
{
	internal static class PrelicenseAgentPerfCounters
	{
		public static void GetPerfCounterInfo(XElement element)
		{
			if (PrelicenseAgentPerfCounters.AllCounters == null)
			{
				return;
			}
			foreach (ExPerformanceCounter exPerformanceCounter in PrelicenseAgentPerfCounters.AllCounters)
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

		public const string CategoryName = "MSExchange Prelicensing Agent";

		private static readonly ExPerformanceCounter RateOfMessagesPreLicensed = new ExPerformanceCounter("MSExchange Prelicensing Agent", "Messages Prelicensed/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalMessagesPreLicensed = new ExPerformanceCounter("MSExchange Prelicensing Agent", "Total Messages Prelicensed", string.Empty, null, new ExPerformanceCounter[]
		{
			PrelicenseAgentPerfCounters.RateOfMessagesPreLicensed
		});

		private static readonly ExPerformanceCounter RateOfMessagesFailedToPreLicense = new ExPerformanceCounter("MSExchange Prelicensing Agent", "Messages Failed To Prelicense/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalMessagesFailedToPreLicense = new ExPerformanceCounter("MSExchange Prelicensing Agent", "Total Messages Failed To Prelicense", string.Empty, null, new ExPerformanceCounter[]
		{
			PrelicenseAgentPerfCounters.RateOfMessagesFailedToPreLicense
		});

		private static readonly ExPerformanceCounter RateOfDeferralsToPreLicense = new ExPerformanceCounter("MSExchange Prelicensing Agent", "Rate of Deferrals to Prelicense/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalDeferralsToPreLicense = new ExPerformanceCounter("MSExchange Prelicensing Agent", "Total Number of Deferrals when Prelicensing Messages", string.Empty, null, new ExPerformanceCounter[]
		{
			PrelicenseAgentPerfCounters.RateOfDeferralsToPreLicense
		});

		private static readonly ExPerformanceCounter RateOfRecipientsPreLicensed = new ExPerformanceCounter("MSExchange Prelicensing Agent", "Recipients Prelicensed/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalRecipientsPreLicensed = new ExPerformanceCounter("MSExchange Prelicensing Agent", "Total Recipients Prelicensed", string.Empty, null, new ExPerformanceCounter[]
		{
			PrelicenseAgentPerfCounters.RateOfRecipientsPreLicensed
		});

		private static readonly ExPerformanceCounter RateOfRecipientsFailedToPreLicense = new ExPerformanceCounter("MSExchange Prelicensing Agent", "Recipients Failed To Prelicense/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalRecipientsFailedToPreLicense = new ExPerformanceCounter("MSExchange Prelicensing Agent", "Total Recipients Failed To Prelicense", string.Empty, null, new ExPerformanceCounter[]
		{
			PrelicenseAgentPerfCounters.RateOfRecipientsFailedToPreLicense
		});

		public static readonly ExPerformanceCounter Percentile95FailedToLicense = new ExPerformanceCounter("MSExchange Prelicensing Agent", "Over 5% of messages failed prelicensing or server licensing in last 30 minutes", string.Empty, null, new ExPerformanceCounter[0]);

		private static readonly ExPerformanceCounter RateOfMessagesLicensed = new ExPerformanceCounter("MSExchange Prelicensing Agent", "Messages Licensed/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalMessagesLicensed = new ExPerformanceCounter("MSExchange Prelicensing Agent", "Total Messages Licensed", string.Empty, null, new ExPerformanceCounter[]
		{
			PrelicenseAgentPerfCounters.RateOfMessagesLicensed
		});

		private static readonly ExPerformanceCounter RateOfMessagesFailedToLicense = new ExPerformanceCounter("MSExchange Prelicensing Agent", "Messages Failed To License/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalMessagesFailedToLicense = new ExPerformanceCounter("MSExchange Prelicensing Agent", "Total Messages Failed to License", string.Empty, null, new ExPerformanceCounter[]
		{
			PrelicenseAgentPerfCounters.RateOfMessagesFailedToLicense
		});

		private static readonly ExPerformanceCounter RateOfDeferralsToLicense = new ExPerformanceCounter("MSExchange Prelicensing Agent", "Deferrals Of Messages To License/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalDeferralsToLicense = new ExPerformanceCounter("MSExchange Prelicensing Agent", "Total Deferrals Of Messages To License", string.Empty, null, new ExPerformanceCounter[]
		{
			PrelicenseAgentPerfCounters.RateOfDeferralsToLicense
		});

		private static readonly ExPerformanceCounter RateOfExternalMessagesPreLicensed = new ExPerformanceCounter("MSExchange Prelicensing Agent", "External IRM-Protected Messages Prelicensed/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalExternalMessagesPreLicensed = new ExPerformanceCounter("MSExchange Prelicensing Agent", "Total External Messages prelicensed", string.Empty, null, new ExPerformanceCounter[]
		{
			PrelicenseAgentPerfCounters.RateOfExternalMessagesPreLicensed
		});

		private static readonly ExPerformanceCounter RateOfExternalMessagesFailedToPreLicense = new ExPerformanceCounter("MSExchange Prelicensing Agent", "External Messages Failed To Prelicense/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalExternalMessagesFailedToPreLicense = new ExPerformanceCounter("MSExchange Prelicensing Agent", "Total External Messages Failed To Prelicense", string.Empty, null, new ExPerformanceCounter[]
		{
			PrelicenseAgentPerfCounters.RateOfExternalMessagesFailedToPreLicense
		});

		private static readonly ExPerformanceCounter RateOfDeferralsToPreLicenseForExternalMessages = new ExPerformanceCounter("MSExchange Prelicensing Agent", "Rate of Deferrals to Prelicense External Messages/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalDeferralsToPreLicenseForExternalMessages = new ExPerformanceCounter("MSExchange Prelicensing Agent", "Total Deferrals To Prelicense For External Messages", string.Empty, null, new ExPerformanceCounter[]
		{
			PrelicenseAgentPerfCounters.RateOfDeferralsToPreLicenseForExternalMessages
		});

		private static readonly ExPerformanceCounter RateOfRecipientsPreLicensedForExternalMessages = new ExPerformanceCounter("MSExchange Prelicensing Agent", "Recipients Prelicensed For External Messages/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalRecipientsPreLicensedForExternalMessages = new ExPerformanceCounter("MSExchange Prelicensing Agent", "Total Recipients Prelicensed for Messages Protected by External AD RMS Server", string.Empty, null, new ExPerformanceCounter[]
		{
			PrelicenseAgentPerfCounters.RateOfRecipientsPreLicensedForExternalMessages
		});

		private static readonly ExPerformanceCounter RateOfRecipientsFailedToPreLicenseForExternalMessages = new ExPerformanceCounter("MSExchange Prelicensing Agent", "Total Recipients Failed To Prelicense/sec For External Messages", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalRecipientsFailedToPreLicenseForExternalMessages = new ExPerformanceCounter("MSExchange Prelicensing Agent", "Total Recipients Failed To Prelicense For External Messages", string.Empty, null, new ExPerformanceCounter[]
		{
			PrelicenseAgentPerfCounters.RateOfRecipientsFailedToPreLicenseForExternalMessages
		});

		public static readonly ExPerformanceCounter[] AllCounters = new ExPerformanceCounter[]
		{
			PrelicenseAgentPerfCounters.TotalMessagesPreLicensed,
			PrelicenseAgentPerfCounters.TotalMessagesFailedToPreLicense,
			PrelicenseAgentPerfCounters.TotalDeferralsToPreLicense,
			PrelicenseAgentPerfCounters.TotalRecipientsPreLicensed,
			PrelicenseAgentPerfCounters.TotalRecipientsFailedToPreLicense,
			PrelicenseAgentPerfCounters.Percentile95FailedToLicense,
			PrelicenseAgentPerfCounters.TotalMessagesLicensed,
			PrelicenseAgentPerfCounters.TotalMessagesFailedToLicense,
			PrelicenseAgentPerfCounters.TotalDeferralsToLicense,
			PrelicenseAgentPerfCounters.TotalExternalMessagesPreLicensed,
			PrelicenseAgentPerfCounters.TotalExternalMessagesFailedToPreLicense,
			PrelicenseAgentPerfCounters.TotalDeferralsToPreLicenseForExternalMessages,
			PrelicenseAgentPerfCounters.TotalRecipientsPreLicensedForExternalMessages,
			PrelicenseAgentPerfCounters.TotalRecipientsFailedToPreLicenseForExternalMessages
		};
	}
}
