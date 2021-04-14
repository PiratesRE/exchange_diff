using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[XmlType(TypeName = "ExtendedPropertyAttributedValue", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange", Name = "ExtendedPropertyAttributedValue")]
	[Serializable]
	public class ExtendedPropertyAttributedValue
	{
		[XmlElement]
		[DataMember(IsRequired = true, Order = 1)]
		public ExtendedPropertyType Value { get; set; }

		[XmlArrayItem("Attribution", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types", Type = typeof(string))]
		[DataMember(IsRequired = true, Order = 2)]
		[XmlArray(ElementName = "Attributions", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
		public string[] Attributions { get; set; }

		public ExtendedPropertyAttributedValue()
		{
		}

		public ExtendedPropertyAttributedValue(ExtendedPropertyType value, string[] attributions)
		{
			this.Value = value;
			this.Attributions = attributions;
		}
	}
}
