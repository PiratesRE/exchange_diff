using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.Services.Core.Search
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	[XmlType(TypeName = "ContactsView", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[Serializable]
	public class ContactsPageView : BasePagingType
	{
		[XmlAttribute]
		[DataMember(Name = "InitialName", IsRequired = false)]
		public string InitialName { get; set; }

		[XmlAttribute]
		[DataMember(Name = "FinalName", IsRequired = false)]
		public string FinalName { get; set; }

		internal override QueryFilter ApplyQueryAppend(QueryFilter filter)
		{
			if (!string.IsNullOrEmpty(this.InitialName))
			{
				ComparisonFilter comparisonFilter = new ComparisonFilter(ComparisonOperator.GreaterThanOrEqual, StoreObjectSchema.DisplayName, this.InitialName);
				if (filter == null)
				{
					filter = comparisonFilter;
				}
				else
				{
					filter = new AndFilter(new QueryFilter[]
					{
						filter,
						comparisonFilter
					});
				}
			}
			if (!string.IsNullOrEmpty(this.FinalName))
			{
				ComparisonFilter comparisonFilter2 = new ComparisonFilter(ComparisonOperator.LessThanOrEqual, StoreObjectSchema.DisplayName, this.FinalName);
				if (filter == null)
				{
					filter = comparisonFilter2;
				}
				else
				{
					filter = new AndFilter(new QueryFilter[]
					{
						filter,
						comparisonFilter2
					});
				}
			}
			return filter;
		}
	}
}
