using System;
using System.Collections.Specialized;
using System.Configuration;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.InfoWorker.Availability;
using Microsoft.Exchange.InfoWorker.EventLog;
using Microsoft.Exchange.Net;

namespace Microsoft.Exchange.InfoWorker.Common.Availability
{
	internal static class Configuration
	{
		public static int MaximumRequestStreamSize
		{
			get
			{
				if (Configuration.maximumRequestStreamSize == -1)
				{
					Configuration.maximumRequestStreamSize = Configuration.ReadIntValue("MaximumRequestStreamSize", 409600);
					Configuration.ConfigurationTracer.TraceDebug<int>(0L, "Using MaximumRequestStreamSize = {0}", Configuration.maximumRequestStreamSize);
				}
				return Configuration.maximumRequestStreamSize;
			}
		}

		public static int MaximumQueryIntervalDays
		{
			get
			{
				if (Configuration.maximumQueryIntervalDays == -1)
				{
					Configuration.maximumQueryIntervalDays = Configuration.ReadIntValue("MaximumQueryIntervalDays", 62);
					Configuration.ConfigurationTracer.TraceDebug<int>(0L, "Using MaximumQueryIntervalDays = {0}", Configuration.maximumQueryIntervalDays);
				}
				return Configuration.maximumQueryIntervalDays;
			}
		}

		public static int MaximumIdentityArraySize
		{
			get
			{
				if (Configuration.maximumIdentityArraySize == -1)
				{
					Configuration.maximumIdentityArraySize = Configuration.ReadIntValue("MaximumIdentityArraySize", 100);
					Configuration.ConfigurationTracer.TraceDebug<int>(0L, "Using MaximumIdentityArraySize = {0}", Configuration.maximumIdentityArraySize);
				}
				return Configuration.maximumIdentityArraySize;
			}
		}

		public static int MaximumResultSetSize
		{
			get
			{
				if (Configuration.maximumResultSetSize == -1)
				{
					Configuration.maximumResultSetSize = Configuration.ReadIntValue("MaximumResultSetSize", 1000);
					Configuration.ConfigurationTracer.TraceDebug<int>(0L, "Using MaximumResultSetSize = {0}", Configuration.maximumResultSetSize);
				}
				return Configuration.maximumResultSetSize;
			}
		}

		public static int ConnectionPoolSize
		{
			get
			{
				if (Configuration.connectionPoolSize == -1)
				{
					Configuration.connectionPoolSize = Configuration.ReadIntValue("ConnectionPoolSize", 255);
					Configuration.ConfigurationTracer.TraceDebug<int>(0L, "Using ConnectionPoolSize = {0}", Configuration.connectionPoolSize);
				}
				return Configuration.connectionPoolSize;
			}
		}

		public static TimeSpan WebRequestTimeoutInSeconds
		{
			get
			{
				if (Configuration.webRequestTimeoutInSeconds == -1)
				{
					Configuration.webRequestTimeoutInSeconds = Configuration.ReadIntValue("WebRequestTimeoutInSeconds", 25);
					Configuration.ConfigurationTracer.TraceDebug<int>(0L, "Using WebRequestTimeoutInSeconds = {0}", Configuration.webRequestTimeoutInSeconds);
				}
				return TimeSpan.FromSeconds((double)Configuration.webRequestTimeoutInSeconds);
			}
		}

		public static int MaximumGroupMemberCount
		{
			get
			{
				if (Configuration.maximumGroupMemberCount == -1)
				{
					Configuration.maximumGroupMemberCount = Configuration.ReadIntValue("MaximumGroupMemberCount", 20);
					Configuration.ConfigurationTracer.TraceDebug<int>(0L, "Using MaximumGroupMemberCount = {0}", Configuration.maximumGroupMemberCount);
				}
				return Configuration.maximumGroupMemberCount;
			}
		}

		public static bool UseSSLForCrossSiteRequests
		{
			get
			{
				if (Configuration.useSSLForCrossSiteRequests == -1)
				{
					Configuration.useSSLForCrossSiteRequests = Configuration.ReadIntValue("UseSSLForCrossSiteRequests", 1);
					Configuration.ConfigurationTracer.TraceDebug<int>(0L, "Using UseSSLForCrossSiteRequests = {0}", Configuration.useSSLForCrossSiteRequests);
				}
				return Configuration.useSSLForCrossSiteRequests > 0;
			}
		}

		public static bool UseSSLForAutoDiscoverRequests
		{
			get
			{
				if (Configuration.useSSLForAutoDiscoverRequests == -1)
				{
					Configuration.useSSLForAutoDiscoverRequests = Configuration.ReadIntValue("UseSSLForAutoDiscoverRequests", 1);
					Configuration.ConfigurationTracer.TraceDebug<int>(0L, "Using UseSSLForAutoDiscoverRequests = {0}", Configuration.useSSLForAutoDiscoverRequests);
				}
				return Configuration.useSSLForAutoDiscoverRequests > 0;
			}
		}

