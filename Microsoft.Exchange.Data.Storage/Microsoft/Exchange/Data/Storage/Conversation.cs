using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using Microsoft.Exchange.Data.Search.AqsParser;
using Microsoft.Exchange.Data.Storage.Conversations;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.StructuredQuery;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class Conversation : IConversation, ICoreConversation, IConversationData
	{
		internal Conversation(ConversationId conversationId, IConversationTree conversationTree, MailboxSession session, ConversationDataExtractor conversationDataExtractor, IConversationTreeFactory conversationTreeFactory, ConversationStateFactory stateFactory)
		{
			this.conversationId = conversationId;
			this.conversationTree = conversationTree;
			this.session = session;
			this.conversationDataExtractor = conversationDataExtractor;
			this.conversationTreeFactory = conversationTreeFactory;
			this.stateFactory = stateFactory;
		}

		public static Conversation Load(MailboxSession session, ConversationId conversationId, IList<StoreObjectId> folderIds, bool useFolderIdsAsExclusionList, bool isIrmEnabled, params PropertyDefinition[] propertyDefinitions)
		{
			ConversationFactory conversationFactory = new ConversationFactory(session);
			return conversationFactory.CreateConversation(conversationId, folderIds, useFolderIdsAsExclusionList, isIrmEnabled, propertyDefinitions);
		}

		internal static Conversation Load(MailboxSession session, ConversationId conversationId, IList<StoreObjectId> foldersToIgnore, bool isIrmEnabled, params PropertyDefinition[] propertyDefinitions)
		{
			return Conversation.Load(session, conversationId, foldersToIgnore, true, isIrmEnabled, propertyDefinitions);
		}

		public static Conversation Load(MailboxSession session, ConversationId conversationId, IList<StoreObjectId> foldersToIgnore, params PropertyDefinition[] propertyDefinitions)
		{
			return Conversation.Load(session, conversationId, foldersToIgnore, false, propertyDefinitions);
		}

		public static Conversation Load(MailboxSession session, ConversationId conversationId, params PropertyDefinition[] propertyDefinitions)
		{
			return Conversation.Load(session, conversationId, null, false, propertyDefinitions);
		}

		public static Conversation Load(MailboxSession session, ConversationId conversationId, bool isIrmEnabled, params PropertyDefinition[] propertyDefinitions)
		{
			return Conversation.Load(session, conversationId, null, isIrmEnabled, propertyDefinitions);
		}

		public static long MaxBytesForConversation
		{
			get
			{
				return ConversationDataExtractor.MaxBytesForConversation;
			}
			set
			{
				ConversationDataExtractor.MaxBytesForConversation = value;
			}
		}

		public ItemPart GetItemPart(StoreObjectId itemId)
		{
			return this.ConversationDataExtractor.GetItemPart(this.ConversationTree, itemId);
		}

		public StoreObjectId RootMessageId
		{
			get
			{
				if (this.ConversationTree.RootMessageNode != null)
				{
					return this.ConversationTree.RootMessageNode.MainStoreObjectId;
				}
				return null;
			}
		}

		public void LoadBodySummaries()
		{
			this.ConversationDataExtractor.LoadBodySummaries(this.ConversationTree);
		}

		public void LoadItemParts(ICollection<IConversationTreeNode> nodes)
		{
			Util.ThrowOnNullOrEmptyArgument(nodes, "nodes");
			this.ConversationDataExtractor.LoadItemParts(this.ConversationTree, from node in nodes
			select node.MainStoreObjectId);
		}

		public void LoadItemParts(ICollection<StoreObjectId> storeIds)
		{
			Util.ThrowOnNullOrEmptyArgument(storeIds, "storeIds");
			this.ConversationDataExtractor.LoadItemParts(this.ConversationTree, storeIds);
		}

		public void TrimToNewest(int count)
		{
			if (this.ConversationDataExtractor.ConversationDataLoaded)
			{
				throw new InvalidOperationException("LoadItemParts or LoadBodySummary already called");
			}
			this.conversationTree = this.conversationTreeFactory.GetNewestSubTree(this.conversationTree, count);
		}

		public void LoadItemParts(IList<StoreObjectId> storeIds, string searchString, CultureInfo cultureinfo, out List<IConversationTreeNode> nodes)
		{
			Util.ThrowOnNullOrEmptyArgument(storeIds, "storeIds");
			this.LoadItemParts(this.conversationTree, storeIds, searchString, cultureinfo, out nodes);
		}

		public void LoadItemParts(IConversationTree conversationTree, IList<StoreObjectId> storeIds, string searchString, CultureInfo cultureinfo, out List<IConversationTreeNode> nodes)
		{
			Util.ThrowOnNullOrEmptyArgument(storeIds, "storeIds");
			nodes = new List<IConversationTreeNode>(0);
			this.ConversationDataExtractor.LoadItemParts(conversationTree, new Collection<StoreObjectId>(storeIds));
			if (string.IsNullOrEmpty(searchString))
			{
				nodes = this.MatchAllNodes(storeIds);
				return;
			}
			IList<string> words = null;
			if (this.InternalTryAqsMatch(storeIds, searchString, cultureinfo, out words, out nodes))
			{
				return;
			}
			bool flag = false;
			foreach (StoreObjectId storeObjectId in storeIds)
			{
				ItemPart itemPart = this.ConversationDataExtractor.GetItemPart(conversationTree, storeObjectId);
				if (itemPart.UniqueFragmentInfo != null && itemPart.UniqueFragmentInfo.IsMatchFound(words))
				{
					IConversationTreeNode item = null;
					if (conversationTree.TryGetConversationTreeNode(storeObjectId, out item))
					{
						nodes.Add(item);
						flag = true;
					}
				}
			}
			if (!flag)
			{
				nodes = this.MatchAllNodes(storeIds);
			}
		}

		public KeyValuePair<List<StoreObjectId>, List<StoreObjectId>> CalculateChanges(byte[] olderState)
		{
			return this.ConversationState.CalculateChanges(olderState);
		}

		public AggregateOperationResult AlwaysDelete(bool enable, bool processItems)
		{
			AggregateOperationResult result = null;
			using (ConversationActionItem associatedActionItem = this.GetAssociatedActionItem())
			{
				associatedActionItem.AlwaysDeleteValue = enable;
				associatedActionItem.ConversationActionMaxDeliveryTime = ExDateTime.MinValue;
				if (enable && processItems)
				{
					result = associatedActionItem.ProcessItems(ConversationAction.AlwaysDelete, this);
				}
				associatedActionItem.Save(SaveMode.ResolveConflicts);
			}
			return result;
		}

		public AggregateOperationResult AlwaysMove(StoreObjectId folderId, bool processItems)
		{
			AggregateOperationResult result = null;
			using (ConversationActionItem associatedActionItem = this.GetAssociatedActionItem())
			{
				associatedActionItem.AlwaysMoveValue = folderId;
				if (folderId != null && processItems)
				{
					result = associatedActionItem.ProcessItems(ConversationAction.AlwaysMove, this);
				}
				associatedActionItem.Save(SaveMode.ResolveConflicts);
			}
			return result;
		}

		public AggregateOperationResult AlwaysCategorize(string[] categories, bool processItems)
		{
			AggregateOperationResult result = null;
			using (ConversationActionItem associatedActionItem = this.GetAssociatedActionItem())
			{
				associatedActionItem.AlwaysCategorizeValue = categories;
				if (categories != null && processItems)
				{
					result = associatedActionItem.ProcessItems(ConversationAction.AlwaysCategorize, this);
				}
				associatedActionItem.Save(SaveMode.ResolveConflicts);
			}
			return result;
		}

		public AggregateOperationResult AlwaysClutterOrUnclutter(bool? clutterOrUnclutter, bool processItems)
		{
			AggregateOperationResult result = null;
			using (ConversationActionItem associatedActionItem = this.GetAssociatedActionItem())
			{
				associatedActionItem.AlwaysClutterOrUnclutterValue = clutterOrUnclutter;
				if (clutterOrUnclutter != null && processItems)
				{
					result = associatedActionItem.ProcessItems(ConversationAction.AlwaysClutterOrUnclutter, this);
				}
				associatedActionItem.Save(SaveMode.ResolveConflicts);
			}
			return result;
		}

		public AggregateOperationResult Delete(StoreObjectId contextFolderId, DateTime? actionTime, DeleteItemFlags deleteFlags)
		{
			return this.Delete(contextFolderId, actionTime, deleteFlags, false);
		}

		public AggregateOperationResult Delete(StoreObjectId contextFolderId, DateTime? actionTime, DeleteItemFlags deleteFlags, bool returnNewItemIds)
		{
			EnumValidator.ThrowIfInvalid<DeleteItemFlags>(deleteFlags, "deleteFlags");
			List<StoreObjectId> filteredItemIds = this.GetFilteredItemIds(contextFolderId, actionTime, null);
			return this.session.Delete(deleteFlags, returnNewItemIds, filteredItemIds.ToArray());
		}

		public AggregateOperationResult ClearFlags(StoreObjectId contextFolderId, DateTime? actionTime)
		{
			StoreObjectId[] array = this.GetFilteredItemIds(contextFolderId, actionTime, (IStorePropertyBag propertyBag) => !propertyBag.TryGetProperty(ItemSchema.FlagStatus).Equals(FlagStatus.NotFlagged)).ToArray();
			List<GroupOperationResult> groupOperationResults = new List<GroupOperationResult>(array.Length);
			StoreObjectId[] array2 = array;
			for (int i = 0; i < array2.Length; i++)
			{
				StoreObjectId itemId = array2[i];
				Folder.ExecuteGroupOperationAndAggregateResults(groupOperationResults, new StoreObjectId[]
				{
					itemId
				}, () => this.PerformFlaggingOperation(itemId, delegate(Item item)
				{
					item.ClearFlag();
				}));
			}
			return Folder.CreateAggregateOperationResult(groupOperationResults);
		}

		public AggregateOperationResult SetFlags(StoreObjectId contextFolderId, DateTime? actionTime, string flagRequest, ExDateTime? startDate, ExDateTime? dueDate)
		{
			List<StoreObjectId> filteredItemIds = this.GetFilteredItemIds(contextFolderId, actionTime, null);
			StoreObjectId itemIdToFlag = this.GetMostRecentItem(filteredItemIds);
			List<GroupOperationResult> groupOperationResults = new List<GroupOperationResult>(1);
			if (itemIdToFlag != null)
			{
				Folder.ExecuteGroupOperationAndAggregateResults(groupOperationResults, new StoreObjectId[]
				{
					itemIdToFlag
				}, () => this.PerformFlaggingOperation(itemIdToFlag, delegate(Item item)
				{
					item.SetFlag(flagRequest, startDate, dueDate);
				}));
			}
			return Folder.CreateAggregateOperationResult(groupOperationResults);
		}

		public AggregateOperationResult CompleteFlags(StoreObjectId contextFolderId, DateTime? actionTime, ExDateTime? completeDate)
		{
			Conversation.<>c__DisplayClass15 CS$<>8__locals1 = new Conversation.<>c__DisplayClass15();
			CS$<>8__locals1.completeDate = completeDate;
			CS$<>8__locals1.<>4__this = this;
			List<StoreObjectId> filteredItemIds = this.GetFilteredItemIds(contextFolderId, actionTime, (IStorePropertyBag propertyBag) => propertyBag.TryGetProperty(ItemSchema.FlagStatus).Equals(FlagStatus.Flagged));
			List<GroupOperationResult> groupOperationResults = null;
			if (filteredItemIds.Count > 0)
			{
				groupOperationResults = new List<GroupOperationResult>(filteredItemIds.Count);
				using (List<StoreObjectId>.Enumerator enumerator = filteredItemIds.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						StoreObjectId itemId = enumerator.Current;
						Folder.ExecuteGroupOperationAndAggregateResults(groupOperationResults, new StoreObjectId[]
						{
							itemId
						}, () => CS$<>8__locals1.<>4__this.PerformFlaggingOperation(itemId, delegate(Item item)
						{
							item.CompleteFlag(CS$<>8__locals1.completeDate);
						}));
					}
					goto IL_12F;
				}
			}
			filteredItemIds = this.GetFilteredItemIds(contextFolderId, actionTime, null);
			groupOperationResults = new List<GroupOperationResult>(filteredItemIds.Count);
			StoreObjectId itemIdToFlag = this.GetMostRecentItem(filteredItemIds);
			if (itemIdToFlag != null)
			{
				Folder.ExecuteGroupOperationAndAggregateResults(groupOperationResults, new StoreObjectId[]
				{
					itemIdToFlag
				}, () => CS$<>8__locals1.<>4__this.PerformFlaggingOperation(itemIdToFlag, delegate(Item item)
				{
					item.CompleteFlag(CS$<>8__locals1.completeDate);
				}));
			}
			IL_12F:
			return Folder.CreateAggregateOperationResult(groupOperationResults);
		}

		public AggregateOperationResult Copy(StoreObjectId contextFolderId, DateTime? actionTime, StoreSession destinationSession, StoreObjectId destinationFolderId)
		{
			StoreObjectId[] ids = this.GetFilteredItemIds(contextFolderId, actionTime, null).ToArray();
			if (destinationSession != null)
			{
				return this.session.Copy(destinationSession, destinationFolderId, ids);
			}
			return this.session.Copy(destinationFolderId, ids);
		}

		public AggregateOperationResult Move(StoreObjectId contextFolderId, DateTime? actionTime, StoreSession destinationSession, StoreObjectId destinationFolderId)
		{
			return this.Move(contextFolderId, actionTime, destinationSession, destinationFolderId, false);
		}

		public AggregateOperationResult Move(StoreObjectId contextFolderId, DateTime? actionTime, StoreSession destinationSession, StoreObjectId destinationFolderId, bool returnNewItemIds)
		{
			StoreObjectId[] ids = this.GetFilteredItemIds(contextFolderId, actionTime, null).ToArray();
			if (destinationSession != null)
			{
				return this.session.Move(destinationSession, destinationFolderId, returnNewItemIds, ids);
			}
			return this.session.Move(destinationFolderId, ids);
		}

		public AggregateOperationResult SetIsReadState(StoreObjectId contextFolderId, DateTime? actionTime, bool isRead, bool suppressReadReceipts)
		{
			StoreObjectId[] array = this.GetFilteredItemIds(contextFolderId, actionTime, (IStorePropertyBag propertyBag) => propertyBag.TryGetProperty(MessageItemSchema.IsRead).Equals(!isRead)).ToArray();
			if (isRead)
			{
				this.session.MarkAsRead(suppressReadReceipts, array);
			}
			else
			{
				this.session.MarkAsUnread(suppressReadReceipts, array);
			}
			return new AggregateOperationResult(OperationResult.Succeeded, new GroupOperationResult[]
			{
				new GroupOperationResult(OperationResult.Succeeded, array, null)
			});
		}

		public AggregateOperationResult SetRetentionPolicy(StoreObjectId contextFolderId, DateTime? actionTime, PolicyTag policyTag, bool isArchiveAction)
		{
			StoreObjectId[] array = this.GetFilteredItemIds(contextFolderId, actionTime, null).ToArray();
			List<GroupOperationResult> groupOperationResults = new List<GroupOperationResult>(array.Length);
			if (array != null)
			{
				StoreObjectId[] array2 = array;
				for (int i = 0; i < array2.Length; i++)
				{
					StoreObjectId itemId = array2[i];
					Folder.ExecuteGroupOperationAndAggregateResults(groupOperationResults, new StoreObjectId[]
					{
						itemId
					}, () => this.SetRetentionPolicyInternal(itemId, policyTag, isArchiveAction));
				}
			}
			return Folder.CreateAggregateOperationResult(groupOperationResults);
		}

		public ParticipantTable LoadReplyAllParticipantsPerType()
		{
			return this.ConversationDataExtractor.LoadReplyAllParticipantsPerType(this.conversationTree);
		}

		public ParticipantSet LoadReplyAllParticipants(IConversationTreeNode node)
		{
			return this.ConversationDataExtractor.LoadReplyAllParticipants(this.conversationTree, node);
		}

		public IConversationTreeNode RootMessageNode
		{
			get
			{
				return this.conversationTree.RootMessageNode;
			}
		}

		public int GetNodeCount(bool includeSubmitted)
		{
			return this.conversationTree.GetNodeCount(includeSubmitted);
		}

		public IConversationTreeNode FirstNode
		{
			get
			{
				return this.RootMessageNode;
			}
		}

		public Dictionary<IConversationTreeNode, ParticipantSet> LoadAddedParticipants()
		{
			return this.ConversationDataExtractor.LoadAddedParticipants(this.conversationTree);
		}

		public Dictionary<IConversationTreeNode, IConversationTreeNode> BuildPreviousNodeGraph()
		{
			return this.conversationTree.BuildPreviousNodeGraph();
		}

		public IConversationTree GetNewestSubTree(int count)
		{
			return this.conversationTreeFactory.GetNewestSubTree(this.conversationTree, count);
		}

		public List<StoreObjectId> GetMessageIdsForPreread()
		{
			return this.ConversationDataExtractor.GetMessageIdsForPreread(this.ConversationTree);
		}

		public bool ConversationNodeContainedInChildren(IConversationTreeNode node)
		{
			Util.ThrowOnNullOrEmptyArgument(node, "node");
			return this.ConversationDataExtractor.ConversationNodeContainedInChildren(this.conversationTree, node);
		}

		public byte[] ConversationCreatorSID
		{
			get
			{
				return this.conversationTree.ConversationCreatorSID;
			}
		}

		public ConversationId ConversationId
		{
			get
			{
				return this.conversationId;
			}
		}

		public IConversationTree ConversationTree
		{
			get
			{
				return this.conversationTree;
			}
		}

		public string Topic
		{
			get
			{
				return this.ConversationTree.Topic;
			}
		}

		public EffectiveRights EffectiveRights
		{
			get
			{
				return this.ConversationTree.EffectiveRights;
			}
		}

		public event OnBeforeItemLoadEventDelegate OnBeforeItemLoad
		{
			add
			{
				this.ConversationDataExtractor.OnBeforeItemLoad += value;
			}
			remove
			{
				this.ConversationDataExtractor.OnBeforeItemLoad -= value;
			}
		}

		public OptimizationInfo OptimizationCounters
		{
			get
			{
				return this.ConversationDataExtractor.OptimizationCounters;
			}
		}

		public IConversationStatistics ConversationStatistics
		{
			get
			{
				return this.OptimizationCounters;
			}
		}

		private ConversationState ConversationState
		{
			get
			{
				if (this.conversationState == null)
				{
					this.conversationState = this.stateFactory.Create(null);
				}
				return this.conversationState;
			}
		}

		public byte[] SerializedTreeState
		{
			get
			{
				return this.ConversationState.SerializedState;
			}
		}

		protected ConversationDataExtractor ConversationDataExtractor
		{
			get
			{
				return this.conversationDataExtractor;
			}
		}

		public byte[] GetSerializedTreeStateWithNodesToExclude(ICollection<IConversationTreeNode> nodesToExclude)
		{
			ConversationState conversationState = this.stateFactory.Create(nodesToExclude);
			return conversationState.SerializedState;
		}

		public ParticipantSet AllParticipants(ICollection<IConversationTreeNode> loadedNodes = null)
		{
			return this.ConversationDataExtractor.CalculateAllRecipients(this.ConversationTree, loadedNodes);
		}

		protected List<StoreObjectId> GetFilteredItemIds(StoreObjectId localFolderId, DateTime? createdBefore, Func<IStorePropertyBag, bool> filter)
		{
			HashSet<StoreObjectId> localItemIds = this.GetLocalItemIds(localFolderId);
			List<StoreObjectId> list = new List<StoreObjectId>();
			foreach (IConversationTreeNode conversationTreeNode in this.ConversationTree)
			{
				foreach (IStorePropertyBag storePropertyBag in conversationTreeNode.StorePropertyBags)
				{
					StoreObjectId objectId = ((VersionedId)storePropertyBag.TryGetProperty(ItemSchema.Id)).ObjectId;
					if (localItemIds.Contains(objectId))
					{
						if (createdBefore != null && createdBefore != null)
						{
							ExDateTime? exDateTime = storePropertyBag.TryGetProperty(ItemSchema.ReceivedTime) as ExDateTime?;
							if (exDateTime == null || (exDateTime.Value.UniversalTime - createdBefore.Value.ToUniversalTime()).TotalSeconds > 1.0)
							{
								continue;
							}
						}
						if (filter == null || filter(storePropertyBag))
						{
							list.Add(objectId);
						}
					}
				}
			}
			return list;
		}

		private GroupOperationResult PerformFlaggingOperation(StoreObjectId itemId, Action<Item> flaggingAction)
		{
			using (Item item = Item.Bind(this.session, itemId, Conversation.flaggingProperties))
			{
				item.OpenAsReadWrite();
				flaggingAction(item);
				item.Save(SaveMode.ResolveConflicts);
			}
			return new GroupOperationResult(OperationResult.Succeeded, new StoreObjectId[]
			{
				itemId
			}, null);
		}

		private HashSet<StoreObjectId> GetLocalItemIds(StoreObjectId localFolderId)
		{
			HashSet<StoreObjectId> hashSet = new HashSet<StoreObjectId>();
			foreach (IConversationTreeNode conversationTreeNode in this.ConversationTree)
			{
				foreach (StoreObjectId storeObjectId in conversationTreeNode.ToListStoreObjectId())
				{
					StoreObjectId valueOrDefault = conversationTreeNode.GetValueOrDefault<StoreObjectId>(storeObjectId, StoreObjectSchema.ParentItemId, null);
					if (localFolderId == null || localFolderId.CompareTo(valueOrDefault) == 0)
					{
						hashSet.Add(storeObjectId);
					}
				}
			}
			if (hashSet.Count == 0 && localFolderId != null)
			{
				using (Folder folder = Folder.Bind(this.session, localFolderId))
				{
					SearchFolder searchFolder = folder as SearchFolder;
					if (searchFolder != null)
					{
						using (QueryResult queryResult = searchFolder.ConversationItemQuery(null, new SortBy[]
						{
							new SortBy(ConversationItemSchema.ConversationId, SortOrder.Ascending)
						}, new PropertyDefinition[]
						{
							ConversationItemSchema.ConversationItemIds
						}))
						{
							if (queryResult.SeekToCondition(SeekReference.OriginBeginning, new ComparisonFilter(ComparisonOperator.Equal, ConversationItemSchema.ConversationId, this.ConversationId)))
							{
								IStorePropertyBag[] propertyBags = queryResult.GetPropertyBags(1);
								if (propertyBags.Length > 0)
								{
									StoreObjectId[] array = propertyBags[0].TryGetProperty(ConversationItemSchema.ConversationItemIds) as StoreObjectId[];
									if (array != null)
									{
										foreach (StoreObjectId item in array)
										{
											hashSet.Add(item);
										}
									}
								}
							}
						}
					}
				}
			}
			return hashSet;
		}

		public void AggregateHeaders(params IAggregatorRule[] aggregationRules)
		{
			Util.ThrowOnNullArgument(aggregationRules, "aggregationRules");
			foreach (IAggregatorRule aggregatorRule in aggregationRules)
			{
				aggregatorRule.BeginAggregation();
			}
			foreach (IConversationTreeNode conversationTreeNode in this.ConversationTree)
			{
				foreach (IAggregatorRule aggregatorRule2 in aggregationRules)
				{
					if (conversationTreeNode.HasData)
					{
						aggregatorRule2.AddToAggregation(conversationTreeNode.MainPropertyBag);
					}
				}
			}
			foreach (IAggregatorRule aggregatorRule3 in aggregationRules)
			{
				aggregatorRule3.EndAggregation();
			}
		}

		private ConversationActionItem GetAssociatedActionItem()
		{
			ConversationActionItem result = null;
			try
			{
				result = ConversationActionItem.Bind(this.session, this.ConversationId);
			}
			catch (ObjectNotFoundException)
			{
				string conversationTopic = (this.Topic == null) ? string.Empty : this.Topic;
				result = ConversationActionItem.Create(this.session, this.ConversationId, conversationTopic);
			}
			return result;
		}

		private List<IConversationTreeNode> MatchAllNodes(IList<StoreObjectId> storeIds)
		{
			List<IConversationTreeNode> list = new List<IConversationTreeNode>(0);
			foreach (StoreObjectId storeObjectId in storeIds)
			{
				IConversationTreeNode item = null;
				if (this.ConversationTree.TryGetConversationTreeNode(storeObjectId, out item))
				{
					list.Add(item);
				}
			}
			return list;
		}

		private GroupOperationResult SetRetentionPolicyInternal(StoreObjectId itemId, PolicyTag policyTag, bool isArchiveAction)
		{
			PropertyDefinition[] propsToReturn = isArchiveAction ? PolicyTagHelper.ArchiveProperties : PolicyTagHelper.RetentionProperties;
			using (Item item = Item.Bind(this.session, itemId, propsToReturn))
			{
				item.OpenAsReadWrite();
				if (isArchiveAction)
				{
					if (policyTag == null)
					{
						PolicyTagHelper.ClearPolicyTagForArchiveOnItem(item);
					}
					else
					{
						PolicyTagHelper.SetPolicyTagForArchiveOnItem(policyTag, item);
					}
				}
				else if (policyTag == null)
				{
					PolicyTagHelper.ClearPolicyTagForDeleteOnItem(item);
				}
				else
				{
					PolicyTagHelper.SetPolicyTagForDeleteOnItem(policyTag, item);
				}
				item.Save(SaveMode.ResolveConflicts);
			}
			return new GroupOperationResult(OperationResult.Succeeded, new StoreObjectId[]
			{
				itemId
			}, null);
		}

		private StoreObjectId GetMostRecentItem(List<StoreObjectId> itemIds)
		{
			StoreObjectId storeObjectId = null;
			if (itemIds.Count == 1)
			{
				storeObjectId = itemIds[0];
			}
			else
			{
				ExDateTime t = ExDateTime.MinValue;
				foreach (StoreObjectId storeObjectId2 in itemIds)
				{
					ExDateTime valueOrDefault = this.ConversationTree.GetValueOrDefault<ExDateTime>(storeObjectId2, ItemSchema.ReceivedTime, ExDateTime.MinValue);
					if (valueOrDefault > t)
					{
						storeObjectId = storeObjectId2;
						t = valueOrDefault;
					}
				}
				if (storeObjectId == null && itemIds.Count > 0)
				{
					storeObjectId = itemIds[0];
				}
			}
			return storeObjectId;
		}

		private static string PredicateToBodyQueryString(LeafCondition leafCond)
		{
			if (!AqsParser.PropertyKeywordMap.ContainsKey(leafCond.PropertyName))
			{
				return null;
			}
			PropertyKeyword? propertyKeyword = new PropertyKeyword?(AqsParser.PropertyKeywordMap[leafCond.PropertyName]);
			if (propertyKeyword == null)
			{
				return null;
			}
			if (propertyKeyword.Value == PropertyKeyword.All || propertyKeyword.Value == PropertyKeyword.Body)
			{
				return (string)leafCond.Value;
			}
			return null;
		}

		private static List<string> ConditionToBodyQueryString(Condition condition)
		{
			List<string> list = new List<string>();
			if (condition.Type == null || condition.Type == 1)
			{
				using (List<Condition>.Enumerator enumerator = ((CompoundCondition)condition).Children.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						Condition condition2 = enumerator.Current;
						List<string> list2 = Conversation.ConditionToBodyQueryString(condition2);
						if (list2.Count > 0)
						{
							list.AddRange(list2);
						}
					}
					return list;
				}
			}
			if (condition.Type == 2)
			{
				List<string> list3 = Conversation.ConditionToBodyQueryString(((NegationCondition)condition).Child);
				if (list3.Count > 0)
				{
					list.AddRange(list3);
				}
			}
			else
			{
				if (condition.Type != 3)
				{
					throw new ArgumentException("No condition node other than NOT, AND, OR and Leaf is allowed.");
				}
				string text = Conversation.PredicateToBodyQueryString((LeafCondition)condition);
				if (!string.IsNullOrEmpty(text))
				{
					list.Add(text);
				}
			}
			return list;
		}

		private bool InternalTryAqsMatch(IList<StoreObjectId> storeIds, string searchString, CultureInfo cultureinfo, out IList<string> bodySearchString, out List<IConversationTreeNode> nodes)
		{
			nodes = null;
			bodySearchString = null;
			AqsParser aqsParser = new AqsParser();
			using (Condition condition = aqsParser.Parse(searchString, AqsParser.ParseOption.SuppressError, cultureinfo))
			{
				bodySearchString = Conversation.ConditionToBodyQueryString(condition);
				if (bodySearchString == null || bodySearchString.Count < 1)
				{
					nodes = this.MatchAllNodes(storeIds);
					return true;
				}
			}
			return false;
		}

		private const double MaxTimeForRoundTripThroughWebServices = 1.0;

		private static readonly PropertyDefinition[] flaggingProperties = new PropertyDefinition[]
		{
			ItemSchema.FlagStatus,
			TaskSchema.StartDate,
			TaskSchema.DueDate,
			ItemSchema.CompleteDate
		};

		private readonly ConversationId conversationId;

		private readonly MailboxSession session;

		private readonly ConversationDataExtractor conversationDataExtractor;

		private readonly IConversationTreeFactory conversationTreeFactory;

		private readonly ConversationStateFactory stateFactory;

		private IConversationTree conversationTree;

		private ConversationState conversationState;
	}
}
