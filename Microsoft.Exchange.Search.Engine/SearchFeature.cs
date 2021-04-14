using System;
using System.Collections.Generic;
using Microsoft.Exchange.Search.Mdb;
using Microsoft.Exchange.Search.OperatorSchema;

namespace Microsoft.Exchange.Search.Engine
{
	internal class SearchFeature
	{
		internal SearchFeature(SearchFeature.SearchFeatureType searchFeatureType, ISearchServiceConfig config, MdbInfo mdbInfo)
		{
			this.searchFeatureType = searchFeatureType;
			this.config = config;
			this.mdbInfo = mdbInfo;
		}

		public static IReadOnlyList<SearchFeature.SearchFeatureType> SearchFeatureTypeList
		{
			get
			{
				return SearchFeature.searchFeatureTypeList;
			}
		}

		internal bool IsFlipAllowed()
		{
			switch (this.searchFeatureType)
			{
			case SearchFeature.SearchFeatureType.InstantSearch:
				return !this.config.DisableGracefulDegradationForInstantSearch && this.mdbInfo.MountedOnLocalServer;
			case SearchFeature.SearchFeatureType.PassiveCatalog:
				return !this.config.DisableGracefulDegradationForAutoSuspend && (!this.mdbInfo.MountedOnLocalServer && !this.mdbInfo.IsSuspended) && this.mdbInfo.NumberOfCopies >= 3;
			default:
				throw new InvalidOperationException("searchFeatureType");
			}
		}

		internal long GetPotentialMemoryChange()
		{
			switch (this.searchFeatureType)
			{
			case SearchFeature.SearchFeatureType.InstantSearch:
				return (long)((float)(this.config.InstantSearchCostPerActiveItem * this.mdbInfo.NumberOfItems * (this.mdbInfo.IsInstantSearchEnabled ? -1L : 1L)) * SearchMemoryModel.MemoryUsageAdjustmentMultiplier);
			case SearchFeature.SearchFeatureType.PassiveCatalog:
				return (long)((float)(this.config.BaselineCostPerPassiveItem * this.mdbInfo.NumberOfItems * (this.mdbInfo.IsCatalogSuspended ? 1L : -1L)) * SearchMemoryModel.MemoryUsageAdjustmentMultiplier);
			default:
				throw new InvalidOperationException("searchFeatureType");
			}
		}

		private static readonly IReadOnlyList<SearchFeature.SearchFeatureType> searchFeatureTypeList = (IReadOnlyList<SearchFeature.SearchFeatureType>)Enum.GetValues(typeof(SearchFeature.SearchFeatureType));

		private SearchFeature.SearchFeatureType searchFeatureType;

		private ISearchServiceConfig config;

		private MdbInfo mdbInfo;

		internal enum SearchFeatureType
		{
			InstantSearch,
			PassiveCatalog
		}
	}
}
