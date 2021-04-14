using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal abstract class AbstractFolder : AbstractStoreObject, IFolder, IStoreObject, IStorePropertyBag, IPropertyBag, IReadOnlyPropertyBag, IDisposable
	{
		public virtual string DisplayName
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		public virtual int ItemCount
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public virtual IQueryResult IFolderQuery(FolderQueryFlags queryFlags, QueryFilter queryFilter, SortBy[] sortColumns, params PropertyDefinition[] dataColumns)
		{
			throw new NotImplementedException();
		}

		public virtual IQueryResult IItemQuery(ItemQueryType queryFlags, QueryFilter queryFilter, SortBy[] sortColumns, ICollection<PropertyDefinition> dataColumns)
		{
			throw new NotImplementedException();
		}

		public IQueryResult IConversationItemQuery(QueryFilter queryFilter, SortBy[] sortColumns, ICollection<PropertyDefinition> dataColumns)
		{
			throw new NotImplementedException();
		}

		public IQueryResult IConversationMembersQuery(QueryFilter queryFilter, SortBy[] sortColumns, ICollection<PropertyDefinition> dataColumns)
		{
			throw new NotImplementedException();
		}

		public QueryResult GroupedItemQuery(QueryFilter queryFilter, ItemQueryType queryFlags, GroupByAndOrder[] groupBy, int expandCount, SortBy[] sortColumns, params PropertyDefinition[] dataColumns)
		{
			throw new NotImplementedException();
		}

		public virtual AggregateOperationResult DeleteObjects(DeleteItemFlags deleteFlags, params StoreId[] ids)
		{
			throw new NotImplementedException();
		}

		public virtual FolderSaveResult Save()
		{
			throw new NotImplementedException();
		}

		public FolderSaveResult Save(SaveMode saveMode)
		{
			throw new NotImplementedException();
		}

		public virtual IQueryResult PersonItemQuery(QueryFilter queryFilter, QueryFilter aggregationFilter, SortBy[] sortColumns, ICollection<PropertyDefinition> dataColumns, AggregationExtension aggregationExtension)
		{
			throw new NotImplementedException();
		}

		public virtual IQueryResult PersonItemQuery(QueryFilter queryFilter, QueryFilter aggregationFilter, SortBy[] sortColumns, ICollection<PropertyDefinition> dataColumns)
		{
			throw new NotImplementedException();
		}
	}
}
