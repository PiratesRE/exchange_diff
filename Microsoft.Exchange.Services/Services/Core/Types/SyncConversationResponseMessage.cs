using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[XmlType("SyncConversationResponseMessageType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public class SyncConversationResponseMessage : ResponseMessage
	{
		public SyncConversationResponseMessage()
		{
		}

		internal SyncConversationResponseMessage(ServiceResultCode code, ServiceError error) : base(code, error)
		{
		}

		public override ResponseType GetResponseType()
		{
			return ResponseType.SyncConversationResponseMessage;
		}

		[DataMember(Name = "SyncState", IsRequired = true, Order = 1)]
		[XmlElement("SyncState", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
		public string SyncState { get; set; }

		[DataMember(Name = "IncludesLastItemInRange", IsRequired = true, EmitDefaultValue = true, Order = 2)]
		[XmlElement("IncludesLastItemInRange", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
		public bool IncludesLastItemInRange { get; set; }

		[DataMember(Order = 3)]
		[XmlArray(ElementName = "Conversations")]
		[XmlArrayItem(ElementName = "Conversation", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
		public ConversationType[] Conversations { get; set; }

		[XmlArrayItem(ElementName = "DeletedConversation", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
		[XmlArray(ElementName = "DeletedConversations")]
		[DataMember(Order = 4)]
		public DeletedConversationType[] DeletedConversations { get; set; }

		[DataMember(Order = 5)]
		[XmlArray(ElementName = "ConversationViewDataList")]
		[XmlArrayItem(ElementName = "ConversationViewData", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
		public ConversationViewDataType[] ConversationViewDataList { get; set; }
	}
}
