using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Services.Core.Conversations;

namespace Microsoft.Exchange.Services.Core.Types
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	[Serializable]
	public class GetThreadedConversationItemsRequest : GetConversationItemsRequest
	{
		internal override ServiceCommandBase GetServiceCommand(CallContext callContext)
		{
			return new GetThreadedConversationItems(callContext, this);
		}
	}
}
