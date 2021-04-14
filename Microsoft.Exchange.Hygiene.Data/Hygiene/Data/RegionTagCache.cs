using System;
using System.Configuration;
using System.Reflection;
using System.Runtime.Caching;
using System.Threading;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Hygiene.Data
{
	internal class RegionTagCache
	{
		private RegionTagCache()
		{
			this.cache = new MemoryCache("RegionTag", null);
			RegionTagCache.RegionTagCacheConfiguration instance = RegionTagCache.RegionTagCacheConfiguration.GetInstance();
			this.goodTenantCachePolicy = new CacheItemPolicy();
			this.goodTenantEntryTTL = instance.TenantWithRegionTagEntryTTL;
			this.badTenantCachePolicy = new CacheItemPolicy();
			this.badTenantEntryTTL = instance.TenantWithoutRegionTagEntryTTL;
		}

		public long Count
		{
			get
			{
				return this.cache.GetCount(null);
			}
		}

		public long CacheMemoryLimit
		{
			get
			{
				return this.cache.CacheMemoryLimit;
			}
		}

		public long PhysicalMemoryLimit
		{
			get
			{
				return this.cache.PhysicalMemoryLimit;
			}
		}

		public static RegionTagCache GetInstance()
		{
			if (RegionTagCache.singleton == null)
			{
				lock (RegionTagCache.singletonLock)
				{
					if (RegionTagCache.singleton == null)
					{
						RegionTagCache regionTagCache = new RegionTagCache();
						Thread.MemoryBarrier();
						RegionTagCache.singleton = regionTagCache;
					}
				}
			}
			return RegionTagCache.singleton;
		}

		public void AddGoodTenant(Guid tenantId, string regionTag)
		{
			ArgumentValidator.ThrowIfNullOrEmpty("regionTag", regionTag);
			this.goodTenantCachePolicy.AbsoluteExpiration = new DateTimeOffset(DateTime.UtcNow.Add(this.goodTenantEntryTTL));
			this.cache.Set(tenantId.ToString(), regionTag, this.goodTenantCachePolicy, null);
		}

		public void AddBadTenant(Guid tenantId)
		{
			this.badTenantCachePolicy.AbsoluteExpiration = new DateTimeOffset(DateTime.UtcNow.Add(this.badTenantEntryTTL));
			this.cache.Set(tenantId.ToString(), string.Empty, this.badTenantCachePolicy, null);
		}

		public string Get(Guid tenantId)
		{
			string key = tenantId.ToString();
			if (this.cache.Contains(key, null))
			{
				return (string)this.cache[key];
			}
			return null;
		}

		private const string Name = "RegionTag";

		private static RegionTagCache singleton;

		private static object singletonLock = new object();

		private readonly TimeSpan goodTenantEntryTTL;

		private readonly TimeSpan badTenantEntryTTL;

		private MemoryCache cache;

		private CacheItemPolicy goodTenantCachePolicy;

		private CacheItemPolicy badTenantCachePolicy;

		private class RegionTagCacheConfiguration : ConfigurationSection
		{
			[ConfigurationProperty("TenantWithRegionTagEntryTTL", IsRequired = false, DefaultValue = "1.00:00:00")]
			public TimeSpan TenantWithRegionTagEntryTTL
			{
				get
				{
					return (TimeSpan)base["TenantWithRegionTagEntryTTL"];
				}
				internal set
				{
					base["TenantWithRegionTagEntryTTL"] = value;
				}
			}

			[ConfigurationProperty("TenantWithoutRegionTagEntryTTL", IsRequired = false, DefaultValue = "00:15:00")]
			public TimeSpan TenantWithoutRegionTagEntryTTL
			{
				get
				{
					return (TimeSpan)base["TenantWithoutRegionTagEntryTTL"];
				}
				internal set
				{
					base["TenantWithoutRegionTagEntryTTL"] = value;
				}
			}

			public static RegionTagCache.RegionTagCacheConfiguration GetInstance()
			{
				if (RegionTagCache.RegionTagCacheConfiguration.instance == null)
				{
					RegionTagCache.RegionTagCacheConfiguration.instance = (RegionTagCache.RegionTagCacheConfiguration)ConfigurationManager.GetSection("RegionTagCache");
					if (RegionTagCache.RegionTagCacheConfiguration.instance == null)
					{
						string exeConfigFilename = Uri.UnescapeDataString(new UriBuilder(Assembly.GetExecutingAssembly().CodeBase).Path) + ".config";
						ExeConfigurationFileMap fileMap = new ExeConfigurationFileMap
						{
							ExeConfigFilename = exeConfigFilename
						};
						Configuration configuration = ConfigurationManager.OpenMappedExeConfiguration(fileMap, ConfigurationUserLevel.None);
						RegionTagCache.RegionTagCacheConfiguration.instance = (RegionTagCache.RegionTagCacheConfiguration)configuration.GetSection("RegionTagCache");
					}
					if (RegionTagCache.RegionTagCacheConfiguration.instance == null)
					{
						RegionTagCache.RegionTagCacheConfiguration.instance = new RegionTagCache.RegionTagCacheConfiguration();
					}
				}
				return RegionTagCache.RegionTagCacheConfiguration.instance;
			}

			private const string TenantWithRegionTagEntryExpirationParam = "TenantWithRegionTagEntryTTL";

			private const string TenantWithoutRegionTagEntryExpirationParam = "TenantWithoutRegionTagEntryTTL";

			private const string SectionName = "RegionTagCache";

			private static RegionTagCache.RegionTagCacheConfiguration instance;
		}
	}
}
