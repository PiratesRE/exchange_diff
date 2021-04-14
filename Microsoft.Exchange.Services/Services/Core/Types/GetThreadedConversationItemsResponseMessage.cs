using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Services.Core.Types.Conversations;

namespace Microsoft.Exchange.Services.Core.Types
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public class GetThreadedConversationItemsResponseMessage : ResponseMessage
	{
		public GetThreadedConversationItemsResponseMessage()
		{
		}

		internal GetThreadedConversationItemsResponseMessage(ServiceResultCode code, ServiceError error, ThreadedConversationResponseType conversation) : base(code, error)
		{
			this.Conversation = conversation;
		}

		public override ResponseType GetResponseType()
		{
			return ResponseType.GetThreadedConversationItemsResponseMessage;
		}

		[DataMember(EmitDefaultValue = false)]
		public ThreadedConversationResponseType Conversation { get; set; }
	}
}
