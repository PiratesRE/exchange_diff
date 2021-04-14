using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Directory;
using Microsoft.Exchange.Net;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	internal static class CacheTimeToLive
	{
		public static TimeSpan FederatedCacheTimeToLive
		{
			get
			{
				return CacheTimeToLive.FederatedCacheTimeToLiveData.Value;
			}
		}

		public static TimeSpan OrgPropertyCacheTimeToLive
		{
			get
			{
				return CacheTimeToLive.OrgPropertyCacheTimeToLiveData.Value;
			}
		}

		public static TimeSpan GlobalCountryListCacheTimeToLive
		{
			get
			{
				return CacheTimeToLive.GlobalCountryListCacheTimeToLiveData.Value;
			}
		}

		private static TimeSpan DefaultFederatedCacheTimeToLive
		{
			get
			{
				if (!ExEnvironment.IsTest)
				{
					return TimeSpan.FromMinutes(5.0);
				}
				return TimeSpan.FromMilliseconds(1.0);
			}
		}

		private static readonly TimeSpanAppSettingsEntry FederatedCacheTimeToLiveData = new TimeSpanAppSettingsEntry("FederatedCacheTimeToLive", TimeSpanUnit.Seconds, CacheTimeToLive.DefaultFederatedCacheTimeToLive, ExTraceGlobals.SystemConfigurationCacheTracer);

		private static readonly TimeSpanAppSettingsEntry OrgPropertyCacheTimeToLiveData = new TimeSpanAppSettingsEntry("OrgPropertyCacheTimeToLiveSeconds", TimeSpanUnit.Seconds, TimeSpan.FromHours(1.0), ExTraceGlobals.SystemConfigurationCacheTracer);

		private static readonly TimeSpanAppSettingsEntry GlobalCountryListCacheTimeToLiveData = new TimeSpanAppSettingsEntry("GlobalCountryListCacheTimeToLiveSeconds", TimeSpanUnit.Seconds, TimeSpan.FromHours(6.0), ExTraceGlobals.SystemConfigurationCacheTracer);
	}
}
