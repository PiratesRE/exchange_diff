using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Core.Search
{
	[XmlType(TypeName = "FieldOrderType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	[Serializable]
	public class SortResults
	{
		[XmlElement("IndexedFieldURI", typeof(DictionaryPropertyUri))]
		[DataMember(Name = "Path")]
		[XmlElement("FieldURI", typeof(PropertyUri))]
		[XmlElement("ExtendedFieldURI", typeof(ExtendedPropertyUri))]
		[XmlElement("Path")]
		public PropertyPath SortByProperty { get; set; }

		[IgnoreDataMember]
		[XmlAttribute]
		public SortDirection Order { get; set; }

		[XmlIgnore]
		[DataMember(Name = "Order", IsRequired = true)]
		public string OrderString
		{
			get
			{
				return EnumUtilities.ToString<SortDirection>(this.Order);
			}
			set
			{
				this.Order = EnumUtilities.Parse<SortDirection>(value);
			}
		}

		internal static SortBy[] ToXsoSortBy(params SortResults[] sortResults)
		{
			if (sortResults == null)
			{
				return null;
			}
			SortBy[] array = new SortBy[sortResults.Length];
			for (int i = 0; i < sortResults.Length; i++)
			{
				SortResults sortResults2 = sortResults[i];
				PropertyDefinition propertyDefinition;
				if (!SearchSchemaMap.TryGetPropertyDefinition(sortResults2.SortByProperty, out propertyDefinition))
				{
					throw new UnsupportedPathForSortGroupException(sortResults2.SortByProperty);
				}
				StorePropertyDefinition storePropertyDefinition = (StorePropertyDefinition)propertyDefinition;
				if ((storePropertyDefinition.Capabilities & StorePropertyCapabilities.CanSortBy) != StorePropertyCapabilities.CanSortBy)
				{
					throw new UnsupportedPathForSortGroupException(sortResults2.SortByProperty);
				}
				SortOrder order = (SortOrder)sortResults2.Order;
				array[i] = new SortBy(storePropertyDefinition, order);
			}
			return array;
		}

		internal static SortResults[] FromXsoSortBy(params SortBy[] sortBys)
		{
			if (sortBys == null)
			{
				return null;
			}
			SortResults[] array = new SortResults[sortBys.Length];
			for (int i = 0; i < sortBys.Length; i++)
			{
				SortBy sortBy = sortBys[i];
				PropertyPath sortByProperty;
				if (!SearchSchemaMap.TryGetPropertyPath(sortBy.ColumnDefinition, out sortByProperty))
				{
					throw new UnsupportedPropertyDefinitionException(sortBy.ColumnDefinition.Name);
				}
				SortResults sortResults = new SortResults
				{
					SortByProperty = sortByProperty,
					Order = (SortDirection)sortBy.SortOrder
				};
				array[i] = sortResults;
			}
			return array;
		}
	}
}
