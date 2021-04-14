using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage.Conversations
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class ConversationFactory : ICoreConversationFactory<Conversation>
	{
		public ConversationFactory(IMailboxSession session) : this(session, new ConversationTreeFactory(session), new ConversationMembersQuery(XSOFactory.Default, session), new ConversationDataExtractorFactory(XSOFactory.Default, session))
		{
		}

		protected ConversationFactory(IMailboxSession session, IConversationTreeFactory treeFactory, IConversationMembersQuery membersQuery, IConversationDataExtractorFactory dataExtractorFactory)
		{
			this.session = session;
			this.membersQuery = membersQuery;
			this.treeFactory = treeFactory;
			this.dataExtractorFactory = dataExtractorFactory;
		}

		protected IConversationTreeFactory TreeFactory
		{
			get
			{
				return this.treeFactory;
			}
		}

		protected IMailboxSession Session
		{
			get
			{
				return this.session;
			}
		}

		protected IConversationMembersQuery MembersQuery
		{
			get
			{
				return this.membersQuery;
			}
		}

		protected IConversationDataExtractorFactory DataExtractorFactory
		{
			get
			{
				return this.dataExtractorFactory;
			}
		}

		public Conversation CreateConversation(ConversationId conversationId, params PropertyDefinition[] requestedProperties)
		{
			return this.CreateConversation(conversationId, null, false, false, requestedProperties);
		}

		public virtual Conversation CreateConversation(ConversationId conversationId, IList<StoreObjectId> folderIds, bool useFolderIdsAsExclusionList, bool isIrmEnabled, params PropertyDefinition[] requestedProperties)
		{
			return this.CreateConversation(conversationId, folderIds, useFolderIdsAsExclusionList, isIrmEnabled, false, null, requestedProperties);
		}

		public virtual Conversation CreateConversation(ConversationId conversationId, IList<StoreObjectId> folderIds, bool useFolderIdsAsExclusionList, bool isIrmEnabled, bool isSmimeSupported, string domainName, params PropertyDefinition[] requestedProperties)
		{
			HashSet<PropertyDefinition> hashSet = this.SanitizePropertiesRequested(requestedProperties);
			List<IStorePropertyBag> queryResult = this.MembersQuery.Query(conversationId, hashSet, folderIds, useFolderIdsAsExclusionList);
			IConversationTree conversationTree = this.TreeFactory.Create(queryResult, hashSet);
			ConversationStateFactory stateFactory = new ConversationStateFactory(this.Session, conversationTree);
			ConversationDataExtractor conversationDataExtractor = this.DataExtractorFactory.Create(isIrmEnabled, hashSet, conversationId, conversationTree, isSmimeSupported, domainName);
			return this.InternalCreateConversation(conversationId, conversationDataExtractor, stateFactory, conversationTree);
		}

		protected virtual Conversation InternalCreateConversation(ConversationId conversationId, ConversationDataExtractor conversationDataExtractor, ConversationStateFactory stateFactory, IConversationTree conversationTree)
		{
			return new Conversation(conversationId, conversationTree, this.Session as MailboxSession, conversationDataExtractor, new ConversationTreeFactory(this.Session), stateFactory);
		}

		private HashSet<PropertyDefinition> SanitizePropertiesRequested(PropertyDefinition[] requestedProperties)
		{
			return this.TreeFactory.CalculatePropertyDefinitionsToBeLoaded(requestedProperties);
		}

		private readonly IMailboxSession session;

		private readonly IConversationMembersQuery membersQuery;

		private readonly IConversationTreeFactory treeFactory;

		private readonly IConversationDataExtractorFactory dataExtractorFactory;
	}
}
