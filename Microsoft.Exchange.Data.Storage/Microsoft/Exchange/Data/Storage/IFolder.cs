using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal interface IFolder : IStoreObject, IStorePropertyBag, IPropertyBag, IReadOnlyPropertyBag, IDisposable
	{
		string DisplayName { get; set; }

		int ItemCount { get; }

		IQueryResult IFolderQuery(FolderQueryFlags queryFlags, QueryFilter queryFilter, SortBy[] sortColumns, params PropertyDefinition[] dataColumns);

		IQueryResult IItemQuery(ItemQueryType queryFlags, QueryFilter queryFilter, SortBy[] sortColumns, ICollection<PropertyDefinition> dataColumns);

		IQueryResult IConversationItemQuery(QueryFilter queryFilter, SortBy[] sortColumns, ICollection<PropertyDefinition> dataColumns);

		IQueryResult IConversationMembersQuery(QueryFilter queryFilter, SortBy[] sortColumns, ICollection<PropertyDefinition> dataColumns);

		QueryResult GroupedItemQuery(QueryFilter queryFilter, ItemQueryType itemQueryType, GroupByAndOrder[] groupBys, int expandCount, SortBy[] sortColumns, params PropertyDefinition[] dataColumns);

		AggregateOperationResult DeleteObjects(DeleteItemFlags deleteFlags, params StoreId[] ids);

		FolderSaveResult Save();

		FolderSaveResult Save(SaveMode saveMode);

		IQueryResult PersonItemQuery(QueryFilter queryFilter, QueryFilter aggregationFilter, SortBy[] sortColumns, ICollection<PropertyDefinition> dataColumns, AggregationExtension aggregationExtension);

		IQueryResult PersonItemQuery(QueryFilter queryFilter, QueryFilter aggregationFilter, SortBy[] sortColumns, ICollection<PropertyDefinition> dataColumns);
	}
}
