using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange", Name = "EmailAddressAttributedValue")]
	[XmlType(TypeName = "EmailAddressAttributedValue", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[Serializable]
	public class EmailAddressAttributedValue
	{
		[XmlElement]
		[DataMember(IsRequired = true, Order = 1)]
		public EmailAddressWrapper Value { get; set; }

		[XmlArrayItem("Attribution", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types", Type = typeof(string))]
		[DataMember(IsRequired = true, Order = 2)]
		[XmlArray(ElementName = "Attributions", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
		public string[] Attributions { get; set; }

		public EmailAddressAttributedValue()
		{
		}

		public EmailAddressAttributedValue(EmailAddressWrapper value, string[] attributions)
		{
			this.Value = value;
			this.Attributions = attributions;
		}
	}
}
