using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[XmlType(TypeName = "BodyContentAttributedValue", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange", Name = "BodyContentAttributedValue")]
	[Serializable]
	public class BodyContentAttributedValue
	{
		[XmlElement]
		[DataMember(IsRequired = true, Order = 1)]
		public BodyContentType Value { get; set; }

		[XmlArrayItem("Attribution", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types", Type = typeof(string))]
		[DataMember(IsRequired = true, Order = 2)]
		[XmlArray(ElementName = "Attributions", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
		public string[] Attributions { get; set; }

		public BodyContentAttributedValue()
		{
		}

		public BodyContentAttributedValue(BodyContentType value, string[] attributions)
		{
			this.Value = value;
			this.Attributions = attributions;
		}
	}
}
