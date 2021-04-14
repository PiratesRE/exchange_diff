using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Storage;

namespace Microsoft.Exchange.Data.Storage.Conversations
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class ConversationCreatorSidCalculator : IConversationCreatorSidCalculator
	{
		public ConversationCreatorSidCalculator(IXSOFactory xsoFactory, IMailboxSession mailboxSession, ICoreConversationFactory<IConversation> conversationFactory)
		{
			this.mailboxSession = mailboxSession;
			this.conversationFactory = conversationFactory;
			this.xsoFactory = xsoFactory;
		}

		public bool TryCalculateOnDelivery(ICorePropertyBag itemPropertyBag, ConversationIndex.FixupStage stage, ConversationIndex conversationIndex, out byte[] conversationCreatorSid, out bool updateAllConversationMessages)
		{
			ConversationCreatorSidCalculator.MessageType messageType = this.CalculateMessageTypeOnDelivery(conversationIndex, itemPropertyBag, stage);
			updateAllConversationMessages = (messageType == ConversationCreatorSidCalculator.MessageType.OutOfOrderRootMessage);
			byte[] valueOrDefault = itemPropertyBag.GetValueOrDefault<byte[]>(InternalSchema.SenderSID, null);
			return this.TryCalculateConversationCreatorSid(conversationIndex, messageType, valueOrDefault, out conversationCreatorSid);
		}

		public bool TryCalculateOnSave(ICorePropertyBag itemPropertyBag, ConversationIndex.FixupStage stage, ConversationIndex conversationIndex, CoreItemOperation operation, out byte[] conversationCreatorSid)
		{
			conversationCreatorSid = null;
			if (operation != CoreItemOperation.Save)
			{
				return false;
			}
			byte[] itemOwnerSid = ValueConvertor.ConvertValueToBinary(this.mailboxSession.MailboxOwner.Sid, null);
			ConversationCreatorSidCalculator.MessageType messageType = this.CalculateMessageTypeOnSave(stage);
			return this.TryCalculateConversationCreatorSid(conversationIndex, messageType, itemOwnerSid, out conversationCreatorSid);
		}

		public bool TryCalculateOnReply(ConversationIndex conversationIndex, out byte[] conversationCreatorSid)
		{
			conversationCreatorSid = null;
			return false;
		}

		public void UpdateConversationMessages(ConversationIndex conversationIndex, byte[] conversationCreatorSid)
		{
			IConversation conversation = this.LoadConversation(conversationIndex);
			foreach (IConversationTreeNode conversationTreeNode in conversation.ConversationTree)
			{
				foreach (StoreObjectId id in conversationTreeNode.ToListStoreObjectId())
				{
					using (IItem item = this.xsoFactory.BindToItem(this.mailboxSession, id, new PropertyDefinition[0]))
					{
						item.OpenAsReadWrite();
						item.SetOrDeleteProperty(ItemSchema.ConversationCreatorSID, conversationCreatorSid);
						item.Save(SaveMode.ResolveConflicts);
					}
				}
			}
		}

		private IConversation LoadConversation(ConversationIndex index)
		{
			ConversationId conversationId = ConversationId.Create(index);
			return this.conversationFactory.CreateConversation(conversationId, ConversationCreatorSidCalculator.ConversationCreatorRelevantProperties);
		}

		private ConversationCreatorSidCalculator.MessageType CalculateMessageTypeOnDelivery(ConversationIndex conversationIndex, ICorePropertyBag itemBag, ConversationIndex.FixupStage fixupStage)
		{
			if (ConversationIndex.CheckStageValue(fixupStage, ConversationIndex.FixupStage.Error))
			{
				return ConversationCreatorSidCalculator.MessageType.Unknown;
			}
			if (ConversationIndex.IsFixUpCreatingNewConversation(fixupStage))
			{
				return ConversationCreatorSidCalculator.MessageType.RootMessage;
			}
			IConversation conversation = this.LoadConversation(conversationIndex);
			if (conversation.RootMessageNode == null)
			{
				ExTraceGlobals.StorageTracer.TraceDebug<string, string>(0L, "ConversationCreatorHelper::CalculateConversationDeliveryScenario : On some corner cases, the conversation is loaded without nodes and then root node is null. MessageClassConversationID:{0} FixupStage:{1}", this.LoadConversation(conversationIndex).ConversationId.ToString(), fixupStage.ToString());
				return ConversationCreatorSidCalculator.MessageType.RootMessage;
			}
			if (ConversationIndex.IsFixupAddingOutOfOrderMessageToConversation(fixupStage) && this.IsRootMessage(conversation, itemBag))
			{
				return ConversationCreatorSidCalculator.MessageType.OutOfOrderRootMessage;
			}
			return ConversationCreatorSidCalculator.MessageType.NonRootMessage;
		}

		private ConversationCreatorSidCalculator.MessageType CalculateMessageTypeOnSave(ConversationIndex.FixupStage fixupStage)
		{
			if (ConversationIndex.CheckStageValue(fixupStage, ConversationIndex.FixupStage.Error))
			{
				return ConversationCreatorSidCalculator.MessageType.Unknown;
			}
			if (ConversationIndex.IsFixUpCreatingNewConversation(fixupStage))
			{
				return ConversationCreatorSidCalculator.MessageType.RootMessage;
			}
			return ConversationCreatorSidCalculator.MessageType.NonRootMessage;
		}

		private bool IsRootMessage(IConversation conversation, ICorePropertyBag message)
		{
			string valueOrDefault = conversation.RootMessageNode.GetValueOrDefault<string>(InternalSchema.InReplyTo, string.Empty);
			if (string.IsNullOrEmpty(valueOrDefault))
			{
				return false;
			}
			string valueOrDefault2 = message.GetValueOrDefault<string>(InternalSchema.InternetMessageId, string.Empty);
			return valueOrDefault.Equals(valueOrDefault2, StringComparison.InvariantCultureIgnoreCase);
		}

		private bool TryCalculateConversationCreatorSid(ConversationIndex conversationIndex, ConversationCreatorSidCalculator.MessageType messageType, byte[] itemOwnerSid, out byte[] conversationCreatorSid)
		{
			switch (messageType)
			{
			case ConversationCreatorSidCalculator.MessageType.RootMessage:
			case ConversationCreatorSidCalculator.MessageType.OutOfOrderRootMessage:
				conversationCreatorSid = itemOwnerSid;
				break;
			case ConversationCreatorSidCalculator.MessageType.NonRootMessage:
				conversationCreatorSid = this.LoadConversation(conversationIndex).ConversationCreatorSID;
				break;
			default:
				conversationCreatorSid = null;
				break;
			}
			return conversationCreatorSid != null;
		}

		private static PropertyDefinition[] ConversationCreatorRelevantProperties = new PropertyDefinition[]
		{
			ItemSchema.Id,
			ItemSchema.ConversationCreatorSID,
			InternalSchema.SenderSID,
			ItemSchema.ReceivedTime,
			ItemSchema.InReplyTo
		};

		private readonly IMailboxSession mailboxSession;

		private readonly ICoreConversationFactory<IConversation> conversationFactory;

		private readonly IXSOFactory xsoFactory;

		private enum MessageType
		{
			Unknown,
			RootMessage,
			OutOfOrderRootMessage,
			NonRootMessage
		}
	}
}
