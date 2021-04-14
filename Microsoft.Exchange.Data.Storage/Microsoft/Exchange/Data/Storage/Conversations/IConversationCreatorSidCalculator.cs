using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage.Conversations
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal interface IConversationCreatorSidCalculator
	{
		bool TryCalculateOnDelivery(ICorePropertyBag itemPropertyBag, ConversationIndex.FixupStage stage, ConversationIndex conversationIndex, out byte[] conversationCreatorSid, out bool updateAllConversationMessages);

		bool TryCalculateOnSave(ICorePropertyBag itemPropertyBag, ConversationIndex.FixupStage stage, ConversationIndex conversationIndex, CoreItemOperation operation, out byte[] conversationCreatorSid);

		bool TryCalculateOnReply(ConversationIndex conversationIndex, out byte[] conversationCreatorSid);

		void UpdateConversationMessages(ConversationIndex conversationIndex, byte[] conversationCreatorSid);
	}
}
