using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[DataContract(Name = "SearchRefinerItem", Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	[XmlType(TypeName = "SearchRefinerItemType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[Serializable]
	public class SearchRefinerItem
	{
		[XmlElement("Name")]
		[DataMember(Name = "Name", IsRequired = true)]
		public string Name { get; set; }

		[XmlElement("Value")]
		[DataMember(Name = "Value", IsRequired = true)]
		public string Value { get; set; }

		[XmlElement("Count")]
		[DataMember(Name = "Count", IsRequired = true)]
		public long Count { get; set; }

		[XmlElement("Token")]
		[DataMember(Name = "Token", IsRequired = true)]
		public string Token { get; set; }
	}
}
