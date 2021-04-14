using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Core.Search
{
	[XmlInclude(typeof(DistinguishedGroupByType))]
	[KnownType(typeof(GroupByType))]
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	[XmlInclude(typeof(GroupByType))]
	[XmlType(TypeName = "BaseGroupByType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[KnownType(typeof(DistinguishedGroupByType))]
	[KnownType(typeof(NoGrouping))]
	[Serializable]
	public abstract class BaseGroupByType
	{
		public BaseGroupByType()
		{
		}

		internal BaseGroupByType(SortDirection sortDirection)
		{
			this.sortDirection = sortDirection;
		}

		[XmlAttribute(AttributeName = "Order")]
		[IgnoreDataMember]
		public SortDirection SortDirection
		{
			get
			{
				return this.sortDirection;
			}
			set
			{
				this.sortDirection = value;
			}
		}

		[XmlIgnore]
		[DataMember(Name = "Order", EmitDefaultValue = false)]
		public string SortDirectionString
		{
			get
			{
				return EnumUtilities.ToString<SortDirection>(this.SortDirection);
			}
			set
			{
				this.SortDirection = EnumUtilities.Parse<SortDirection>(value);
			}
		}

		internal static PropertyDefinition PropertyPathToPropertyDefinition(PropertyPath propertyPath)
		{
			if (propertyPath == null)
			{
				return null;
			}
			PropertyDefinition propertyDefinition;
			if (!SearchSchemaMap.TryGetPropertyDefinition(propertyPath, out propertyDefinition))
			{
				throw new UnsupportedPathForSortGroupException(propertyPath);
			}
			StorePropertyDefinition storePropertyDefinition = (StorePropertyDefinition)propertyDefinition;
			if ((storePropertyDefinition.Capabilities & StorePropertyCapabilities.CanGroupBy) != StorePropertyCapabilities.CanGroupBy)
			{
				throw new UnsupportedPathForSortGroupException(propertyPath);
			}
			return propertyDefinition;
		}

		internal abstract BasePageResult IssueQuery(QueryFilter query, Folder folder, SortBy[] sortBy, BasePagingType paging, ItemQueryTraversal traversal, PropertyDefinition[] propsToFetch, RequestDetailsLogger logger);

		internal abstract PropertyDefinition[] GetAdditionalFetchProperties();

		internal abstract QueryType QueryType { get; }

		private SortDirection sortDirection;
	}
}
