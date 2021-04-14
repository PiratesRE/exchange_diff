using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange", Name = "StringAttributedValue")]
	[XmlType(TypeName = "StringAttributedValueType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[Serializable]
	public class StringAttributedValue
	{
		[XmlElement]
		[DataMember(IsRequired = true, Order = 1)]
		public string Value { get; set; }

		[XmlArray(ElementName = "Attributions", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
		[XmlArrayItem("Attribution", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types", Type = typeof(string))]
		[DataMember(IsRequired = true, Order = 2)]
		public string[] Attributions { get; set; }

		public StringAttributedValue()
		{
		}

		public StringAttributedValue(string value, string[] attributions)
		{
			this.Value = value;
			this.Attributions = attributions;
		}
	}
}
