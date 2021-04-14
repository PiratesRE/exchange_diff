using System;
using System.Configuration;

namespace Microsoft.Exchange.Hygiene.Cache.Data
{
	internal class EntityCacheConfiguration : ConfigurationElement
	{
		[ConfigurationProperty("Name", IsKey = true, IsRequired = true)]
		public string Name
		{
			get
			{
				return (string)base["Name"];
			}
			set
			{
				base["Name"] = value;
			}
		}

		[ConfigurationProperty("MaxCapacity", IsRequired = false, DefaultValue = 100000)]
		public int MaxCapacity
		{
			get
			{
				return (int)base["MaxCapacity"];
			}
			set
			{
				base["MaxCapacity"] = value;
			}
		}

		[ConfigurationProperty("TenantBatchSize", IsRequired = false, DefaultValue = 100)]
		public int TenantBatchSize
		{
			get
			{
				return (int)base["TenantBatchSize"];
			}
			set
			{
				base["TenantBatchSize"] = value;
			}
		}

		[ConfigurationProperty("ItemRefreshInterval", IsRequired = false, DefaultValue = "00:30:00")]
		public TimeSpan ItemRefreshInterval
		{
			get
			{
				return (TimeSpan)base["ItemRefreshInterval"];
			}
			set
			{
				base["ItemRefreshInterval"] = value;
			}
		}

		[ConfigurationProperty("MaxUnusedInterval", IsRequired = false, DefaultValue = "1.00:00:00")]
		public TimeSpan MaxUnusedInterval
		{
			get
			{
				return (TimeSpan)base["MaxUnusedInterval"];
			}
			set
			{
				base["MaxUnusedInterval"] = value;
			}
		}

		[ConfigurationProperty("MaxItemRefreshStaggerInterval", IsRequired = false, DefaultValue = "00:05:00")]
		public TimeSpan MaxItemRefreshStaggerInterval
		{
			get
			{
				return (TimeSpan)base["MaxItemRefreshStaggerInterval"];
			}
			set
			{
				base["MaxItemRefreshStaggerInterval"] = value;
			}
		}

		[ConfigurationProperty("RefreshThreadSleepInterval", IsRequired = false, DefaultValue = "00:00:30")]
		public TimeSpan RefreshThreadSleepInterval
		{
			get
			{
				return (TimeSpan)base["RefreshThreadSleepInterval"];
			}
			set
			{
				base["RefreshThreadSleepInterval"] = value;
			}
		}

		[ConfigurationProperty("Serialize", IsRequired = false, DefaultValue = false)]
		public bool Serialize
		{
			get
			{
				return (bool)base["Serialize"];
			}
			set
			{
				base["Serialize"] = value;
			}
		}

		[ConfigurationProperty("PrepopulateTenants", IsRequired = false, DefaultValue = true)]
		public bool PrepopulateTenants
		{
			get
			{
				return (bool)base["PrepopulateTenants"];
			}
			set
			{
				base["PrepopulateTenants"] = value;
			}
		}

		private const string NameKey = "Name";

		private const string MaxCapacityKey = "MaxCapacity";

		private const string TenantBatchSizeKey = "TenantBatchSize";

		private const string ItemRefreshIntervalKey = "ItemRefreshInterval";

		private const string MaxUnusedIntervalKey = "MaxUnusedInterval";

		private const string MaxItemRefreshStaggerIntervalKey = "MaxItemRefreshStaggerInterval";

		private const string RefreshThreadSleepIntervalKey = "RefreshThreadSleepInterval";

		private const string SerializeKey = "Serialize";

		private const string PrepopulateTenantsKey = "PrepopulateTenants";
	}
}
