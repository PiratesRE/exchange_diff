using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics.Components.Clients;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	internal static class ADCacheUtils
	{
		private static TenantConfigurationCache<OwaPerTenantTransportSettings> OwaPerTenantTransportSettingsCache
		{
			get
			{
				if (ADCacheUtils.owasPerTenantTransportSettingsCache == null)
				{
					lock (ADCacheUtils.lockObjectTransportSettings)
					{
						if (ADCacheUtils.owasPerTenantTransportSettingsCache == null)
						{
							ADCacheUtils.owasPerTenantTransportSettingsCache = new TenantConfigurationCache<OwaPerTenantTransportSettings>(ADCacheUtils.owaTransportSettingsCacheSize, ADCacheUtils.owaTransportSettingsCacheExpirationInterval, ADCacheUtils.owaTransportSettingsCacheCleanupInterval, new PerTenantCacheTracer(ExTraceGlobals.OwaPerTenantCacheTracer, "OwaPerTenantTransportSettings"), new PerTenantCachePerformanceCounters("OwaPerTenantTransportSettings"));
						}
					}
				}
				return ADCacheUtils.owasPerTenantTransportSettingsCache;
			}
		}

		private static TenantConfigurationCache<OwaPerTenantAcceptedDomains> OwaPerTenantAcceptedDomainsCache
		{
			get
			{
				if (ADCacheUtils.owasPerTenantAcceptedDomainsCache == null)
				{
					lock (ADCacheUtils.lockObjectAcceptedDomains)
					{
						if (ADCacheUtils.owasPerTenantAcceptedDomainsCache == null)
						{
							ADCacheUtils.owasPerTenantAcceptedDomainsCache = new TenantConfigurationCache<OwaPerTenantAcceptedDomains>(ADCacheUtils.owaAcceptedDomainsCacheSize, ADCacheUtils.owaRemoteDomainsCacheExpirationInterval, ADCacheUtils.owaRemoteDomainsCacheCleanupInterval, new PerTenantCacheTracer(ExTraceGlobals.OwaPerTenantCacheTracer, "OwaPerTenantAcceptedDomains"), new PerTenantCachePerformanceCounters("OwaPerTenantAcceptedDomains"));
						}
					}
				}
				return ADCacheUtils.owasPerTenantAcceptedDomainsCache;
			}
		}

		private static TenantConfigurationCache<OwaPerTenantRemoteDomains> OwaPerTenantRemoteDomainsCache
		{
			get
			{
				if (ADCacheUtils.owasPerTenantRemoteDomainsCache == null)
				{
					lock (ADCacheUtils.lockObjectRemoteDomains)
					{
						if (ADCacheUtils.owasPerTenantRemoteDomainsCache == null)
						{
							ADCacheUtils.owasPerTenantRemoteDomainsCache = new TenantConfigurationCache<OwaPerTenantRemoteDomains>(ADCacheUtils.owaRemoteDomainsCacheSize, ADCacheUtils.owaRemoteDomainsCacheExpirationInterval, ADCacheUtils.owaRemoteDomainsCacheCleanupInterval, new PerTenantCacheTracer(ExTraceGlobals.OwaPerTenantCacheTracer, "OwaPerTenantRemoteDomains"), new PerTenantCachePerformanceCounters("OwaPerTenantRemoteDomains"));
						}
					}
				}
				return ADCacheUtils.owasPerTenantRemoteDomainsCache;
			}
		}

		public static OwaPerTenantTransportSettings GetOwaPerTenantTransportSettings(OrganizationId organizationId)
		{
			return ADCacheUtils.OwaPerTenantTransportSettingsCache.GetValue(organizationId);
		}

		public static OwaPerTenantAcceptedDomains GetOwaPerTenantAcceptedDomains(OrganizationId organizationId)
		{
			return ADCacheUtils.OwaPerTenantAcceptedDomainsCache.GetValue(organizationId);
		}

		public static OwaPerTenantRemoteDomains GetOwaPerTenantRemoteDomains(OrganizationId organizationId)
		{
			return ADCacheUtils.OwaPerTenantRemoteDomainsCache.GetValue(organizationId);
		}

		private const string PerTenantCacheTransportSettingsName = "OwaPerTenantTransportSettings";

		private const string PerTenantCacheAcceptedDomainsName = "OwaPerTenantAcceptedDomains";

		private const string PerTenantCacheRemoteDomainsName = "OwaPerTenantRemoteDomains";

		private static long owaTransportSettingsCacheSize = BaseApplication.GetAppSetting<long>("OwaTransportSettingsCacheSize", 20L) * 1024L * 1024L;

		private static TimeSpan owaTransportSettingsCacheExpirationInterval = TimeSpan.FromMinutes((double)BaseApplication.GetAppSetting<int>("OwaTransportSettingsCacheExpirationInterval", 60));

		private static TimeSpan owaTransportSettingsCacheCleanupInterval = TimeSpan.FromMinutes((double)BaseApplication.GetAppSetting<int>("OwaTransportSettingsCacheCleanupInterval", 60));

		private static long owaAcceptedDomainsCacheSize = BaseApplication.GetAppSetting<long>("OwaAcceptedDomainsCacheSize", 20L) * 1024L * 1024L;

		private static TimeSpan owaAcceptedDomainsCacheExpirationInterval = TimeSpan.FromMinutes((double)BaseApplication.GetAppSetting<int>("OwaAcceptedDomainsCacheExpirationInterval", 60));

		private static TimeSpan owaAcceptedDomainsCacheCleanupInterval = TimeSpan.FromMinutes((double)BaseApplication.GetAppSetting<int>("OwaAcceptedDomainsCacheCleanupInterval", 60));

		private static long owaRemoteDomainsCacheSize = BaseApplication.GetAppSetting<long>("OwaRemoteDomainsCacheSize", 20L) * 1024L * 1024L;

		private static TimeSpan owaRemoteDomainsCacheExpirationInterval = TimeSpan.FromMinutes((double)BaseApplication.GetAppSetting<int>("OwaRemoteDomainsCacheExpirationInterval", 60));

		private static TimeSpan owaRemoteDomainsCacheCleanupInterval = TimeSpan.FromMinutes((double)BaseApplication.GetAppSetting<int>("OwaRemoteDomainsCacheCleanupInterval", 60));

		private static TenantConfigurationCache<OwaPerTenantTransportSettings> owasPerTenantTransportSettingsCache;

		private static TenantConfigurationCache<OwaPerTenantAcceptedDomains> owasPerTenantAcceptedDomainsCache;

		private static TenantConfigurationCache<OwaPerTenantRemoteDomains> owasPerTenantRemoteDomainsCache;

		private static readonly List<PropertyDefinition> PropertiesToGet = new List<PropertyDefinition>
		{
			ADRecipientSchema.EmailAddresses
		};

		private static object lockObjectTransportSettings = new object();

		private static object lockObjectAcceptedDomains = new object();

		private static object lockObjectRemoteDomains = new object();
	}
}
