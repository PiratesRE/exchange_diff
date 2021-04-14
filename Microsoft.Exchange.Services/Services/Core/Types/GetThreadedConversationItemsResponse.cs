using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Services.Core.Types.Conversations;

namespace Microsoft.Exchange.Services.Core.Types
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public class GetThreadedConversationItemsResponse : BaseInfoResponse
	{
		public GetThreadedConversationItemsResponse() : base(ResponseType.GetThreadedConversationItemsResponseMessage)
		{
		}

		internal override ResponseMessage CreateResponseMessage<TValue>(ServiceResultCode code, ServiceError error, TValue item)
		{
			return new GetThreadedConversationItemsResponseMessage(code, error, item as ThreadedConversationResponseType);
		}
	}
}
