using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange", Name = "PostalAddressAttributedValue")]
	[XmlType(TypeName = "PostalAddressAttributedValue", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[Serializable]
	public class PostalAddressAttributedValue
	{
		[XmlElement]
		[DataMember(IsRequired = true, Order = 1)]
		public PostalAddress Value { get; set; }

		[XmlArrayItem("Attribution", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types", Type = typeof(string))]
		[XmlArray(ElementName = "Attributions", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
		[DataMember(IsRequired = true, Order = 2)]
		public string[] Attributions { get; set; }

		public PostalAddressAttributedValue()
		{
		}

		public PostalAddressAttributedValue(PostalAddress value, string[] attributions)
		{
			this.Value = value;
			this.Attributions = attributions;
		}
	}
}
