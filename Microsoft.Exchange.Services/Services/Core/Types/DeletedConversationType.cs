using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange", Name = "DeletedConversation")]
	[Serializable]
	public class DeletedConversationType
	{
		public DeletedConversationType()
		{
		}

		public DeletedConversationType(ItemId convItemId, FolderId folderId)
		{
			this.ConversationId = convItemId;
			this.FolderId = folderId;
		}

		[DataMember(IsRequired = true, Order = 1)]
		[XmlElement("ConversationId")]
		public ItemId ConversationId { get; set; }

		[DataMember(IsRequired = true, Order = 2)]
		[XmlElement("FolderId")]
		public BaseFolderId FolderId { get; set; }
	}
}
