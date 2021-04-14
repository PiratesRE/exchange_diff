using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[XmlType(TypeName = "UnifiedGroupsSet", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[DataContract(Name = "UnifiedGroupsSet", Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	[Serializable]
	public class UnifiedGroupsSet
	{
		[XmlElement("FilterType", typeof(UnifiedGroupsFilterType))]
		[DataMember(Name = "FilterType", EmitDefaultValue = false)]
		public UnifiedGroupsFilterType FilterType { get; set; }

		[XmlElement("TotalGroups", typeof(int))]
		[DataMember(Name = "TotalGroups", EmitDefaultValue = false)]
		public int TotalGroups { get; set; }

		[XmlArray("Groups")]
		[XmlArrayItem("UnifiedGroup", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types", Type = typeof(UnifiedGroup))]
		[DataMember(Name = "Groups", EmitDefaultValue = false)]
		public UnifiedGroup[] Groups { get; set; }
	}
}
