using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Wcf.Types
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public class GetModernConversationAttachmentsResponse : BaseInfoResponse
	{
		public GetModernConversationAttachmentsResponse() : base(ResponseType.GetModernConversationAttachmentsResponseMessage)
		{
		}

		internal override ResponseMessage CreateResponseMessage<TValue>(ServiceResultCode code, ServiceError error, TValue item)
		{
			return new GetModernConversationAttachmentsResponseMessage(code, error, item as ModernConversationAttachmentsResponseType);
		}
	}
}
