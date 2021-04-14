using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Core.Search
{
	[XmlType(TypeName = "GroupByType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	[Serializable]
	public class GroupByType : BaseGroupByType
	{
		public GroupByType()
		{
		}

		internal GroupByType(PropertyPath aggregateOnProperty, AggregateType aggregateType, PropertyPath groupByProperty, SortDirection sortDirection) : base(sortDirection)
		{
			this.groupByProperty = groupByProperty;
			this.AggregateOn = new AggregateOnType(aggregateOnProperty, aggregateType);
		}

		[XmlElement("ExtendedFieldURI", typeof(ExtendedPropertyUri), IsNullable = false)]
		[DataMember(Name = "GroupByProperty", Order = 1)]
		[XmlElement("IndexedFieldURI", typeof(DictionaryPropertyUri), IsNullable = false)]
		[XmlElement("FieldURI", typeof(PropertyUri), IsNullable = false)]
		public PropertyPath GroupByProperty
		{
			get
			{
				return this.groupByProperty;
			}
			set
			{
				this.groupByProperty = value;
			}
		}

		[XmlElement(ElementName = "AggregateOn", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
		[DataMember(Name = "AggregateOn", Order = 2)]
		public AggregateOnType AggregateOn
		{
			get
			{
				return this.aggregateOn;
			}
			set
			{
				this.aggregateOn = value;
			}
		}

		internal GroupSort ToGroupSort()
		{
			return new GroupSort(BaseGroupByType.PropertyPathToPropertyDefinition(this.aggregateOn.AggregationProperty), (SortOrder)base.SortDirection, (Aggregate)this.AggregateOn.Aggregate);
		}

		internal PropertyDefinition GroupByPropertyDefinition
		{
			get
			{
				if (this.groupByPropertyDefinition == null)
				{
					if (!SearchSchemaMap.TryGetPropertyDefinition(this.groupByProperty, out this.groupByPropertyDefinition))
					{
						throw new UnsupportedPathForSortGroupException(this.groupByProperty);
					}
					StorePropertyDefinition storePropertyDefinition = (StorePropertyDefinition)this.groupByPropertyDefinition;
					if ((storePropertyDefinition.Capabilities & StorePropertyCapabilities.CanGroupBy) != StorePropertyCapabilities.CanGroupBy)
					{
						throw new UnsupportedPathForSortGroupException(this.groupByProperty);
					}
				}
				return this.groupByPropertyDefinition;
			}
		}

		internal override BasePageResult IssueQuery(QueryFilter query, Folder folder, SortBy[] sortBy, BasePagingType paging, ItemQueryTraversal traversal, PropertyDefinition[] propsToFetch, RequestDetailsLogger logger)
		{
			BasePageResult result;
			using (GroupedQueryResult groupedQueryResult = folder.GroupedItemQuery(query, this.GroupByPropertyDefinition, this.ToGroupSort(), sortBy, propsToFetch))
			{
				int num = 0;
				while (num < propsToFetch.Length && propsToFetch[num] != this.GroupByPropertyDefinition)
				{
					num++;
				}
				result = BasePagingType.ApplyPostQueryGroupedPaging(groupedQueryResult, paging, num);
			}
			return result;
		}

		internal override PropertyDefinition[] GetAdditionalFetchProperties()
		{
			return new PropertyDefinition[]
			{
				this.GroupByPropertyDefinition
			};
		}

		internal override QueryType QueryType
		{
			get
			{
				return QueryType.Groups;
			}
		}

		private PropertyPath groupByProperty;

		private AggregateOnType aggregateOn;

		private PropertyDefinition groupByPropertyDefinition;
	}
}
