using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage.Conversations
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class LegacyConversationCreatorSidCalculator : IConversationCreatorSidCalculator
	{
		public LegacyConversationCreatorSidCalculator(IMailboxSession mailboxSession)
		{
			this.mailboxSession = mailboxSession;
		}

		public bool TryCalculateOnDelivery(ICorePropertyBag itemPropertyBag, ConversationIndex.FixupStage stage, ConversationIndex conversationIndex, out byte[] conversationCreatorSid, out bool updateAllConversationMessages)
		{
			return ConversationCreatorHelper.TryCalculateConversationCreatorSidOnDeliveryProcessing(this.mailboxSession as MailboxSession, itemPropertyBag, stage, conversationIndex, out conversationCreatorSid, out updateAllConversationMessages);
		}

		public bool TryCalculateOnSave(ICorePropertyBag itemPropertyBag, ConversationIndex.FixupStage stage, ConversationIndex conversationIndex, CoreItemOperation operation, out byte[] conversationCreatorSid)
		{
			return ConversationCreatorHelper.TryCalculateConversationCreatorSidOnSaving(this.mailboxSession as MailboxSession, itemPropertyBag, stage, conversationIndex, out conversationCreatorSid);
		}

		public bool TryCalculateOnReply(ConversationIndex conversationIndex, out byte[] conversationCreatorSid)
		{
			return ConversationCreatorHelper.TryCalculateConversationCreatorSidOnReplying(this.mailboxSession as MailboxSession, conversationIndex, out conversationCreatorSid);
		}

		public void UpdateConversationMessages(ConversationIndex conversationIndex, byte[] conversationCreatorSid)
		{
			ConversationCreatorHelper.FixupConversationMessagesCreatorSid(this.mailboxSession as MailboxSession, conversationIndex, conversationCreatorSid);
		}

		private readonly IMailboxSession mailboxSession;
	}
}
