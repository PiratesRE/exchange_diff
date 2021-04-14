using System;
using System.Xml;
using System.Xml.Linq;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.InfoWorker.Common.Availability
{
	internal static class PerformanceCounters
	{
		public static void GetPerfCounterInfo(XElement element)
		{
			if (PerformanceCounters.AllCounters == null)
			{
				return;
			}
			foreach (ExPerformanceCounter exPerformanceCounter in PerformanceCounters.AllCounters)
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

		public const string CategoryName = "MSExchange Availability Service";

		public static readonly ExPerformanceCounter AverageFreeBusyRequestProcessingTime = new ExPerformanceCounter("MSExchange Availability Service", "Average Time to Process a Free Busy Request", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter AverageFreeBusyRequestProcessingTimeBase = new ExPerformanceCounter("MSExchange Availability Service", "Base for Average Time to Process a Free Busy Request", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter AverageSuggestionsRequestProcessingTime = new ExPerformanceCounter("MSExchange Availability Service", "Average Time to Process a Meeting Suggestions Request", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter AverageSuggestionsRequestProcessingTimeBase = new ExPerformanceCounter("MSExchange Availability Service", "Base for Average Time to Process a Meeting Suggestions Request", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter AverageMailboxCountPerRequest = new ExPerformanceCounter("MSExchange Availability Service", "Average Number of Mailboxes Processed per Request", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter AverageMailboxCountPerRequestBase = new ExPerformanceCounter("MSExchange Availability Service", "Base for Average Number of Mailboxes Processed per Request", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter AverageCrossSiteFreeBusyRequestProcessingTime = new ExPerformanceCounter("MSExchange Availability Service", "Average Time to Process a Cross-Site Free Busy Request", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter AverageCrossSiteFreeBusyRequestProcessingTimeBase = new ExPerformanceCounter("MSExchange Availability Service", "Base forAverage Time to Process a Cross-Site Free Busy Request", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter AverageCrossForestFreeBusyRequestProcessingTime = new ExPerformanceCounter("MSExchange Availability Service", "Average Time to Process a Cross-Forest Free Busy Request", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter AverageCrossForestFreeBusyRequestProcessingTimeBase = new ExPerformanceCounter("MSExchange Availability Service", "Base for Average Time to Process a Cross-Forest Free Busy Request", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter RequestsPerSecond = new ExPerformanceCounter("MSExchange Availability Service", "Availability Requests (sec)", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter SuggestionsRequestsPerSecond = new ExPerformanceCounter("MSExchange Availability Service", "Suggestions Requests (sec)", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter IntraSiteCalendarQueriesPerSecond = new ExPerformanceCounter("MSExchange Availability Service", "Intra-Site Calendar Queries (sec)", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter CrossSiteCalendarQueriesPerSecond = new ExPerformanceCounter("MSExchange Availability Service", "Cross-Site Calendar Queries (sec)", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter CrossForestCalendarQueriesPerSecond = new ExPerformanceCounter("MSExchange Availability Service", "Cross-Forest Calendar Queries (sec)", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter PublicFolderQueriesPerSecond = new ExPerformanceCounter("MSExchange Availability Service", "Public Folder Queries (sec)", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter ForeignConnectorQueriesPerSecond = new ExPerformanceCounter("MSExchange Availability Service", "Foreign Connector Queries (sec)", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter IntraSiteCalendarFailuresPerSecond = new ExPerformanceCounter("MSExchange Availability Service", "Intra-Site Calendar Failures (sec)", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter CrossSiteCalendarFailuresPerSecond = new ExPerformanceCounter("MSExchange Availability Service", "Cross-Site Calendar Failures (sec)", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter CrossForestCalendarFailuresPerSecond = new ExPerformanceCounter("MSExchange Availability Service", "Cross-Forest Calendar Failures (sec)", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter PublicFolderRequestFailuresPerSecond = new ExPerformanceCounter("MSExchange Availability Service", "Public Folder Request Failures (sec)", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter ForeignConnectorFailuresPerSecond = new ExPerformanceCounter("MSExchange Availability Service", "Foreign Connector Request Failure Rate", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter AverageExternalAuthenticationIdentityMappingTime = new ExPerformanceCounter("MSExchange Availability Service", "Average Time to Map External Caller to Internal Identity", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter AverageExternalAuthenticationIdentityMappingTimeBase = new ExPerformanceCounter("MSExchange Availability Service", "Base for Average Time for External Caller to Internal Identity", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter FederatedFreeBusyQueriesPerSecond = new ExPerformanceCounter("MSExchange Availability Service", "Federated Free Busy Calendar Queries, including OAuth (sec)", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter FederatedFreeBusyFailuresPerSecond = new ExPerformanceCounter("MSExchange Availability Service", "Federated Free Busy Failures (sec)", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter AverageFederatedFreeBusyRequestProcessingTime = new ExPerformanceCounter("MSExchange Availability Service", "Average Time to Process a Federated Free Busy Request", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter AverageFederatedFreeBusyRequestProcessingTimeBase = new ExPerformanceCounter("MSExchange Availability Service", "Base for Average Time to Process a Federated Free Busy Request", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter FederatedByOAuthFreeBusyFailuresPerSecond = new ExPerformanceCounter("MSExchange Availability Service", "Federated Free Busy Failures with OAuth(sec)", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter AverageFederatedByOAuthFreeBusyRequestProcessingTime = new ExPerformanceCounter("MSExchange Availability Service", "Average Time to Process a Federated Free Busy Request with OAuth", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter AverageFederatedByOAuthFreeBusyRequestProcessingTimeBase = new ExPerformanceCounter("MSExchange Availability Service", "Base for Average Time to Process a Federated Free Busy Request with OAuth", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter CurrentRequests = new ExPerformanceCounter("MSExchange Availability Service", "Current Requests", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter IntraSiteProxyFreeBusyQueriesPerSecond = new ExPerformanceCounter("MSExchange Availability Service", "Intra-Site Proxy Free Busy Calendar Queries (sec)", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter IntraSiteProxyFreeBusyFailuresPerSecond = new ExPerformanceCounter("MSExchange Availability Service", "Intra-Site Proxy Free Busy Failures (sec)", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter AverageIntraSiteProxyFreeBusyRequestProcessingTime = new ExPerformanceCounter("MSExchange Availability Service", "Average Time to Process an Intra-Site Free Busy Request", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter AverageIntraSiteProxyFreeBusyRequestProcessingTimeBase = new ExPerformanceCounter("MSExchange Availability Service", "Base for Average Time to Process an Intra-Site Free/Busy Request", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter FailedClientReportedRequestsTotal = new ExPerformanceCounter("MSExchange Availability Service", "Client-Reported Failures - Total", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter FailedClientRequestsNoASUrl = new ExPerformanceCounter("MSExchange Availability Service", "Client-Reported Failures - Autodiscover Failures", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter FailedClientRequestsTimeouts = new ExPerformanceCounter("MSExchange Availability Service", "Client-Reported Failures - Timeout Failures", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter FailedClientRequestsNoConnection = new ExPerformanceCounter("MSExchange Availability Service", "Client-Reported Failures - Connection Failures", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter FailedClientRequestsPartialOrOther = new ExPerformanceCounter("MSExchange Availability Service", "Client-Reported Failures - Partial or Other Failures", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter PastTotalClientSuccessRequests = new ExPerformanceCounter("MSExchange Availability Service", "Successful Client-Reported Requests - Total", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter PastClientRequestsUnder5 = new ExPerformanceCounter("MSExchange Availability Service", "Successful Client-Reported Requests - Less than 5 seconds", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter PastClientRequestsUnder10 = new ExPerformanceCounter("MSExchange Availability Service", "Successful Client-Reported Requests - Less than 10 seconds", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter PastClientRequestsUnder20 = new ExPerformanceCounter("MSExchange Availability Service", "Successful Client-Reported Requests - Less than 20 seconds", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter PastClientRequestsOver20 = new ExPerformanceCounter("MSExchange Availability Service", "Successful Client-Reported Requests - Over 20 seconds", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter[] AllCounters = new ExPerformanceCounter[]
		{
			PerformanceCounters.AverageFreeBusyRequestProcessingTime,
			PerformanceCounters.AverageFreeBusyRequestProcessingTimeBase,
			PerformanceCounters.AverageSuggestionsRequestProcessingTime,
			PerformanceCounters.AverageSuggestionsRequestProcessingTimeBase,
			PerformanceCounters.AverageMailboxCountPerRequest,
			PerformanceCounters.AverageMailboxCountPerRequestBase,
			PerformanceCounters.AverageCrossSiteFreeBusyRequestProcessingTime,
			PerformanceCounters.AverageCrossSiteFreeBusyRequestProcessingTimeBase,
			PerformanceCounters.AverageCrossForestFreeBusyRequestProcessingTime,
			PerformanceCounters.AverageCrossForestFreeBusyRequestProcessingTimeBase,
			PerformanceCounters.RequestsPerSecond,
			PerformanceCounters.SuggestionsRequestsPerSecond,
			PerformanceCounters.IntraSiteCalendarQueriesPerSecond,
			PerformanceCounters.CrossSiteCalendarQueriesPerSecond,
			PerformanceCounters.CrossForestCalendarQueriesPerSecond,
			PerformanceCounters.PublicFolderQueriesPerSecond,
			PerformanceCounters.ForeignConnectorQueriesPerSecond,
			PerformanceCounters.IntraSiteCalendarFailuresPerSecond,
			PerformanceCounters.CrossSiteCalendarFailuresPerSecond,
			PerformanceCounters.CrossForestCalendarFailuresPerSecond,
			PerformanceCounters.PublicFolderRequestFailuresPerSecond,
			PerformanceCounters.ForeignConnectorFailuresPerSecond,
			PerformanceCounters.AverageExternalAuthenticationIdentityMappingTime,
			PerformanceCounters.AverageExternalAuthenticationIdentityMappingTimeBase,
			PerformanceCounters.FederatedFreeBusyQueriesPerSecond,
			PerformanceCounters.FederatedFreeBusyFailuresPerSecond,
			PerformanceCounters.AverageFederatedFreeBusyRequestProcessingTime,
			PerformanceCounters.AverageFederatedFreeBusyRequestProcessingTimeBase,
			PerformanceCounters.FederatedByOAuthFreeBusyFailuresPerSecond,
			PerformanceCounters.AverageFederatedByOAuthFreeBusyRequestProcessingTime,
			PerformanceCounters.AverageFederatedByOAuthFreeBusyRequestProcessingTimeBase,
			PerformanceCounters.CurrentRequests,
			PerformanceCounters.IntraSiteProxyFreeBusyQueriesPerSecond,
			PerformanceCounters.IntraSiteProxyFreeBusyFailuresPerSecond,
			PerformanceCounters.AverageIntraSiteProxyFreeBusyRequestProcessingTime,
			PerformanceCounters.AverageIntraSiteProxyFreeBusyRequestProcessingTimeBase,
			PerformanceCounters.FailedClientReportedRequestsTotal,
			PerformanceCounters.FailedClientRequestsNoASUrl,
			PerformanceCounters.FailedClientRequestsTimeouts,
			PerformanceCounters.FailedClientRequestsNoConnection,
			PerformanceCounters.FailedClientRequestsPartialOrOther,
			PerformanceCounters.PastTotalClientSuccessRequests,
			PerformanceCounters.PastClientRequestsUnder5,
			PerformanceCounters.PastClientRequestsUnder10,
			PerformanceCounters.PastClientRequestsUnder20,
			PerformanceCounters.PastClientRequestsOver20
		};
	}
}
