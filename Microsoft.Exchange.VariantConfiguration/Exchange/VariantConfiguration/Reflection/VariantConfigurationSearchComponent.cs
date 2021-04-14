using System;
using Microsoft.Exchange.Search;

namespace Microsoft.Exchange.VariantConfiguration.Reflection
{
	public sealed class VariantConfigurationSearchComponent : VariantConfigurationComponent
	{
		internal VariantConfigurationSearchComponent() : base("Search")
		{
			base.Add(new VariantConfigurationSection("Search.settings.ini", "TransportFlowSettings", typeof(ITransportFlowSettings), false));
			base.Add(new VariantConfigurationSection("Search.settings.ini", "RequireMountedForCrawl", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("Search.settings.ini", "RemoveOrphanedCatalogs", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("Search.settings.ini", "IndexStatusInvalidateInterval", typeof(IIndexStatusSettings), false));
			base.Add(new VariantConfigurationSection("Search.settings.ini", "ProcessItemsWithNullCompositeId", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("Search.settings.ini", "InstantSearch", typeof(IInstantSearch), false));
			base.Add(new VariantConfigurationSection("Search.settings.ini", "CrawlerFeederUpdateCrawlingStatusResetCache", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("Search.settings.ini", "LanguageDetection", typeof(ILanguageDetection), false));
			base.Add(new VariantConfigurationSection("Search.settings.ini", "CachePreWarmingEnabled", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("Search.settings.ini", "MonitorDocumentValidationFailures", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("Search.settings.ini", "UseAlphaSchema", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("Search.settings.ini", "EnableIndexPartsCache", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("Search.settings.ini", "SchemaUpgrading", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("Search.settings.ini", "EnableGracefulDegradation", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("Search.settings.ini", "EnableIndexStatusTimestampVerification", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("Search.settings.ini", "EnableDynamicActivationPreference", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("Search.settings.ini", "UseExecuteAndReadPage", typeof(IFeature), true));
			base.Add(new VariantConfigurationSection("Search.settings.ini", "Completions", typeof(ICompletions), false));
			base.Add(new VariantConfigurationSection("Search.settings.ini", "UseBetaSchema", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("Search.settings.ini", "ReadFlag", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("Search.settings.ini", "EnableSingleValueRefiners", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("Search.settings.ini", "DocumentFeederSettings", typeof(IDocumentFeederSettings), false));
			base.Add(new VariantConfigurationSection("Search.settings.ini", "CrawlerFeederCollectDocumentsVerifyPendingWatermarks", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("Search.settings.ini", "MemoryModel", typeof(IMemoryModelSettings), false));
			base.Add(new VariantConfigurationSection("Search.settings.ini", "EnableTopN", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("Search.settings.ini", "FeederSettings", typeof(IFeederSettings), false));
			base.Add(new VariantConfigurationSection("Search.settings.ini", "WaitForMountPoints", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("Search.settings.ini", "EnableInstantSearch", typeof(IFeature), false));
		}

		public VariantConfigurationSection TransportFlowSettings
		{
			get
			{
				return base["TransportFlowSettings"];
			}
		}

		public VariantConfigurationSection RequireMountedForCrawl
		{
			get
			{
				return base["RequireMountedForCrawl"];
			}
		}

		public VariantConfigurationSection RemoveOrphanedCatalogs
		{
			get
			{
				return base["RemoveOrphanedCatalogs"];
			}
		}

		public VariantConfigurationSection IndexStatusInvalidateInterval
		{
			get
			{
				return base["IndexStatusInvalidateInterval"];
			}
		}

		public VariantConfigurationSection ProcessItemsWithNullCompositeId
		{
			get
			{
				return base["ProcessItemsWithNullCompositeId"];
			}
		}

		public VariantConfigurationSection InstantSearch
		{
			get
			{
				return base["InstantSearch"];
			}
		}

		public VariantConfigurationSection CrawlerFeederUpdateCrawlingStatusResetCache
		{
			get
			{
				return base["CrawlerFeederUpdateCrawlingStatusResetCache"];
			}
		}

		public VariantConfigurationSection LanguageDetection
		{
			get
			{
				return base["LanguageDetection"];
			}
		}

		public VariantConfigurationSection CachePreWarmingEnabled
		{
			get
			{
				return base["CachePreWarmingEnabled"];
			}
		}

		public VariantConfigurationSection MonitorDocumentValidationFailures
		{
			get
			{
				return base["MonitorDocumentValidationFailures"];
			}
		}

		public VariantConfigurationSection UseAlphaSchema
		{
			get
			{
				return base["UseAlphaSchema"];
			}
		}

		public VariantConfigurationSection EnableIndexPartsCache
		{
			get
			{
				return base["EnableIndexPartsCache"];
			}
		}

		public VariantConfigurationSection SchemaUpgrading
		{
			get
			{
				return base["SchemaUpgrading"];
			}
		}

		public VariantConfigurationSection EnableGracefulDegradation
		{
			get
			{
				return base["EnableGracefulDegradation"];
			}
		}

		public VariantConfigurationSection EnableIndexStatusTimestampVerification
		{
			get
			{
				return base["EnableIndexStatusTimestampVerification"];
			}
		}

		public VariantConfigurationSection EnableDynamicActivationPreference
		{
			get
			{
				return base["EnableDynamicActivationPreference"];
			}
		}

		public VariantConfigurationSection UseExecuteAndReadPage
		{
			get
			{
				return base["UseExecuteAndReadPage"];
			}
		}

		public VariantConfigurationSection Completions
		{
			get
			{
				return base["Completions"];
			}
		}

		public VariantConfigurationSection UseBetaSchema
		{
			get
			{
				return base["UseBetaSchema"];
			}
		}

		public VariantConfigurationSection ReadFlag
		{
			get
			{
				return base["ReadFlag"];
			}
		}

		public VariantConfigurationSection EnableSingleValueRefiners
		{
			get
			{
				return base["EnableSingleValueRefiners"];
			}
		}

		public VariantConfigurationSection DocumentFeederSettings
		{
			get
			{
				return base["DocumentFeederSettings"];
			}
		}

		public VariantConfigurationSection CrawlerFeederCollectDocumentsVerifyPendingWatermarks
		{
			get
			{
				return base["CrawlerFeederCollectDocumentsVerifyPendingWatermarks"];
			}
		}

		public VariantConfigurationSection MemoryModel
		{
			get
			{
				return base["MemoryModel"];
			}
		}

		public VariantConfigurationSection EnableTopN
		{
			get
			{
				return base["EnableTopN"];
			}
		}

		public VariantConfigurationSection FeederSettings
		{
			get
			{
				return base["FeederSettings"];
			}
		}

		public VariantConfigurationSection WaitForMountPoints
		{
			get
			{
				return base["WaitForMountPoints"];
			}
		}

		public VariantConfigurationSection EnableInstantSearch
		{
			get
			{
				return base["EnableInstantSearch"];
			}
		}
	}
}
