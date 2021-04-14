using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Wcf.Types
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public class GetModernConversationAttachmentsResponseMessage : ResponseMessage
	{
		public GetModernConversationAttachmentsResponseMessage()
		{
		}

		internal GetModernConversationAttachmentsResponseMessage(ServiceResultCode code, ServiceError error, ModernConversationAttachmentsResponseType conversationAttachments) : base(code, error)
		{
			this.ConversationAttachments = conversationAttachments;
		}

		public override ResponseType GetResponseType()
		{
			return ResponseType.GetModernConversationAttachmentsResponseMessage;
		}

		[DataMember(EmitDefaultValue = false)]
		public ModernConversationAttachmentsResponseType ConversationAttachments { get; set; }
	}
}
