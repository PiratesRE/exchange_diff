using System;
using Microsoft.Exchange.Flighting;

namespace Microsoft.Exchange.VariantConfiguration.Reflection
{
	public sealed class VariantConfigurationDiscoveryComponent : VariantConfigurationComponent
	{
		internal VariantConfigurationDiscoveryComponent() : base("Discovery")
		{
			base.Add(new VariantConfigurationSection("Discovery.settings.ini", "DiscoveryServerLookupConcurrency", typeof(ISettingsValue), false));
			base.Add(new VariantConfigurationSection("Discovery.settings.ini", "DiscoveryMaxAllowedExecutorItems", typeof(ISettingsValue), false));
			base.Add(new VariantConfigurationSection("Discovery.settings.ini", "DiscoveryKeywordsBatchSize", typeof(ISettingsValue), false));
			base.Add(new VariantConfigurationSection("Discovery.settings.ini", "DiscoveryExecutesInParallel", typeof(ISettingsValue), false));
			base.Add(new VariantConfigurationSection("Discovery.settings.ini", "UrlRebind", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("Discovery.settings.ini", "DiscoveryDisplaySearchPageSize", typeof(ISettingsValue), false));
			base.Add(new VariantConfigurationSection("Discovery.settings.ini", "DiscoveryLocalSearchConcurrency", typeof(ISettingsValue), false));
			base.Add(new VariantConfigurationSection("Discovery.settings.ini", "SearchTimeout", typeof(ISettingsValue), false));
			base.Add(new VariantConfigurationSection("Discovery.settings.ini", "ServiceTopologyTimeout", typeof(ISettingsValue), false));
			base.Add(new VariantConfigurationSection("Discovery.settings.ini", "DiscoveryDisplaySearchBatchSize", typeof(ISettingsValue), false));
			base.Add(new VariantConfigurationSection("Discovery.settings.ini", "DiscoveryDefaultPageSize", typeof(ISettingsValue), false));
			base.Add(new VariantConfigurationSection("Discovery.settings.ini", "DiscoveryServerLookupBatch", typeof(ISettingsValue), false));
			base.Add(new VariantConfigurationSection("Discovery.settings.ini", "DiscoveryMaxAllowedResultsPageSize", typeof(ISettingsValue), false));
			base.Add(new VariantConfigurationSection("Discovery.settings.ini", "SearchScale", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("Discovery.settings.ini", "MailboxServerLocatorTimeout", typeof(ISettingsValue), false));
			base.Add(new VariantConfigurationSection("Discovery.settings.ini", "DiscoveryADPageSize", typeof(ISettingsValue), false));
			base.Add(new VariantConfigurationSection("Discovery.settings.ini", "DiscoveryMailboxMaxProhibitSendReceiveQuota", typeof(ISettingsValue), false));
			base.Add(new VariantConfigurationSection("Discovery.settings.ini", "DiscoveryFanoutConcurrency", typeof(ISettingsValue), false));
			base.Add(new VariantConfigurationSection("Discovery.settings.ini", "DiscoveryExcludedFolders", typeof(ISettingsValue), false));
			base.Add(new VariantConfigurationSection("Discovery.settings.ini", "DiscoveryUseFastSearch", typeof(ISettingsValue), false));
			base.Add(new VariantConfigurationSection("Discovery.settings.ini", "DiscoveryFanoutBatch", typeof(ISettingsValue), false));
			base.Add(new VariantConfigurationSection("Discovery.settings.ini", "DiscoveryLocalSearchIsParallel", typeof(ISettingsValue), false));
			base.Add(new VariantConfigurationSection("Discovery.settings.ini", "DiscoveryAggregateLogs", typeof(ISettingsValue), false));
			base.Add(new VariantConfigurationSection("Discovery.settings.ini", "DiscoveryMailboxMaxProhibitSendQuota", typeof(ISettingsValue), false));
			base.Add(new VariantConfigurationSection("Discovery.settings.ini", "DiscoveryMaxAllowedMailboxQueriesPerRequest", typeof(ISettingsValue), false));
			base.Add(new VariantConfigurationSection("Discovery.settings.ini", "DiscoveryMaxMailboxes", typeof(ISettingsValue), false));
			base.Add(new VariantConfigurationSection("Discovery.settings.ini", "DiscoveryADLookupConcurrency", typeof(ISettingsValue), false));
			base.Add(new VariantConfigurationSection("Discovery.settings.ini", "DiscoveryExcludedFoldersEnabled", typeof(ISettingsValue), false));
		}

		public VariantConfigurationSection DiscoveryServerLookupConcurrency
		{
			get
			{
				return base["DiscoveryServerLookupConcurrency"];
			}
		}

		public VariantConfigurationSection DiscoveryMaxAllowedExecutorItems
		{
			get
			{
				return base["DiscoveryMaxAllowedExecutorItems"];
			}
		}

		public VariantConfigurationSection DiscoveryKeywordsBatchSize
		{
			get
			{
				return base["DiscoveryKeywordsBatchSize"];
			}
		}

		public VariantConfigurationSection DiscoveryExecutesInParallel
		{
			get
			{
				return base["DiscoveryExecutesInParallel"];
			}
		}

		public VariantConfigurationSection UrlRebind
		{
			get
			{
				return base["UrlRebind"];
			}
		}

		public VariantConfigurationSection DiscoveryDisplaySearchPageSize
		{
			get
			{
				return base["DiscoveryDisplaySearchPageSize"];
			}
		}

		public VariantConfigurationSection DiscoveryLocalSearchConcurrency
		{
			get
			{
				return base["DiscoveryLocalSearchConcurrency"];
			}
		}

		public VariantConfigurationSection SearchTimeout
		{
			get
			{
				return base["SearchTimeout"];
			}
		}

		public VariantConfigurationSection ServiceTopologyTimeout
		{
			get
			{
				return base["ServiceTopologyTimeout"];
			}
		}

		public VariantConfigurationSection DiscoveryDisplaySearchBatchSize
		{
			get
			{
				return base["DiscoveryDisplaySearchBatchSize"];
			}
		}

		public VariantConfigurationSection DiscoveryDefaultPageSize
		{
			get
			{
				return base["DiscoveryDefaultPageSize"];
			}
		}

		public VariantConfigurationSection DiscoveryServerLookupBatch
		{
			get
			{
				return base["DiscoveryServerLookupBatch"];
			}
		}

		public VariantConfigurationSection DiscoveryMaxAllowedResultsPageSize
		{
			get
			{
				return base["DiscoveryMaxAllowedResultsPageSize"];
			}
		}

		public VariantConfigurationSection SearchScale
		{
			get
			{
				return base["SearchScale"];
			}
		}

		public VariantConfigurationSection MailboxServerLocatorTimeout
		{
			get
			{
				return base["MailboxServerLocatorTimeout"];
			}
		}

		public VariantConfigurationSection DiscoveryADPageSize
		{
			get
			{
				return base["DiscoveryADPageSize"];
			}
		}

		public VariantConfigurationSection DiscoveryMailboxMaxProhibitSendReceiveQuota
		{
			get
			{
				return base["DiscoveryMailboxMaxProhibitSendReceiveQuota"];
			}
		}

		public VariantConfigurationSection DiscoveryFanoutConcurrency
		{
			get
			{
				return base["DiscoveryFanoutConcurrency"];
			}
		}

		public VariantConfigurationSection DiscoveryExcludedFolders
		{
			get
			{
				return base["DiscoveryExcludedFolders"];
			}
		}

		public VariantConfigurationSection DiscoveryUseFastSearch
		{
			get
			{
				return base["DiscoveryUseFastSearch"];
			}
		}

		public VariantConfigurationSection DiscoveryFanoutBatch
		{
			get
			{
				return base["DiscoveryFanoutBatch"];
			}
		}

		public VariantConfigurationSection DiscoveryLocalSearchIsParallel
		{
			get
			{
				return base["DiscoveryLocalSearchIsParallel"];
			}
		}

		public VariantConfigurationSection DiscoveryAggregateLogs
		{
			get
			{
				return base["DiscoveryAggregateLogs"];
			}
		}

		public VariantConfigurationSection DiscoveryMailboxMaxProhibitSendQuota
		{
			get
			{
				return base["DiscoveryMailboxMaxProhibitSendQuota"];
			}
		}

		public VariantConfigurationSection DiscoveryMaxAllowedMailboxQueriesPerRequest
		{
			get
			{
				return base["DiscoveryMaxAllowedMailboxQueriesPerRequest"];
			}
		}

		public VariantConfigurationSection DiscoveryMaxMailboxes
		{
			get
			{
				return base["DiscoveryMaxMailboxes"];
			}
		}

		public VariantConfigurationSection DiscoveryADLookupConcurrency
		{
			get
			{
				return base["DiscoveryADLookupConcurrency"];
			}
		}

		public VariantConfigurationSection DiscoveryExcludedFoldersEnabled
		{
			get
			{
				return base["DiscoveryExcludedFoldersEnabled"];
			}
		}
	}
}
