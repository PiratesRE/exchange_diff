using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage.Conversations
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class ThreadedConversationFactory : ICoreConversationFactory<IThreadedConversation>
	{
		public ThreadedConversationFactory(IMailboxSession mailboxSession) : this(mailboxSession, new ConversationMembersQuery(XSOFactory.Default, mailboxSession), new ConversationTreeFactory(mailboxSession), new ConversationTreeFactory(mailboxSession, ConversationTreeNodeFactory.ConversationFamilyTreeNodeIndexPropertyDefinition), new ConversationDataExtractorFactory(XSOFactory.Default, mailboxSession))
		{
		}

		public ThreadedConversationFactory(IMailboxSession mailboxSession, IConversationMembersQuery membersQuery, IConversationTreeFactory conversationTreeFactory, IConversationTreeFactory conversationThreadTreeFactory, IConversationDataExtractorFactory dataExtractorFactory)
		{
			this.mailboxSession = mailboxSession;
			this.membersQuery = membersQuery;
			this.conversationTreeFactory = conversationTreeFactory;
			this.conversationThreadTreeFactory = conversationThreadTreeFactory;
			this.dataExtractorFactory = dataExtractorFactory;
		}

		public IThreadedConversation CreateConversation(ConversationId conversationId, IList<StoreObjectId> folderIds, bool useFolderIdsAsExclusionList, bool isIrmEnabled, params PropertyDefinition[] requestedItemProperties)
		{
			return this.CreateConversation(conversationId, folderIds, useFolderIdsAsExclusionList, isIrmEnabled, false, null, requestedItemProperties);
		}

		public IThreadedConversation CreateConversation(ConversationId conversationId, IList<StoreObjectId> folderIds, bool useFolderIdsAsExclusionList, bool isIrmEnabled, bool isSmimeSupported, string domainName, params PropertyDefinition[] requestedItemProperties)
		{
			ICollection<PropertyDefinition> defaultThreadProperties = ThreadedConversationFactory.DefaultThreadProperties;
			HashSet<PropertyDefinition> hashSet = this.SanitizePropertiesRequested(requestedItemProperties, defaultThreadProperties);
			Dictionary<object, List<IStorePropertyBag>> aggregatedMessages = this.QueryMessages(conversationId, folderIds, useFolderIdsAsExclusionList, hashSet);
			IConversationTree conversationTree = this.CreateTree(hashSet, aggregatedMessages);
			ConversationStateFactory stateFactory = this.CreateStateFactory(conversationTree);
			List<IConversationTree> threadTrees = this.CreateThreadTrees(hashSet, aggregatedMessages);
			ConversationDataExtractor conversationDataExtractor = this.dataExtractorFactory.Create(isIrmEnabled, hashSet, conversationId, conversationTree, isSmimeSupported, domainName);
			return this.InternalCreateConversation(conversationId, conversationDataExtractor, stateFactory, conversationTree, threadTrees, defaultThreadProperties);
		}

		public IThreadedConversation CreateConversation(ConversationId conversationId, params PropertyDefinition[] requestedProperties)
		{
			return this.CreateConversation(conversationId, null, false, false, requestedProperties);
		}

		private ConversationStateFactory CreateStateFactory(IConversationTree conversationTree)
		{
			return new ConversationStateFactory(this.mailboxSession, conversationTree);
		}

		private IConversationTree CreateTree(ICollection<PropertyDefinition> propertyDefinitions, Dictionary<object, List<IStorePropertyBag>> aggregatedMessages)
		{
			return this.conversationTreeFactory.Create(aggregatedMessages.SelectMany((KeyValuePair<object, List<IStorePropertyBag>> d) => d.Value).ToList<IStorePropertyBag>(), propertyDefinitions);
		}

		private List<IConversationTree> CreateThreadTrees(ICollection<PropertyDefinition> propertyDefinitions, Dictionary<object, List<IStorePropertyBag>> aggregatedMessages)
		{
			List<IConversationTree> list = new List<IConversationTree>();
			foreach (KeyValuePair<object, List<IStorePropertyBag>> keyValuePair in aggregatedMessages)
			{
				IConversationTree item = this.conversationThreadTreeFactory.Create(keyValuePair.Value, propertyDefinitions);
				list.Add(item);
			}
			return list;
		}

		private IThreadedConversation InternalCreateConversation(ConversationId conversationId, ConversationDataExtractor conversationDataExtractor, ConversationStateFactory stateFactory, IConversationTree tree, List<IConversationTree> threadTrees, ICollection<PropertyDefinition> requestedThreadProperties)
		{
			IList<IConversationThread> conversationThreads = this.CreateConversationThreads(tree, threadTrees, conversationDataExtractor, requestedThreadProperties);
			return new ThreadedConversation(stateFactory, conversationDataExtractor, conversationId, tree, conversationThreads);
		}

		private IList<IConversationThread> CreateConversationThreads(IConversationTree tree, List<IConversationTree> threadTrees, ConversationDataExtractor conversationDataExtractor, ICollection<PropertyDefinition> requestedThreadProperties)
		{
			bool isSingleThreadConversation = threadTrees.Count == 1;
			ConversationThreadDataExtractor threadDataExtractor = new ConversationThreadDataExtractor(requestedThreadProperties, tree, isSingleThreadConversation);
			IList<IConversationThread> list = new List<IConversationThread>();
			foreach (IConversationTree threadTree in threadTrees)
			{
				ConversationThread item = new ConversationThread(conversationDataExtractor, threadDataExtractor, threadTree, this.conversationTreeFactory);
				list.Add(item);
			}
			return list;
		}

		private HashSet<PropertyDefinition> SanitizePropertiesRequested(PropertyDefinition[] requestedItemProperties, ICollection<PropertyDefinition> requestedThreadProperties)
		{
			ICollection<PropertyDefinition> requestedProperties = InternalSchema.Combine<PropertyDefinition>(requestedItemProperties, requestedThreadProperties);
			return this.conversationThreadTreeFactory.CalculatePropertyDefinitionsToBeLoaded(requestedProperties);
		}

		private Dictionary<object, List<IStorePropertyBag>> QueryMessages(ConversationId conversationId, IList<StoreObjectId> folderIds, bool useFolderIdsAsExclusionList, ICollection<PropertyDefinition> propertyDefinitions)
		{
			List<IStorePropertyBag> members = this.membersQuery.Query(conversationId, propertyDefinitions, folderIds, useFolderIdsAsExclusionList);
			return this.membersQuery.AggregateMembersPerField(ItemSchema.ConversationFamilyId, null, members);
		}

		private static ICollection<PropertyDefinition> DefaultThreadProperties = AggregatedConversationSchema.Instance.AllProperties;

		private readonly IMailboxSession mailboxSession;

		private readonly IConversationMembersQuery membersQuery;

		private readonly IConversationTreeFactory conversationTreeFactory;

		private readonly IConversationTreeFactory conversationThreadTreeFactory;

		private readonly IConversationDataExtractorFactory dataExtractorFactory;
	}
}
