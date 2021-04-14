using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using Microsoft.Exchange.Services.Core.Types.Conversations;

namespace Microsoft.Exchange.Services.Core.Types
{
	[XmlType("GetConversationItemsResponseMessageType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public class GetConversationItemsResponseMessage : ResponseMessage
	{
		public GetConversationItemsResponseMessage()
		{
		}

		internal GetConversationItemsResponseMessage(ServiceResultCode code, ServiceError error, ConversationResponseType conversation) : base(code, error)
		{
			this.Conversation = conversation;
		}

		public override ResponseType GetResponseType()
		{
			return ResponseType.GetConversationItemsResponseMessage;
		}

		[DataMember(EmitDefaultValue = false)]
		[XmlElement(ElementName = "Conversation")]
		public ConversationResponseType Conversation { get; set; }
	}
}
