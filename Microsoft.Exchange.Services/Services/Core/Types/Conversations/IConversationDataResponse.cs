using System;

namespace Microsoft.Exchange.Services.Core.Types.Conversations
{
	internal interface IConversationDataResponse
	{
		EmailAddressWrapper[] ToRecipients { get; set; }

		EmailAddressWrapper[] CcRecipients { get; set; }

		ConversationNode[] ConversationNodes { get; set; }

		int TotalConversationNodesCount { get; set; }
	}
}
