using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Services.Core.DataConverter;
using Microsoft.Exchange.Services.Core.Search;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.Services.Diagnostics;

namespace Microsoft.Exchange.Services.Core
{
	internal abstract class SearchPeopleStrategy
	{
		protected BasePagingType Paging
		{
			get
			{
				return this.parameters.Paging;
			}
		}

		protected string QueryString
		{
			get
			{
				return this.parameters.QueryString;
			}
		}

		private protected SortBy[] SortBy { protected get; private set; }

		private protected QueryFilter RestrictionFilter { protected get; private set; }

		private protected StoreId SearchScope { protected get; private set; }

		internal SearchPeopleStrategy(FindPeopleParameters parameters, QueryFilter restrictionFilter, StoreId searchScope)
		{
			parameters.Paging.NoRowCountRetrieval = true;
			this.parameters = parameters;
			this.RestrictionFilter = restrictionFilter;
			this.SearchScope = searchScope;
			if (parameters.SortResults == null || parameters.SortResults.Length == 0)
			{
				this.SortBy = SearchPeopleStrategy.GetSortBy("persona:DisplayName");
				return;
			}
			this.SortBy = SearchPeopleStrategy.GetSortBy(parameters.SortResults);
		}

		public abstract Persona[] Execute();

		protected void Log(FindPeopleMetadata metadata, object value)
		{
			this.parameters.Logger.Set(metadata, value);
		}

		private static SortBy[] GetSortBy(string propertyToSortBy)
		{
			PropertyUriEnum uri;
			PropertyUriMapper.TryGetPropertyUriEnum(propertyToSortBy, out uri);
			PropertyPath propertyPath = new PropertyUri(uri);
			StorePropertyDefinition storePropertyDefinition = SearchPeopleStrategy.GetStorePropertyDefinition(propertyPath);
			return new SortBy[]
			{
				new SortBy(storePropertyDefinition, SortOrder.Ascending)
			};
		}

		private static SortBy[] GetSortBy(SortResults[] sortResults)
		{
			return (from sortResult in sortResults
			select new SortBy(SearchPeopleStrategy.GetStorePropertyDefinition(sortResult.SortByProperty), (SortOrder)sortResult.Order)).ToArray<SortBy>();
		}

		private static StorePropertyDefinition GetStorePropertyDefinition(PropertyPath propertyPath)
		{
			PropertyDefinition propertyDefinition;
			if (!SearchSchemaMap.TryGetPropertyDefinition(propertyPath, out propertyDefinition))
			{
				throw new UnsupportedPathForSortGroupException(propertyPath);
			}
			StorePropertyDefinition storePropertyDefinition = (StorePropertyDefinition)propertyDefinition;
			if ((storePropertyDefinition.Capabilities & StorePropertyCapabilities.CanSortBy) != StorePropertyCapabilities.CanSortBy)
			{
				throw new UnsupportedPathForSortGroupException(propertyPath);
			}
			return storePropertyDefinition;
		}

		private const string PersonDisplayNamePropertyURI = "persona:DisplayName";

		internal static readonly HashSet<PropertyPath> AdditionalSupportedProperties = new HashSet<PropertyPath>
		{
			PersonaSchema.ThirdPartyPhotoUrls.PropertyPath,
			PersonaSchema.Attributions.PropertyPath
		};

		protected FindPeopleParameters parameters;
	}
}
