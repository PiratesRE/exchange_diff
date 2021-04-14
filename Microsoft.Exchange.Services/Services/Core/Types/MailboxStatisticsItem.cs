using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[DataContract(Name = "MailboxStatisticsItem", Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	[XmlType(TypeName = "MailboxStatisticsItemType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[Serializable]
	public class MailboxStatisticsItem
	{
		[XmlElement("MailboxId")]
		[DataMember(Name = "MailboxId", IsRequired = true)]
		public string MailboxId { get; set; }

		[XmlElement("DisplayName")]
		[DataMember(Name = "DisplayName", IsRequired = true)]
		public string DisplayName { get; set; }

		[DataMember(Name = "ItemCount", IsRequired = true)]
		[XmlElement("ItemCount")]
		public long ItemCount { get; set; }

		[XmlElement("Size")]
		[DataMember(Name = "Size", IsRequired = true)]
		public ulong Size { get; set; }
	}
}
