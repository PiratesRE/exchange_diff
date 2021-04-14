using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Storage;

namespace Microsoft.Exchange.Data.Storage.Conversations
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal static class ConversationCreatorHelper
	{
		public static bool TryCalculateConversationCreatorSidOnDeliveryProcessing(MailboxSession mailboxSession, ICorePropertyBag itemPropertyBag, ConversationIndex.FixupStage fixupStage, ConversationIndex conversationIndex, out byte[] conversationCreatorSid, out bool updateAllConversationMessages)
		{
			conversationCreatorSid = null;
			updateAllConversationMessages = false;
			if (!ConversationCreatorHelper.SupportsConversationCreator(mailboxSession))
			{
				return false;
			}
			byte[] valueOrDefault = itemPropertyBag.GetValueOrDefault<byte[]>(InternalSchema.SenderSID, null);
			return ConversationCreatorHelper.TryCalculateConversationCreatorSid(mailboxSession, itemPropertyBag, fixupStage, conversationIndex, valueOrDefault, out conversationCreatorSid, out updateAllConversationMessages);
		}

		public static bool TryCalculateConversationCreatorSidOnSaving(MailboxSession mailboxSession, ICorePropertyBag itemPropertyBag, ConversationIndex.FixupStage fixupStage, ConversationIndex conversationIndex, out byte[] conversationCreatorSid)
		{
			conversationCreatorSid = null;
			if (!ConversationCreatorHelper.SupportsConversationCreator(mailboxSession) || itemPropertyBag.GetValueOrDefault<bool>(InternalSchema.DeleteAfterSubmit, false))
			{
				return false;
			}
			conversationCreatorSid = ConversationCreatorHelper.CalculateConversationCreatorSid(mailboxSession, itemPropertyBag, fixupStage, conversationIndex, ValueConvertor.ConvertValueToBinary(mailboxSession.MailboxOwner.Sid, null));
			return conversationCreatorSid != null;
		}

		public static bool TryCalculateConversationCreatorSidOnReplying(MailboxSession mailboxSession, ConversationIndex conversationIndex, out byte[] conversationCreatorSid)
		{
			conversationCreatorSid = null;
			if (!ConversationCreatorHelper.SupportsConversationCreator(mailboxSession))
			{
				return false;
			}
			ConversationCreatorHelper.ConversationCreatorDefinitionData definitionData = new ConversationCreatorHelper.ConversationCreatorDefinitionData(mailboxSession, conversationIndex);
			conversationCreatorSid = ConversationCreatorHelper.CalculateConversationCreatorSid(definitionData, ConversationCreatorHelper.MessageDeliveryScenario.DeliveringNonRootMessage, null);
			return conversationCreatorSid != null;
		}

		public static void FixupConversationMessagesCreatorSid(MailboxSession mailboxSession, ConversationIndex conversationIndex, byte[] conversationCreatorSid)
		{
			ConversationCreatorHelper.ConversationCreatorDefinitionData conversationCreatorDefinitionData = new ConversationCreatorHelper.ConversationCreatorDefinitionData(mailboxSession, conversationIndex);
			foreach (IConversationTreeNode conversationTreeNode in conversationCreatorDefinitionData.Conversation.ConversationTree)
			{
				ConversationTreeNode conversationTreeNode2 = (ConversationTreeNode)conversationTreeNode;
				foreach (StoreObjectId storeId in conversationTreeNode2.ToListStoreObjectId())
				{
					using (Item item = Item.Bind(mailboxSession, storeId, null))
					{
						item.OpenAsReadWrite();
						item.SetOrDeleteProperty(ItemSchema.ConversationCreatorSID, conversationCreatorSid);
						item.Save(SaveMode.ResolveConflicts);
					}
				}
			}
		}

		private static ConversationCreatorHelper.MessageDeliveryScenario CalculateConversationDeliveryScenario(ConversationCreatorHelper.ConversationCreatorDefinitionData definitionData, ICorePropertyBag itemPropertyBag, ConversationIndex.FixupStage fixupStage)
		{
			if (ConversationIndex.CheckStageValue(fixupStage, ConversationIndex.FixupStage.Error))
			{
				return ConversationCreatorHelper.MessageDeliveryScenario.Unknown;
			}
			if (ConversationIndex.IsFixUpCreatingNewConversation(fixupStage))
			{
				return ConversationCreatorHelper.MessageDeliveryScenario.DeliveringRootMessage;
			}
			if (definitionData.Conversation.RootMessageId == null)
			{
				ExTraceGlobals.StorageTracer.TraceDebug<string, string>(0L, "ConversationCreatorHelper::CalculateConversationDeliveryScenario : On some corner cases, the conversation is loaded without nodes and then root node is null. MessageClassConversationID:{0} FixupStage:{1}", definitionData.Conversation.ConversationId.ToString(), fixupStage.ToString());
				return ConversationCreatorHelper.MessageDeliveryScenario.DeliveringRootMessage;
			}
			if (ConversationIndex.IsFixupAddingOutOfOrderMessageToConversation(fixupStage) && ConversationCreatorHelper.IsRootMessage(definitionData.Conversation, itemPropertyBag))
			{
				return ConversationCreatorHelper.MessageDeliveryScenario.DeliveringOutOfOrderRootMessage;
			}
			return ConversationCreatorHelper.MessageDeliveryScenario.Unknown;
		}

		private static bool IsRootMessage(Conversation conversation, ICorePropertyBag messagePropertyBag)
		{
			StoreObjectId rootMessageId = conversation.RootMessageId;
			if (rootMessageId == null)
			{
				return false;
			}
			VersionedId valueOrDefault = messagePropertyBag.GetValueOrDefault<VersionedId>(ItemSchema.Id, null);
			return rootMessageId.Equals(valueOrDefault.ObjectId);
		}

		private static bool TryCalculateConversationCreatorSid(MailboxSession mailboxSession, ICorePropertyBag itemPropertyBag, ConversationIndex.FixupStage fixupStage, ConversationIndex conversationIndex, byte[] itemOwnerSID, out byte[] conversationCreatorSid, out bool updateAllConversationMessages)
		{
			ConversationCreatorHelper.ConversationCreatorDefinitionData definitionData = new ConversationCreatorHelper.ConversationCreatorDefinitionData(mailboxSession, conversationIndex);
			ConversationCreatorHelper.MessageDeliveryScenario messageDeliveryScenario = ConversationCreatorHelper.CalculateConversationDeliveryScenario(definitionData, itemPropertyBag, fixupStage);
			return ConversationCreatorHelper.TryCalculateConversationCreatorSid(definitionData, messageDeliveryScenario, itemOwnerSID, out conversationCreatorSid, out updateAllConversationMessages);
		}

		private static byte[] CalculateConversationCreatorSid(MailboxSession mailboxSession, ICorePropertyBag itemPropertyBag, ConversationIndex.FixupStage fixupStage, ConversationIndex conversationIndex, byte[] itemOwnerSID)
		{
			ConversationCreatorHelper.ConversationCreatorDefinitionData definitionData = new ConversationCreatorHelper.ConversationCreatorDefinitionData(mailboxSession, conversationIndex);
			ConversationCreatorHelper.MessageDeliveryScenario messageDeliveryScenario = ConversationCreatorHelper.CalculateConversationDeliveryScenario(definitionData, itemPropertyBag, fixupStage);
			return ConversationCreatorHelper.CalculateConversationCreatorSid(definitionData, messageDeliveryScenario, itemOwnerSID);
		}

		private static bool TryCalculateConversationCreatorSid(ConversationCreatorHelper.ConversationCreatorDefinitionData definitionData, ConversationCreatorHelper.MessageDeliveryScenario messageDeliveryScenario, byte[] itemOwnerSID, out byte[] conversationCreatorSid, out bool updateAllConversationMessages)
		{
			updateAllConversationMessages = false;
			if (messageDeliveryScenario == ConversationCreatorHelper.MessageDeliveryScenario.DeliveringRootMessage || messageDeliveryScenario == ConversationCreatorHelper.MessageDeliveryScenario.Unknown)
			{
				conversationCreatorSid = itemOwnerSID;
				return true;
			}
			if (messageDeliveryScenario == ConversationCreatorHelper.MessageDeliveryScenario.DeliveringOutOfOrderRootMessage)
			{
				conversationCreatorSid = itemOwnerSID;
				updateAllConversationMessages = true;
				return true;
			}
			Conversation conversation = definitionData.Conversation;
			conversationCreatorSid = conversation.ConversationCreatorSID;
			if (conversationCreatorSid == null)
			{
				IConversationTreeNode rootMessageNode = definitionData.Conversation.ConversationTree.RootMessageNode;
				if (rootMessageNode == null)
				{
					return false;
				}
				ExTraceGlobals.StorageTracer.TraceDebug<string>(0L, "ConversationCreatorHelper::TryCalculateConversationCreatorSid : Probably the conversation was created before we started tracking the ConversationCreatorSID or the conversation was created from a message sent by X-Prem user. ConversationID:{0}", conversation.ConversationId.ToString());
				conversationCreatorSid = rootMessageNode.GetValueOrDefault<byte[]>(InternalSchema.SenderSID, null);
				updateAllConversationMessages = (conversationCreatorSid != null);
			}
			return conversationCreatorSid != null;
		}

		private static byte[] CalculateConversationCreatorSid(ConversationCreatorHelper.ConversationCreatorDefinitionData definitionData, ConversationCreatorHelper.MessageDeliveryScenario messageDeliveryScenario, byte[] itemOwnerSID)
		{
			if (messageDeliveryScenario == ConversationCreatorHelper.MessageDeliveryScenario.DeliveringRootMessage || messageDeliveryScenario == ConversationCreatorHelper.MessageDeliveryScenario.Unknown)
			{
				return itemOwnerSID;
			}
			Conversation conversation = definitionData.Conversation;
			return conversation.ConversationCreatorSID;
		}

		private static bool SupportsConversationCreator(MailboxSession session)
		{
			if (session == null)
			{
				return false;
			}
			if (!ConversationCreatorHelper.IsSessionLogonTypeSupported(session.LogonType))
			{
				return false;
			}
			int? num = session.Mailbox.TryGetProperty(MailboxSchema.MailboxTypeDetail) as int?;
			return num != null && StoreSession.IsGroupMailbox(num.Value);
		}

		private static bool IsSessionLogonTypeSupported(LogonType logonType)
		{
			switch (logonType)
			{
			case LogonType.Owner:
			case LogonType.Delegated:
			case LogonType.Transport:
				return true;
			}
			return false;
		}

		private class ConversationCreatorDefinitionData
		{
			public ConversationCreatorDefinitionData(MailboxSession mailboxSession, ConversationIndex conversationIndex)
			{
				this.mailboxSession = mailboxSession;
				this.conversationIndex = conversationIndex;
			}

			public Conversation Conversation
			{
				get
				{
					if (this.conversation == null)
					{
						this.conversation = this.LoadConversationForConversationCreatorSidDefinition();
					}
					return this.conversation;
				}
			}

			private Conversation LoadConversationForConversationCreatorSidDefinition()
			{
				ConversationId conversationId = ConversationId.Create(this.conversationIndex.Guid);
				return Conversation.Load(this.mailboxSession, conversationId, ConversationCreatorHelper.ConversationCreatorDefinitionData.ConversationCreatorRelevantProperties);
			}

			private static PropertyDefinition[] ConversationCreatorRelevantProperties = new PropertyDefinition[]
			{
				ItemSchema.Id,
				ItemSchema.ConversationCreatorSID,
				InternalSchema.SenderSID,
				ItemSchema.ReceivedTime
			};

			private readonly MailboxSession mailboxSession;

			private readonly ConversationIndex conversationIndex;

			private Conversation conversation;
		}

		private enum MessageDeliveryScenario
		{
			Unknown,
			DeliveringRootMessage,
			DeliveringOutOfOrderRootMessage,
			DeliveringNonRootMessage
		}
	}
}
