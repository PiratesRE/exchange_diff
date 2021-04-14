using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange", Name = "ConversationViewData")]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[Serializable]
	public class ConversationViewDataType
	{
		[DataMember(IsRequired = true, Order = 1)]
		[XmlElement("FolderId")]
		public BaseFolderId FolderId { get; set; }

		[DataMember(EmitDefaultValue = true, Order = 2)]
		[XmlElement("TotalConversationsInView")]
		public int TotalConversationsInView { get; set; }

		[XmlElement("OldestDeliveryTime")]
		[DataMember(EmitDefaultValue = false, Order = 3)]
		public string OldestDeliveryTime { get; set; }

		[XmlElement("MoreItemsOnServer")]
		[DataMember(EmitDefaultValue = true, Order = 4)]
		public bool MoreItemsOnServer { get; set; }
	}
}