		public static bool DisableGzipForProxyRequests
		{
			get
			{
				if (Configuration.disableGzipForProxyRequests == -1)
				{
					Configuration.disableGzipForProxyRequests = Configuration.ReadIntValue("DisableGzipForProxyRequests", 0);
					Configuration.ConfigurationTracer.TraceDebug<int>(0L, "Using DisableGzipForProxyRequests = {0}", Configuration.disableGzipForProxyRequests);
				}
				return Configuration.disableGzipForProxyRequests > 0;
			}
		}

		public static int ADRefreshIntervalInMinutes
		{
			get
			{
				if (Configuration.adRefreshIntervalInMinutes == -1)
				{
					Configuration.adRefreshIntervalInMinutes = Configuration.ReadIntValue("ADRefreshIntervalInMinutes", 60);
					Configuration.ConfigurationTracer.TraceDebug<int>(0L, "Using ADRefreshIntervalInMinutes = {0}", Configuration.adRefreshIntervalInMinutes);
				}
				return Configuration.adRefreshIntervalInMinutes;
			}
		}

		public static TimeSpan IntraSiteTimeout
		{
			get
			{
				if (Configuration.intraSiteTimeoutInSeconds == -1)
				{
					Configuration.intraSiteTimeoutInSeconds = Configuration.ReadIntValue("IntraSiteTimeoutInSeconds", 50);
					Configuration.ConfigurationTracer.TraceDebug<int>(0L, "Using IntraSiteTimeoutInSeconds = {0}", Configuration.intraSiteTimeoutInSeconds);
				}
				return TimeSpan.FromSeconds((double)Configuration.intraSiteTimeoutInSeconds);
			}
		}

		public static bool BypassProxyForCrossSiteRequests
		{
			get
			{
				if (Configuration.bypassProxyForCrossSiteRequests == -1)
				{
					Configuration.bypassProxyForCrossSiteRequests = Configuration.ReadIntValue("BypassProxyForCrossSiteRequests", 1);
					Configuration.ConfigurationTracer.TraceDebug<int>(0L, "Using BypassProxyForCrossSiteRequests = {0}", Configuration.bypassProxyForCrossSiteRequests);
				}
				return Configuration.bypassProxyForCrossSiteRequests > 0;
			}
		}

		public static bool BypassProxyForCrossForestRequests
		{
			get
			{
				if (Configuration.bypassProxyForCrossForestRequests == -1)
				{
					Configuration.bypassProxyForCrossForestRequests = Configuration.ReadIntValue("BypassProxyForCrossForestRequests", 0);
					Configuration.ConfigurationTracer.TraceDebug<int>(0L, "Using BypassProxyForCrossForestRequests = {0}", Configuration.bypassProxyForCrossForestRequests);
				}
				return Configuration.bypassProxyForCrossForestRequests > 0;
			}
		}

		public static TimeSpan RecipientResolutionTimeout
		{
			get
			{
				if (Configuration.recipientResolutionTimeoutInSeconds == -1)
				{
					Configuration.recipientResolutionTimeoutInSeconds = Configuration.ReadIntValue("RecipientResolutionTimeoutInSeconds", 10);
					Configuration.ConfigurationTracer.TraceDebug<int>(0L, "Using RecipientResolutionTimeoutInSeconds = {0}", Configuration.recipientResolutionTimeoutInSeconds);
				}
				return TimeSpan.FromSeconds((double)Configuration.recipientResolutionTimeoutInSeconds);
			}
		}

		public static int MaximumDatabasesInQuery
		{
			get
			{
				if (Configuration.maximumDatabasesInQuery == -1)
				{
					Configuration.maximumDatabasesInQuery = Configuration.ReadIntValue("MaximumDatabasesInQuery", 100);
					Configuration.ConfigurationTracer.TraceDebug<string, int>(0L, "Using {0} = {1}", "MaximumDatabasesInQuery", Configuration.maximumDatabasesInQuery);
					if (Configuration.maximumDatabasesInQuery < 1)
					{
						Configuration.ConfigurationTracer.TraceError(0L, "The {0} setting in the configuration file has been assigned the value {1}, which is lower than the minimum supported value {2}. The Availability service is resetting the {0} value to {2}. The maximum {0} value supported is {3}", new object[]
						{
							"MaximumDatabasesInQuery",
							Configuration.maximumDatabasesInQuery,
							1,
							1000
						});
						Globals.AvailabilityLogger.LogEvent(InfoWorkerEventLogConstants.Tuple_InvalidMinimumDatabasesInQuery, "MaximumDatabasesInQuery", new object[]
						{
							Globals.ProcessId,
							Configuration.maximumDatabasesInQuery,
							1
						});
						Configuration.maximumDatabasesInQuery = 1;
					}
					else if (Configuration.maximumDatabasesInQuery > 1000)
					{
						Configuration.ConfigurationTracer.TraceError(0L, "The {0} setting in the configuration file has been assigned the value {1}, which exceeds the maximum supported value {2}. The Availability service is resetting the {0} value to {2}. The minimum {0} value supported is {3}", new object[]
						{
							"MaximumDatabasesInQuery",
							Configuration.maximumDatabasesInQuery,
							1000,
							1
						});
						Globals.AvailabilityLogger.LogEvent(InfoWorkerEventLogConstants.Tuple_InvalidMaximumDatabasesInQuery, "MaximumDatabasesInQuery", new object[]
						{
							Globals.ProcessId,
							Configuration.maximumDatabasesInQuery,
							1000
						});
						Configuration.maximumDatabasesInQuery = 1000;
					}
				}
				return Configuration.maximumDatabasesInQuery;
			}
		}

