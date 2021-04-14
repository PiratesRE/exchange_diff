using System;
using System.Xml;
using System.Xml.Linq;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Clients.Common
{
	internal static class OwaSingleCounters
	{
		public static void GetPerfCounterInfo(XElement element)
		{
			if (OwaSingleCounters.AllCounters == null)
			{
				return;
			}
			foreach (ExPerformanceCounter exPerformanceCounter in OwaSingleCounters.AllCounters)
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

		public const string CategoryName = "MSExchange OWA";

		public static readonly ExPerformanceCounter CurrentUsers = new ExPerformanceCounter("MSExchange OWA", "Current Users", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter CurrentUniqueUsers = new ExPerformanceCounter("MSExchange OWA", "Current Unique Users", string.Empty, null, new ExPerformanceCounter[0]);

		private static readonly ExPerformanceCounter LogonsPerSecond = new ExPerformanceCounter("MSExchange OWA", "Logons/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalUsers = new ExPerformanceCounter("MSExchange OWA", "Total Users", string.Empty, null, new ExPerformanceCounter[]
		{
			OwaSingleCounters.LogonsPerSecond
		});

		public static readonly ExPerformanceCounter TotalUniqueUsers = new ExPerformanceCounter("MSExchange OWA", "Total Unique Users", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter PeakUserCount = new ExPerformanceCounter("MSExchange OWA", "Peak User Count", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter CurrentUsersLight = new ExPerformanceCounter("MSExchange OWA", "Current Users Light", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter CurrentUniqueUsersLight = new ExPerformanceCounter("MSExchange OWA", "Current Unique Users Light", string.Empty, null, new ExPerformanceCounter[0]);

		private static readonly ExPerformanceCounter LogonsPerSecondLight = new ExPerformanceCounter("MSExchange OWA", "Logons/sec Light", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalUsersLight = new ExPerformanceCounter("MSExchange OWA", "Total Users Light", string.Empty, null, new ExPerformanceCounter[]
		{
			OwaSingleCounters.LogonsPerSecondLight
		});

		public static readonly ExPerformanceCounter TotalUniqueUsersLight = new ExPerformanceCounter("MSExchange OWA", "Total Unique Users Light", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter PeakUserCountLight = new ExPerformanceCounter("MSExchange OWA", "Peak User Count Light", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter CurrentUsersPremium = new ExPerformanceCounter("MSExchange OWA", "Current Users Premium", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter CurrentUniqueUsersPremium = new ExPerformanceCounter("MSExchange OWA", "Current Unique Users Premium", string.Empty, null, new ExPerformanceCounter[0]);

		private static readonly ExPerformanceCounter LogonsPerSecondPremium = new ExPerformanceCounter("MSExchange OWA", "Logons/sec Premium", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalUsersPremium = new ExPerformanceCounter("MSExchange OWA", "Total Users Premium", string.Empty, null, new ExPerformanceCounter[]
		{
			OwaSingleCounters.LogonsPerSecondPremium
		});

		public static readonly ExPerformanceCounter TotalUniqueUsersPremium = new ExPerformanceCounter("MSExchange OWA", "Total Unique Users Premium", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter PeakUserCountPremium = new ExPerformanceCounter("MSExchange OWA", "Peak User Count Premium", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter AverageResponseTime = new ExPerformanceCounter("MSExchange OWA", "Average Response Time", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalSessionsEndedByLogoff = new ExPerformanceCounter("MSExchange OWA", "Sessions Ended by Logoff", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalSessionsEndedByTimeout = new ExPerformanceCounter("MSExchange OWA", "Sessions Ended by Time-out", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter CalendarViewsLoaded = new ExPerformanceCounter("MSExchange OWA", "Calendar Views Loaded", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter CalendarViewsRefreshed = new ExPerformanceCounter("MSExchange OWA", "Calendar View Refreshed", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter ItemsCreated = new ExPerformanceCounter("MSExchange OWA", "Items Created Since OWA Start", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter ItemsUpdated = new ExPerformanceCounter("MSExchange OWA", "Items Updated Since OWA Start", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter ItemsDeleted = new ExPerformanceCounter("MSExchange OWA", "Items Deleted Since OWA Start", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter AttachmentsUploaded = new ExPerformanceCounter("MSExchange OWA", "Attachments Uploaded Since OWA Start", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter MailViewsLoaded = new ExPerformanceCounter("MSExchange OWA", "Mail Views Loaded", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter MailViewRefreshes = new ExPerformanceCounter("MSExchange OWA", "Mail View Refreshes", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter MessagesSent = new ExPerformanceCounter("MSExchange OWA", "Messages Sent", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter IRMMessagesSent = new ExPerformanceCounter("MSExchange OWA", "IRM-protected Messages Sent", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter AverageSearchTime = new ExPerformanceCounter("MSExchange OWA", "Average Search Time", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalSearches = new ExPerformanceCounter("MSExchange OWA", "Searches", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter SearchesTimedOut = new ExPerformanceCounter("MSExchange OWA", "Searches Timed Out", string.Empty, null, new ExPerformanceCounter[0]);

		private static readonly ExPerformanceCounter RequestsPerSecond = new ExPerformanceCounter("MSExchange OWA", "Requests/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalRequests = new ExPerformanceCounter("MSExchange OWA", "Requests", string.Empty, null, new ExPerformanceCounter[]
		{
			OwaSingleCounters.RequestsPerSecond
		});

		private static readonly ExPerformanceCounter RequestsFailedPerSecond = new ExPerformanceCounter("MSExchange OWA", "Failed Requests/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalRequestsFailed = new ExPerformanceCounter("MSExchange OWA", "Requests Failed", string.Empty, null, new ExPerformanceCounter[]
		{
			OwaSingleCounters.RequestsFailedPerSecond
		});

		public static readonly ExPerformanceCounter PID = new ExPerformanceCounter("MSExchange OWA", "PID", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter AverageSpellcheckTime = new ExPerformanceCounter("MSExchange OWA", "Average Check Spelling Time", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalSpellchecks = new ExPerformanceCounter("MSExchange OWA", "Spelling Checks", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalConversions = new ExPerformanceCounter("MSExchange OWA", "Conversions", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter ActiveConversions = new ExPerformanceCounter("MSExchange OWA", "Active Conversions", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalRejectedConversions = new ExPerformanceCounter("MSExchange OWA", "Rejected Conversions", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter QueuedConversionRequests = new ExPerformanceCounter("MSExchange OWA", "Queued Conversion Requests", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalTimeoutConversions = new ExPerformanceCounter("MSExchange OWA", "Conversions Ended by Time-out", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalErrorConversions = new ExPerformanceCounter("MSExchange OWA", "Conversions Ended with Errors", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalConvertingRequestsRate = new ExPerformanceCounter("MSExchange OWA", "Conversion Requests KB/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter SuccessfulConversionRequestRate = new ExPerformanceCounter("MSExchange OWA", "Successful Conversion Requests KB/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalConvertingResponseRate = new ExPerformanceCounter("MSExchange OWA", "Conversion Responses KB/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter AverageConvertingTime = new ExPerformanceCounter("MSExchange OWA", "Average Conversion Time", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter AverageConversionQueuingTime = new ExPerformanceCounter("MSExchange OWA", "Average Conversion Queuing Time", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter InvalidCanaryRequests = new ExPerformanceCounter("MSExchange OWA", "Invalid Canary Requests", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter NamesChecked = new ExPerformanceCounter("MSExchange OWA", "Names Checked", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter PasswordChanges = new ExPerformanceCounter("MSExchange OWA", "Password Changes", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter CurrentProxiedUsers = new ExPerformanceCounter("MSExchange OWA", "Current Proxy Users", string.Empty, null, new ExPerformanceCounter[0]);

		private static readonly ExPerformanceCounter ProxiedUserRequestsPerSecond = new ExPerformanceCounter("MSExchange OWA", "Proxy User Requests/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter ProxiedUserRequests = new ExPerformanceCounter("MSExchange OWA", "Proxy User Requests", string.Empty, null, new ExPerformanceCounter[]
		{
			OwaSingleCounters.ProxiedUserRequestsPerSecond
		});

		public static readonly ExPerformanceCounter ProxiedResponseTimeAverage = new ExPerformanceCounter("MSExchange OWA", "Proxy Response Time Average", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter ProxyRequestBytes = new ExPerformanceCounter("MSExchange OWA", "Proxy Request Bytes", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter ProxyResponseBytes = new ExPerformanceCounter("MSExchange OWA", "Proxy Response Bytes", string.Empty, null, new ExPerformanceCounter[0]);

		private static readonly ExPerformanceCounter WssBytesPerSecond = new ExPerformanceCounter("MSExchange OWA", "WSS Response Bytes/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter WssBytes = new ExPerformanceCounter("MSExchange OWA", "WSS Response Bytes", string.Empty, null, new ExPerformanceCounter[]
		{
			OwaSingleCounters.WssBytesPerSecond
		});

		private static readonly ExPerformanceCounter UncBytesPerSecond = new ExPerformanceCounter("MSExchange OWA", "UNC Response Bytes/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter UncBytes = new ExPerformanceCounter("MSExchange OWA", "UNC Response Bytes", string.Empty, null, new ExPerformanceCounter[]
		{
			OwaSingleCounters.UncBytesPerSecond
		});

		public static readonly ExPerformanceCounter WssRequests = new ExPerformanceCounter("MSExchange OWA", "WSS Requests", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter UncRequests = new ExPerformanceCounter("MSExchange OWA", "UNC Requests", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter ASQueries = new ExPerformanceCounter("MSExchange OWA", "AS Queries", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter ASQueriesFailurePercent = new ExPerformanceCounter("MSExchange OWA", "AS Queries Failure %", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter StoreLogonFailurePercent = new ExPerformanceCounter("MSExchange OWA", "Store Logon Failure %", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter CASIntraSiteRedirectionLatertoEarlierVersion = new ExPerformanceCounter("MSExchange OWA", "CAS Intra-Site Redirection Later to Earlier Version", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter CASIntraSiteRedirectionEarliertoLaterVersion = new ExPerformanceCounter("MSExchange OWA", "CAS Intra-Site Redirection Earlier to Later Version", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter CASCrossSiteRedirectionLatertoEarlierVersion = new ExPerformanceCounter("MSExchange OWA", "CAS Cross-Site Redirection Later to Earlier Version", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter CASCrossSiteRedirectionEarliertoLaterVersion = new ExPerformanceCounter("MSExchange OWA", "CAS Cross-Site Redirection Earlier to Later Version", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter ActiveMailboxSubscriptions = new ExPerformanceCounter("MSExchange OWA", "Active Mailbox Subscriptions", string.Empty, null, new ExPerformanceCounter[0]);

		private static readonly ExPerformanceCounter MailboxNotificationsPerSecond = new ExPerformanceCounter("MSExchange OWA", "Mailbox Notifications/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalMailboxNotifications = new ExPerformanceCounter("MSExchange OWA", "Total Mailbox Notifications", string.Empty, null, new ExPerformanceCounter[]
		{
			OwaSingleCounters.MailboxNotificationsPerSecond
		});

		public static readonly ExPerformanceCounter TotalUserContextReInitializationRequests = new ExPerformanceCounter("MSExchange OWA", "Total Usercontext ReInitialization requests", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter MailboxOfflineExceptionFailuresPercent = new ExPerformanceCounter("MSExchange OWA", "Mailbox Offline Exception Failure %", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter ConnectionFailedTransientExceptionPercent = new ExPerformanceCounter("MSExchange OWA", "Connection Failed Transient Exception %", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter StorageTransientExceptionPercent = new ExPerformanceCounter("MSExchange OWA", "Storage Transient Exception Failure %", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter StoragePermanentExceptionPercent = new ExPerformanceCounter("MSExchange OWA", "Storage Permanent Exception Failure %", string.Empty, null, new ExPerformanceCounter[0]);

		private static readonly ExPerformanceCounter IMInstantMessagesSentPerSecond = new ExPerformanceCounter("MSExchange OWA", "IM - Messages Sent/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter IMTotalInstantMessagesSent = new ExPerformanceCounter("MSExchange OWA", "IM - Total Messages Sent", string.Empty, null, new ExPerformanceCounter[]
		{
			OwaSingleCounters.IMInstantMessagesSentPerSecond
		});

		private static readonly ExPerformanceCounter IMInstantMessagesReceivedPerSecond = new ExPerformanceCounter("MSExchange OWA", "IM - Messages Received/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter IMTotalInstantMessagesReceived = new ExPerformanceCounter("MSExchange OWA", "IM - Total Messages Received", string.Empty, null, new ExPerformanceCounter[]
		{
			OwaSingleCounters.IMInstantMessagesReceivedPerSecond
		});

		private static readonly ExPerformanceCounter IMPresenceQueriesPerSecond = new ExPerformanceCounter("MSExchange OWA", "IM - Presence Queries/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter IMTotalPresenceQueries = new ExPerformanceCounter("MSExchange OWA", "IM - Total Presence Queries", string.Empty, null, new ExPerformanceCounter[]
		{
			OwaSingleCounters.IMPresenceQueriesPerSecond
		});

		private static readonly ExPerformanceCounter IMMessageDeliveryFailuresPerSecond = new ExPerformanceCounter("MSExchange OWA", "IM - Message Delivery Failures/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter IMTotalMessageDeliveryFailures = new ExPerformanceCounter("MSExchange OWA", "IM - Total Message Delivery Failures", string.Empty, null, new ExPerformanceCounter[]
		{
			OwaSingleCounters.IMMessageDeliveryFailuresPerSecond
		});

		private static readonly ExPerformanceCounter IMLogonFailuresPerSecond = new ExPerformanceCounter("MSExchange OWA", "IM - Sign-In Failures/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter IMTotalLogonFailures = new ExPerformanceCounter("MSExchange OWA", "IM - Sign-In Failures", string.Empty, null, new ExPerformanceCounter[]
		{
			OwaSingleCounters.IMLogonFailuresPerSecond
		});

		public static readonly ExPerformanceCounter IMCurrentUsers = new ExPerformanceCounter("MSExchange OWA", "IM - Users Currently Signed In", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter IMTotalUsers = new ExPerformanceCounter("MSExchange OWA", "IM - Total Users", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter IMAverageSignOnTime = new ExPerformanceCounter("MSExchange OWA", "IM - Average Sign-In Time", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter IMLogonFailuresPercent = new ExPerformanceCounter("MSExchange OWA", "IM - Sign-In Failure %", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter IMSentMessageDeliveryFailuresPercent = new ExPerformanceCounter("MSExchange OWA", "IM - Sent Message Delivery Failure %", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter OwaToEwsRequestFailureRate = new ExPerformanceCounter("MSExchange OWA", "Failure rate of requests from OWA to EWS.", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter RequestTimeouts = new ExPerformanceCounter("MSExchange OWA", "Request Time-Outs", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter SenderPhotosTotalLDAPCallsWithPicture = new ExPerformanceCounter("MSExchange OWA", "Sender Photos - Total LDAP calls returned non-empty image data", string.Empty, null, new ExPerformanceCounter[0]);

		private static readonly ExPerformanceCounter SenderPhotosLDAPCallsPerSecond = new ExPerformanceCounter("MSExchange OWA", "Sender Photos - LDAP calls/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter SenderPhotosTotalLDAPCalls = new ExPerformanceCounter("MSExchange OWA", "Sender Photos - Total LDAP calls", string.Empty, null, new ExPerformanceCounter[]
		{
			OwaSingleCounters.SenderPhotosLDAPCallsPerSecond
		});

		public static readonly ExPerformanceCounter SenderPhotosNegativeCacheCount = new ExPerformanceCounter("MSExchange OWA", "Sender Photos - Total entries in Recipients Negative Cache", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter SenderPhotosDataFromNegativeCacheCount = new ExPerformanceCounter("MSExchange OWA", "Sender Photos - Total number of avoided LDAP calls due to cache", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter AggregatedUserConfigurationPartsRebuilt = new ExPerformanceCounter("MSExchange OWA", "Aggregated Configuration - Rebuilds", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter AggregatedUserConfigurationPartsRead = new ExPerformanceCounter("MSExchange OWA", "Aggregated Configuration - Reads", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter AggregatedUserConfigurationPartsRequested = new ExPerformanceCounter("MSExchange OWA", "Aggregated Configuration - Requests", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter SessionDataCacheBuildsStarted = new ExPerformanceCounter("MSExchange OWA", "Session Data Cache - build starts", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter SessionDataCacheBuildsCompleted = new ExPerformanceCounter("MSExchange OWA", "Session Data Cache - builds completed", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter SessionDataCacheWaitedForPreload = new ExPerformanceCounter("MSExchange OWA", "Session Data Cache - waited for preload to complete", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter SessionDataCacheUsed = new ExPerformanceCounter("MSExchange OWA", "Session Data Cache - used", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter SessionDataCacheWaitTimeout = new ExPerformanceCounter("MSExchange OWA", "Session Data Cache - timeout", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter[] AllCounters = new ExPerformanceCounter[]
		{
			OwaSingleCounters.CurrentUsers,
			OwaSingleCounters.CurrentUniqueUsers,
			OwaSingleCounters.TotalUsers,
			OwaSingleCounters.TotalUniqueUsers,
			OwaSingleCounters.PeakUserCount,
			OwaSingleCounters.CurrentUsersLight,
			OwaSingleCounters.CurrentUniqueUsersLight,
			OwaSingleCounters.TotalUsersLight,
			OwaSingleCounters.TotalUniqueUsersLight,
			OwaSingleCounters.PeakUserCountLight,
			OwaSingleCounters.CurrentUsersPremium,
			OwaSingleCounters.CurrentUniqueUsersPremium,
			OwaSingleCounters.TotalUsersPremium,
			OwaSingleCounters.TotalUniqueUsersPremium,
			OwaSingleCounters.PeakUserCountPremium,
			OwaSingleCounters.AverageResponseTime,
			OwaSingleCounters.TotalSessionsEndedByLogoff,
			OwaSingleCounters.TotalSessionsEndedByTimeout,
			OwaSingleCounters.CalendarViewsLoaded,
			OwaSingleCounters.CalendarViewsRefreshed,
			OwaSingleCounters.ItemsCreated,
			OwaSingleCounters.ItemsUpdated,
			OwaSingleCounters.ItemsDeleted,
			OwaSingleCounters.AttachmentsUploaded,
			OwaSingleCounters.MailViewsLoaded,
			OwaSingleCounters.MailViewRefreshes,
			OwaSingleCounters.MessagesSent,
			OwaSingleCounters.IRMMessagesSent,
			OwaSingleCounters.AverageSearchTime,
			OwaSingleCounters.TotalSearches,
			OwaSingleCounters.SearchesTimedOut,
			OwaSingleCounters.TotalRequests,
			OwaSingleCounters.TotalRequestsFailed,
			OwaSingleCounters.PID,
			OwaSingleCounters.AverageSpellcheckTime,
			OwaSingleCounters.TotalSpellchecks,
			OwaSingleCounters.TotalConversions,
			OwaSingleCounters.ActiveConversions,
			OwaSingleCounters.TotalRejectedConversions,
			OwaSingleCounters.QueuedConversionRequests,
			OwaSingleCounters.TotalTimeoutConversions,
			OwaSingleCounters.TotalErrorConversions,
			OwaSingleCounters.TotalConvertingRequestsRate,
			OwaSingleCounters.SuccessfulConversionRequestRate,
			OwaSingleCounters.TotalConvertingResponseRate,
			OwaSingleCounters.AverageConvertingTime,
			OwaSingleCounters.AverageConversionQueuingTime,
			OwaSingleCounters.InvalidCanaryRequests,
			OwaSingleCounters.NamesChecked,
			OwaSingleCounters.PasswordChanges,
			OwaSingleCounters.CurrentProxiedUsers,
			OwaSingleCounters.ProxiedUserRequests,
			OwaSingleCounters.ProxiedResponseTimeAverage,
			OwaSingleCounters.ProxyRequestBytes,
			OwaSingleCounters.ProxyResponseBytes,
			OwaSingleCounters.WssBytes,
			OwaSingleCounters.UncBytes,
			OwaSingleCounters.WssRequests,
			OwaSingleCounters.UncRequests,
			OwaSingleCounters.ASQueries,
			OwaSingleCounters.ASQueriesFailurePercent,
			OwaSingleCounters.StoreLogonFailurePercent,
			OwaSingleCounters.CASIntraSiteRedirectionLatertoEarlierVersion,
			OwaSingleCounters.CASIntraSiteRedirectionEarliertoLaterVersion,
			OwaSingleCounters.CASCrossSiteRedirectionLatertoEarlierVersion,
			OwaSingleCounters.CASCrossSiteRedirectionEarliertoLaterVersion,
			OwaSingleCounters.ActiveMailboxSubscriptions,
			OwaSingleCounters.TotalMailboxNotifications,
			OwaSingleCounters.TotalUserContextReInitializationRequests,
			OwaSingleCounters.MailboxOfflineExceptionFailuresPercent,
			OwaSingleCounters.ConnectionFailedTransientExceptionPercent,
			OwaSingleCounters.StorageTransientExceptionPercent,
			OwaSingleCounters.StoragePermanentExceptionPercent,
			OwaSingleCounters.IMTotalInstantMessagesSent,
			OwaSingleCounters.IMTotalInstantMessagesReceived,
			OwaSingleCounters.IMTotalPresenceQueries,
			OwaSingleCounters.IMTotalMessageDeliveryFailures,
			OwaSingleCounters.IMTotalLogonFailures,
			OwaSingleCounters.IMCurrentUsers,
			OwaSingleCounters.IMTotalUsers,
			OwaSingleCounters.IMAverageSignOnTime,
			OwaSingleCounters.IMLogonFailuresPercent,
			OwaSingleCounters.IMSentMessageDeliveryFailuresPercent,
			OwaSingleCounters.OwaToEwsRequestFailureRate,
			OwaSingleCounters.RequestTimeouts,
			OwaSingleCounters.SenderPhotosTotalLDAPCalls,
			OwaSingleCounters.SenderPhotosTotalLDAPCallsWithPicture,
			OwaSingleCounters.SenderPhotosNegativeCacheCount,
			OwaSingleCounters.SenderPhotosDataFromNegativeCacheCount,
			OwaSingleCounters.AggregatedUserConfigurationPartsRebuilt,
			OwaSingleCounters.AggregatedUserConfigurationPartsRead,
			OwaSingleCounters.AggregatedUserConfigurationPartsRequested,
			OwaSingleCounters.SessionDataCacheBuildsStarted,
			OwaSingleCounters.SessionDataCacheBuildsCompleted,
			OwaSingleCounters.SessionDataCacheWaitedForPreload,
			OwaSingleCounters.SessionDataCacheUsed,
			OwaSingleCounters.SessionDataCacheWaitTimeout
		};
	}
}
