using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage.Conversations
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class ConversationFamilyFactory : ICoreConversationFactory<ConversationFamily>
	{
		public ConversationFamilyFactory(IMailboxSession mailboxSession, ConversationId conversationFamilyId)
		{
			Util.ThrowOnNullArgument(mailboxSession, "session");
			this.mailboxSession = mailboxSession;
			XSOFactory @default = XSOFactory.Default;
			this.conversationFamilyId = conversationFamilyId;
			this.membersQuery = new ConversationFamilyMembersQuery(@default, this.MailboxSession);
			this.conversationFamilyTreeFactory = new ConversationTreeFactory(this.MailboxSession, ConversationTreeNodeFactory.ConversationFamilyTreeNodeIndexPropertyDefinition);
			this.selectedConversationTreeFactory = new ConversationTreeFactory(this.MailboxSession);
			this.dataExtractorFactory = new ConversationDataExtractorFactory(@default, this.MailboxSession);
		}

		protected IConversationTreeFactory SelectedConversationTreeFactory
		{
			get
			{
				return this.selectedConversationTreeFactory;
			}
		}

		protected ConversationId ConversationFamilyId
		{
			get
			{
				return this.conversationFamilyId;
			}
		}

		protected IMailboxSession MailboxSession
		{
			get
			{
				return this.mailboxSession;
			}
		}

		public ConversationFamily CreateConversation(ConversationId conversationId, params PropertyDefinition[] requestedProperties)
		{
			return this.CreateConversation(conversationId, null, false, false, requestedProperties);
		}

		public ConversationFamily CreateConversation(ConversationId conversationId, IList<StoreObjectId> folderIds, bool useFolderIdsAsExclusionList, bool isIrmEnabled, params PropertyDefinition[] requestedProperties)
		{
			return this.CreateConversation(conversationId, folderIds, useFolderIdsAsExclusionList, isIrmEnabled, false, null, requestedProperties);
		}

		public ConversationFamily CreateConversation(ConversationId conversationId, IList<StoreObjectId> folderIds, bool useFolderIdsAsExclusionList, bool isIrmEnabled, bool isSmimeSupported, string domainName, params PropertyDefinition[] requestedProperties)
		{
			Util.ThrowOnNullArgument(conversationId, "conversationId");
			HashSet<PropertyDefinition> hashSet = this.conversationFamilyTreeFactory.CalculatePropertyDefinitionsToBeLoaded(requestedProperties);
			List<IStorePropertyBag> list = this.membersQuery.Query(this.ConversationFamilyId, hashSet, folderIds, useFolderIdsAsExclusionList);
			Dictionary<object, List<IStorePropertyBag>> dictionary = this.membersQuery.AggregateMembersPerField(ItemSchema.ConversationId, this.ConversationFamilyId, list);
			List<IStorePropertyBag> list2;
			if (!dictionary.TryGetValue(conversationId, out list2))
			{
				list2 = new List<IStorePropertyBag>();
				dictionary.Add(conversationId, list2);
			}
			IConversationTree conversationTree = this.SelectedConversationTreeFactory.Create(list2, hashSet);
			ConversationDataExtractor dataExtractor = this.dataExtractorFactory.Create(isIrmEnabled, hashSet, conversationId, conversationTree, isSmimeSupported, domainName);
			if (dictionary.Count > 1)
			{
				IConversationTree conversationFamilyTree = this.conversationFamilyTreeFactory.Create(list, hashSet);
				return this.InternalCreateConversationFamilyWithSeveralConversations(dataExtractor, conversationFamilyTree, conversationId, conversationTree);
			}
			return this.InternalCreateConversationFamilyWithSingleConversation(dataExtractor, conversationTree);
		}

		protected virtual ConversationFamily InternalCreateConversationFamilyWithSeveralConversations(ConversationDataExtractor dataExtractor, IConversationTree conversationFamilyTree, ConversationId selectedConversationId, IConversationTree selectedConversationTree)
		{
			return new ConversationFamily(this.MailboxSession, dataExtractor, this.ConversationFamilyId, conversationFamilyTree, selectedConversationId, selectedConversationTree, this.SelectedConversationTreeFactory);
		}

		protected virtual ConversationFamily InternalCreateConversationFamilyWithSingleConversation(ConversationDataExtractor dataExtractor, IConversationTree conversationTree)
		{
			return new ConversationFamily(this.MailboxSession, dataExtractor, this.ConversationFamilyId, conversationTree, this.SelectedConversationTreeFactory);
		}

		private readonly ConversationFamilyMembersQuery membersQuery;

		private readonly ConversationTreeFactory conversationFamilyTreeFactory;

		private readonly IConversationTreeFactory selectedConversationTreeFactory;

		private readonly ConversationId conversationFamilyId;

		private readonly IMailboxSession mailboxSession;

		private readonly IConversationDataExtractorFactory dataExtractorFactory;
	}
}
