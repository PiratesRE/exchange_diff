using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.Conversations;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Services.Core.DataConverter;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Core.Conversations.ResponseBuilders
{
	internal sealed class ConversationDiagnosticsResponseBuilder : ConversationResponseBuilderBase<GetConversationItemsDiagnosticsResponseType>
	{
		public ConversationDiagnosticsResponseBuilder(IMailboxSession mailboxSession, IConversation conversation, IParticipantResolver resolver)
		{
			this.conversation = conversation;
			this.mailboxSession = mailboxSession;
			this.participantResolver = resolver;
		}

		protected override GetConversationItemsDiagnosticsResponseType BuildSkeleton()
		{
			return new GetConversationItemsDiagnosticsResponseType();
		}

		protected override void BuildConversationProperties()
		{
			base.Response.ConversationId = ConversationDataConverter.GetConversationId(this.mailboxSession, this.conversation.ConversationId);
			base.Response.Recipients = this.CalculateConversationParticipants();
			base.Response.CanDelete = EffectiveRightsProperty.GetFromEffectiveRights(this.conversation.EffectiveRights, this.mailboxSession).Delete;
		}

		protected override void BuildNodes()
		{
			base.Response.ConversationNodeMetadatum = this.BuildConversationNodesMetadata().ToArray();
		}

		private SingleRecipientType[] CalculateConversationParticipants()
		{
			ParticipantTable participantTable = this.conversation.LoadReplyAllParticipantsPerType();
			ParticipantSet participantSet = new ParticipantSet();
			participantSet.UnionWith(participantTable[RecipientItemType.To]);
			participantSet.UnionWith(participantTable[RecipientItemType.Cc]);
			return this.participantResolver.ResolveToSingleRecipientType(participantSet);
		}

		private List<ConversationNodeMetadata> BuildConversationNodesMetadata()
		{
			List<ConversationNodeMetadata> list = new List<ConversationNodeMetadata>();
			List<IConversationTreeNode> list2 = this.conversation.ConversationTree.ToList<IConversationTreeNode>();
			this.conversation.ConversationTree.Sort(ConversationTreeSortOrder.ChronologicalAscending);
			Dictionary<IConversationTreeNode, ParticipantSet> dictionary = this.conversation.LoadAddedParticipants();
			List<IConversationTreeNode> list3 = new List<IConversationTreeNode>();
			int num = 0;
			foreach (IConversationTreeNode conversationTreeNode in list2)
			{
				list3.Add(conversationTreeNode);
				ItemPart itemPart = this.conversation.GetItemPart(conversationTreeNode.MainStoreObjectId);
				ConversationNodeMetadata conversationNodeMetadata = new ConversationNodeMetadata();
				conversationNodeMetadata.Order = num;
				conversationNodeMetadata.From = this.participantResolver.ResolveToSingleRecipientType(this.Get<Participant>(itemPart.StorePropertyBag, ItemSchema.From));
				conversationNodeMetadata.Sender = this.participantResolver.ResolveToSingleRecipientType(this.Get<Participant>(itemPart.StorePropertyBag, ItemSchema.Sender));
				conversationNodeMetadata.InternetMessageId = this.Get<string>(itemPart.StorePropertyBag, ItemSchema.InternetMessageId);
				conversationNodeMetadata.ItemClass = this.Get<string>(itemPart.StorePropertyBag, StoreObjectSchema.ItemClass);
				conversationNodeMetadata.ThreadId = ConversationDataConverter.GetConversationId(this.mailboxSession, this.Get<ConversationId>(itemPart.StorePropertyBag, ItemSchema.ConversationFamilyId));
				conversationNodeMetadata.References = this.Get<string>(itemPart.StorePropertyBag, ItemSchema.InternetReferences);
				conversationNodeMetadata.ConversationIndexTrackingEx = this.Get<string>(itemPart.StorePropertyBag, ItemSchema.ConversationIndexTrackingEx);
				conversationNodeMetadata.SubjectPrefix = this.Get<string>(itemPart.StorePropertyBag, ItemSchema.SubjectPrefix);
				conversationNodeMetadata.Preview = this.Get<string>(itemPart.StorePropertyBag, ItemSchema.Preview);
				conversationNodeMetadata.ReceivedTime = ExDateTimeConverter.ToOffsetXsdDateTime(this.Get<ExDateTime>(itemPart.StorePropertyBag, ItemSchema.ReceivedTime), EWSSettings.RequestTimeZone);
				conversationNodeMetadata.InReplyTo = this.Get<string>(itemPart.StorePropertyBag, ItemSchema.InReplyTo);
				conversationNodeMetadata.ReplyAllParticipants = this.participantResolver.ResolveToSingleRecipientType(this.conversation.LoadReplyAllParticipants(conversationTreeNode));
				conversationNodeMetadata.Subject = itemPart.Subject;
				ParticipantSet participants;
				if (dictionary.TryGetValue(conversationTreeNode, out participants))
				{
					conversationNodeMetadata.AddedParticipants = this.participantResolver.ResolveToSingleRecipientType(participants);
				}
				list.Add(conversationNodeMetadata);
				num++;
			}
			return list;
		}

		private T Get<T>(IStorePropertyBag bag, StorePropertyDefinition property)
		{
			return bag.GetValueOrDefault<T>(property, default(T));
		}

		private readonly IConversation conversation;

		private readonly IMailboxSession mailboxSession;

		private readonly IParticipantResolver participantResolver;
	}
}
