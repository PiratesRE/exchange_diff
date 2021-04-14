using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Threading;
using Microsoft.Exchange.EdgeSync.Common;

namespace Microsoft.Exchange.EdgeSync
{
	internal class EdgeSyncAppConfig
	{
		private EdgeSyncAppConfig()
		{
			string exePath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Microsoft.Exchange.EdgeSyncSvc.exe");
			this.config = ConfigurationManager.OpenExeConfiguration(exePath);
		}

		public bool DelayLdapEnabled
		{
			get
			{
				return this.delayLdapEnabled;
			}
		}

		public TimeSpan DelayLdapSearchRequest
		{
			get
			{
				return this.delayLdapSearchRequest;
			}
		}

		public TimeSpan DelayLdapUpdateRequest
		{
			get
			{
				return this.delayLdapUpdateRequest;
			}
		}

		public string DelayLdapUpdateRequestContainingString
		{
			get
			{
				return this.delayLdapUpdateRequestContainingString;
			}
		}

		public TimeSpan DelayStart
		{
			get
			{
				return this.delayStart;
			}
		}

		public SyncTreeType EnabledSyncType
		{
			get
			{
				return this.enabledSyncType;
			}
		}

		public List<SynchronizationProviderInfo> SynchronizationProviderList
		{
			get
			{
				return this.synchronizationProviderList;
			}
		}

		public long TenantSyncControlCacheSize
		{
			get
			{
				return this.tenantSyncControlCacheSize;
			}
		}

		public TimeSpan TenantSyncControlCacheExpiryInterval
		{
			get
			{
				return this.tenantSyncControlCacheExpiryInterval;
			}
		}

		public TimeSpan TenantSyncControlCacheCleanupInterval
		{
			get
			{
				return this.tenantSyncControlCacheCleanupInterval;
			}
		}

		public TimeSpan TenantSyncControlCachePurgeInterval
		{
			get
			{
				return this.tenantSyncControlCachePurgeInterval;
			}
		}

		public bool TrackDuplicatedAddEntries
		{
			get
			{
				return this.trackDuplicatedAddEntries;
			}
		}

		public int DuplicatedAddEntriesCacheSize
		{
			get
			{
				return this.duplicatedAddEntriesCacheSize;
			}
		}

		public int PodSiteStartRange
		{
			get
			{
				return this.podSiteStartRange;
			}
		}

		public int PodSiteEndRange
		{
			get
			{
				return this.podSiteEndRange;
			}
		}

		public static EdgeSyncAppConfig Load()
		{
			if (EdgeSyncAppConfig.instance == null)
			{
				lock (EdgeSyncAppConfig.initializationLock)
				{
					if (EdgeSyncAppConfig.instance == null)
					{
						EdgeSyncAppConfig edgeSyncAppConfig = new EdgeSyncAppConfig();
						edgeSyncAppConfig.delayStart = edgeSyncAppConfig.GetConfigTimeSpan("DelayStart", TimeSpan.FromSeconds(1.0), TimeSpan.MaxValue, TimeSpan.MinValue);
						edgeSyncAppConfig.enabledSyncType = (SyncTreeType)edgeSyncAppConfig.GetConfigEnum("EnabledSyncType", typeof(SyncTreeType), SyncTreeType.Configuration | SyncTreeType.Recipients);
						edgeSyncAppConfig.delayLdapEnabled = edgeSyncAppConfig.GetConfigBool("DelayLdapEnabled", false);
						if (edgeSyncAppConfig.delayLdapEnabled)
						{
							edgeSyncAppConfig.delayLdapSearchRequest = edgeSyncAppConfig.GetConfigTimeSpan("DelayLdapSearchRequest", TimeSpan.FromSeconds(1.0), TimeSpan.MaxValue, TimeSpan.MinValue);
							edgeSyncAppConfig.delayLdapUpdateRequest = edgeSyncAppConfig.GetConfigTimeSpan("DelayLdapUpdateRequest", TimeSpan.FromSeconds(1.0), TimeSpan.MaxValue, TimeSpan.MinValue);
							edgeSyncAppConfig.delayLdapUpdateRequestContainingString = edgeSyncAppConfig.GetConfigString("DelayLdapUpdateRequestContainingString", string.Empty);
						}
						edgeSyncAppConfig.tenantSyncControlCacheSize = edgeSyncAppConfig.GetConfigLong("TenantSyncControlCacheSize", 1024L, 1073741824L, 36700160L);
						edgeSyncAppConfig.tenantSyncControlCacheExpiryInterval = edgeSyncAppConfig.GetConfigTimeSpan("TenantSyncControlCacheExpiryInterval", TimeSpan.FromSeconds(1.0), TimeSpan.MaxValue, TimeSpan.FromHours(4.0));
						edgeSyncAppConfig.tenantSyncControlCacheCleanupInterval = edgeSyncAppConfig.GetConfigTimeSpan("TenantSyncControlCacheCleanupInterval", TimeSpan.FromSeconds(1.0), TimeSpan.MaxValue, TimeSpan.FromMinutes(15.0));
						edgeSyncAppConfig.tenantSyncControlCachePurgeInterval = edgeSyncAppConfig.GetConfigTimeSpan("TenantSyncControlCachePurgeInterval", TimeSpan.FromSeconds(1.0), TimeSpan.MaxValue, TimeSpan.FromMinutes(5.0));
						edgeSyncAppConfig.trackDuplicatedAddEntries = edgeSyncAppConfig.GetConfigBool("TrackDuplicatedAddEntries", true);
						edgeSyncAppConfig.duplicatedAddEntriesCacheSize = edgeSyncAppConfig.GetConfigInt("DuplicatedAddEntriesCacheSize", 1, int.MaxValue, 1500);
						edgeSyncAppConfig.podSiteStartRange = edgeSyncAppConfig.GetConfigInt("PodSiteStartRange", 0, int.MaxValue, 50000);
						edgeSyncAppConfig.podSiteEndRange = edgeSyncAppConfig.GetConfigInt("PodSiteEndRange", 0, int.MaxValue, 59999);
						edgeSyncAppConfig.synchronizationProviderList = edgeSyncAppConfig.LoadSynchronizationProviders();
						Thread.MemoryBarrier();
						EdgeSyncAppConfig.instance = edgeSyncAppConfig;
					}
				}
			}
			return EdgeSyncAppConfig.instance;
		}

