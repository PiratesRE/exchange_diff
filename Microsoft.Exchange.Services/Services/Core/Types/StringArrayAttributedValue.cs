using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange", Name = "StringArrayAttributedValue")]
	[XmlType(TypeName = "StringArrayAttributedValueType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[Serializable]
	public class StringArrayAttributedValue
	{
		[XmlArray(ElementName = "Values", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
		[DataMember(IsRequired = true, Order = 1)]
		[XmlArrayItem("Value", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types", Type = typeof(string))]
		public string[] Values { get; set; }

		[XmlArray(ElementName = "Attributions", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
		[DataMember(IsRequired = true, Order = 2)]
		[XmlArrayItem("Attribution", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types", Type = typeof(string))]
		public string[] Attributions { get; set; }

		public StringArrayAttributedValue()
		{
		}

		public StringArrayAttributedValue(string[] value, string[] attributions)
		{
			this.Values = value;
			this.Attributions = attributions;
		}
	}
}
