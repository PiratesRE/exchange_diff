using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange", Name = "PhoneNumberAttributedValue")]
	[XmlType(TypeName = "PhoneNumberAttributedValue", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[Serializable]
	public class PhoneNumberAttributedValue
	{
		[DataMember(IsRequired = true, Order = 1)]
		[XmlElement]
		public PhoneNumber Value { get; set; }

		[XmlArrayItem("Attribution", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types", Type = typeof(string))]
		[XmlArray(ElementName = "Attributions", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
		[DataMember(IsRequired = true, Order = 2)]
		public string[] Attributions { get; set; }

		public PhoneNumberAttributedValue()
		{
		}

		public PhoneNumberAttributedValue(PhoneNumber value, string[] attributions)
		{
			this.Value = value;
			this.Attributions = attributions;
		}
	}
}