		public int GetConfigInt(string label, int min, int max, int defaultValue)
		{
			if (max < min)
			{
				throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "Minimum must be smaller than or equal to Maximum (Config='{0}', Min='{1}', Max='{2}', Default='{3}').", new object[]
				{
					label,
					min,
					max,
					defaultValue
				}));
			}
			string text = (this.config.AppSettings.Settings[label] != null) ? this.config.AppSettings.Settings[label].Value : null;
			int num = defaultValue;
			if (string.IsNullOrEmpty(text) || !int.TryParse(text, out num) || num < min || num > max)
			{
				return defaultValue;
			}
			return num;
		}

		public long GetConfigLong(string label, long min, long max, long defaultValue)
		{
			if (max < min)
			{
				throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "Minimum must be smaller than or equal to Maximum (Config='{0}', Min='{1}', Max='{2}', Default='{3}').", new object[]
				{
					label,
					min,
					max,
					defaultValue
				}));
			}
			string text = (this.config.AppSettings.Settings[label] != null) ? this.config.AppSettings.Settings[label].Value : null;
			long num = defaultValue;
			if (string.IsNullOrEmpty(text) || !long.TryParse(text, out num) || num < min || num > max)
			{
				return defaultValue;
			}
			return num;
		}

		public TimeSpan GetConfigTimeSpan(string label, TimeSpan min, TimeSpan max, TimeSpan defaultValue)
		{
			if (max < min)
			{
				throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "Minimum must be smaller than or equal to Maximum (Config='{0}', Min='{1}', Max='{2}', Default='{3}').", new object[]
				{
					label,
					min,
					max,
					defaultValue
				}));
			}
			string text = (this.config.AppSettings.Settings[label] != null) ? this.config.AppSettings.Settings[label].Value : null;
			TimeSpan timeSpan = defaultValue;
			if (string.IsNullOrEmpty(text) || !TimeSpan.TryParse(text, out timeSpan) || timeSpan < min || timeSpan > max)
			{
				return defaultValue;
			}
			return timeSpan;
		}

		public string GetConfigString(string label, string defaultValue)
		{
			KeyValueConfigurationElement keyValueConfigurationElement = this.config.AppSettings.Settings[label];
			if (keyValueConfigurationElement != null)
			{
				return keyValueConfigurationElement.Value ?? defaultValue;
			}
			return defaultValue;
		}

		public bool GetConfigBool(string label, bool defaultValue)
		{
			KeyValueConfigurationElement keyValueConfigurationElement = this.config.AppSettings.Settings[label];
			if (keyValueConfigurationElement == null)
			{
				return defaultValue;
			}
			string value = keyValueConfigurationElement.Value;
			bool result = defaultValue;
			if (string.IsNullOrEmpty(value) || !bool.TryParse(value, out result))
			{
				return defaultValue;
			}
			return result;
		}

		protected List<SynchronizationProviderInfo> LoadSynchronizationProviders()
		{
			List<SynchronizationProviderInfo> list = new List<SynchronizationProviderInfo>();
			SyncProviderSection syncProviderSection = this.config.GetSection("SyncProviderSection") as SyncProviderSection;
			if (syncProviderSection != null)
			{
				foreach (object obj in syncProviderSection.SyncProviderElements)
				{
					SyncProviderElement syncProviderElement = (SyncProviderElement)obj;
					try
					{
						list.Add(new SynchronizationProviderInfo(syncProviderElement.Name, syncProviderElement.AssemblyPath, syncProviderElement.SynchronizationProvider, syncProviderElement.Enabled));
					}
					catch (ConfigurationErrorsException)
					{
					}
				}
			}
			return list;
		}

		private Enum GetConfigEnum(string label, Type enumType, Enum defaultValue)
		{
			string value = (this.config.AppSettings.Settings[label] != null) ? this.config.AppSettings.Settings[label].Value : null;
			if (string.IsNullOrEmpty(value))
			{
				return defaultValue;
			}
			Enum result;
			try
			{
				result = (Enum)Enum.Parse(enumType, value, true);
			}
			catch (ArgumentException)
			{
				return defaultValue;
			}
			return result;
		}

		private static object initializationLock = new object();

		private static EdgeSyncAppConfig instance;

		private TimeSpan delayStart;

		private SyncTreeType enabledSyncType;

		private bool delayLdapEnabled;

		private TimeSpan delayLdapSearchRequest;

		private TimeSpan delayLdapUpdateRequest;

		private string delayLdapUpdateRequestContainingString;

		private List<SynchronizationProviderInfo> synchronizationProviderList;

		private long tenantSyncControlCacheSize;

		private TimeSpan tenantSyncControlCacheExpiryInterval;

		private TimeSpan tenantSyncControlCacheCleanupInterval;

		private TimeSpan tenantSyncControlCachePurgeInterval;

		private bool trackDuplicatedAddEntries;

		private int duplicatedAddEntriesCacheSize;

		private int podSiteStartRange;

		private int podSiteEndRange;

		private Configuration config;
	}
}