		public static bool UseDisabledAccount
		{
			get
			{
				if (Configuration.useDisabledAccount == -1)
				{
					Configuration.useDisabledAccount = Configuration.ReadIntValue("UseDisabledAccount", -1);
					Configuration.ConfigurationTracer.TraceDebug<int>(0L, "Using UseDisabledAccount = {0}", Configuration.useDisabledAccount);
				}
				return Configuration.useDisabledAccount > 0;
			}
		}

		private static int ReadIntValue(string name, int defaultValue)
		{
			int result;
			if (!int.TryParse(Configuration.parameterCollection[name], out result))
			{
				Configuration.ConfigurationTracer.TraceError<string>(0L, "Error while parsing configuration value {0}. Default value is being used.", name);
				result = defaultValue;
			}
			return result;
		}

		internal const int MinFreeBusyMergeInterval = 5;

		internal const int MaxFreeBusyMergeInterval = 1440;

		internal const int MaximumRequestStreamSizeDefault = 409600;

		internal const int MaximumQueryIntervalDaysDefault = 62;

		internal const int MaximumIdentityArraySizeDefault = 100;

		internal const int MaximumResultSetSizeDefault = 1000;

		internal const int MapiConnectionPoolSizeDefault = 255;

		internal const int WebRequestTimeoutInSecondsDefault = 25;

		internal const int MaximumGroupMemberCountDefault = 20;

		internal const int UseSSLForCrossSiteRequestsDefault = 1;

		internal const int UseSSLForAutoDiscoverRequestsDefault = 1;

		internal const int BypassProxyForCrossSiteRequestsDefault = 1;

		internal const int BypassProxyForCrossForestRequestsDefault = 0;

		internal const int DisableGzipForProxyRequestsDefault = 0;

		internal const int ADRefreshIntervalInMinutesDefault = 60;

		internal const int IntraSiteTimeoutInSecondsDefault = 50;

		internal const int RecipientResolutionTimeoutInSecondsDefault = 10;

		internal const int MaximumDatabasesInQueryDefault = 100;

		internal const int MaximumDatabasesInQueryAllowed = 1000;

		internal const int MinimumDatabasesInQueryAllowed = 1;

		internal const string MaximumDatabasesInQueryKey = "MaximumDatabasesInQuery";

		public static StringAppSettingsEntry DnsServerAddress = new StringAppSettingsEntry("DnsIpAddress", null, Configuration.ConfigurationTracer);

		public static BoolAppSettingsEntry BypassDnsCache = new BoolAppSettingsEntry("BypassDnsCache", false, Configuration.ConfigurationTracer);

		public static TimeSpanAppSettingsEntry RemoteUriInvalidCacheDurationInSeconds = new TimeSpanAppSettingsEntry("RemoteUriInvalidCacheDurationInSeconds", TimeSpanUnit.Seconds, TimeSpan.FromHours(1.0), Configuration.ConfigurationTracer);

		public static TimeSpanAppSettingsEntry AutodiscoverSrvRecordLookupTimeoutInSeconds = new TimeSpanAppSettingsEntry("AutodiscoverSrvRecordLookupTimeoutInSeconds", TimeSpanUnit.Seconds, TimeSpan.FromSeconds(5.0), Configuration.ConfigurationTracer);

		public static BoolAppSettingsEntry UnsafeAuthenticatedConnectionSharing = new BoolAppSettingsEntry("UnsafeAuthenticatedConnectionSharing", true, Configuration.ConfigurationTracer);

		private static NameValueCollection parameterCollection = ConfigurationManager.AppSettings;

		private static int maximumRequestStreamSize = -1;

		private static int maximumQueryIntervalDays = -1;

		private static int maximumIdentityArraySize = -1;

		private static int maximumResultSetSize = -1;

		private static int connectionPoolSize = -1;

		private static int webRequestTimeoutInSeconds = -1;

		private static int maximumGroupMemberCount = -1;

		private static int useSSLForCrossSiteRequests = -1;

		private static int useSSLForAutoDiscoverRequests = -1;

		private static int bypassProxyForCrossSiteRequests = -1;

		private static int bypassProxyForCrossForestRequests = -1;

		private static int adRefreshIntervalInMinutes = -1;

		private static int intraSiteTimeoutInSeconds = -1;

		private static int disableGzipForProxyRequests = -1;

		private static int recipientResolutionTimeoutInSeconds = -1;

		private static int maximumDatabasesInQuery = -1;

		private static int useDisabledAccount = -1;

		private static readonly Trace ConfigurationTracer = ExTraceGlobals.ConfigurationTracer;
	}
}
