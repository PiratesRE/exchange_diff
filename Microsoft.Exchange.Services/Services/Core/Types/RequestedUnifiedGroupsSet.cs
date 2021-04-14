using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using Microsoft.Exchange.Services.Core.Search;

namespace Microsoft.Exchange.Services.Core.Types
{
	[XmlType(TypeName = "RequestedUnifiedGroupsSet", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	[Serializable]
	public class RequestedUnifiedGroupsSet
	{
		[XmlElement("FilterType", typeof(UnifiedGroupsFilterType))]
		[DataMember(Name = "FilterType", EmitDefaultValue = false, IsRequired = true)]
		public UnifiedGroupsFilterType FilterType { get; set; }

		[XmlElement("SortType", typeof(UnifiedGroupsSortType))]
		[DataMember(Name = "SortType", EmitDefaultValue = false, IsRequired = false)]
		public UnifiedGroupsSortType SortType { get; set; }

		[DataMember(Name = "SortDirection", EmitDefaultValue = false, IsRequired = false)]
		[XmlElement("SortDirection", typeof(SortDirection))]
		public SortDirection SortDirection { get; set; }

		[DataMember(Name = "GroupsLimit", EmitDefaultValue = false, IsRequired = false)]
		[XmlElement("GroupsLimit", typeof(int))]
		public int GroupsLimit { get; set; }
	}
}
