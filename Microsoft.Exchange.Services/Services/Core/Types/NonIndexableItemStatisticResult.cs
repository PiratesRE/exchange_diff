using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[DataContract(Name = "NonIndexableItemStatisticResult", Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	[XmlType(TypeName = "NonIndexableItemStatisticType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[Serializable]
	public class NonIndexableItemStatisticResult
	{
		[DataMember(Name = "Mailbox", IsRequired = true)]
		[XmlElement("Mailbox")]
		public string Mailbox { get; set; }

		[XmlElement("ItemCount")]
		[DataMember(Name = "ItemCount", IsRequired = true)]
		public long ItemCount { get; set; }

		[DataMember(Name = "ErrorMessage", IsRequired = true)]
		[XmlElement("ErrorMessage")]
		public string ErrorMessage { get; set; }
	}
}
