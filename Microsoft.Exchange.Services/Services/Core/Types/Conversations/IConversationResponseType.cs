using System;

namespace Microsoft.Exchange.Services.Core.Types.Conversations
{
	public interface IConversationResponseType
	{
		ItemId ConversationId { get; set; }

		byte[] SyncState { get; set; }
	}
}
