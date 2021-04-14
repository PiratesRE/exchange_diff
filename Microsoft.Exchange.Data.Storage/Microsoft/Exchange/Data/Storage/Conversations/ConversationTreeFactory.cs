using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Storage;
using Microsoft.Exchange.Extensions;

namespace Microsoft.Exchange.Data.Storage.Conversations
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class ConversationTreeFactory : IConversationTreeFactory
	{
		public ConversationTreeFactory(IMailboxSession mailboxSession) : this(mailboxSession, ConversationTreeNodeFactory.DefaultTreeNodeIndexPropertyDefinition)
		{
		}

		public ConversationTreeFactory(IMailboxSession mailboxSession, PropertyDefinition indexPropertyDefinition)
		{
			this.treeNodeFactory = new ConversationTreeNodeFactory(indexPropertyDefinition);
			this.mailboxSession = mailboxSession;
		}

		public HashSet<PropertyDefinition> CalculatePropertyDefinitionsToBeLoaded(ICollection<PropertyDefinition> requestedProperties)
		{
			ICollection<PropertyDefinition> collection = InternalSchema.Combine<PropertyDefinition>(ConversationTreeFactory.RequiredBuildTreeProperties, requestedProperties);
			HashSet<PropertyDefinition> hashSet = new HashSet<PropertyDefinition>(collection);
			hashSet.ExceptWith(ConversationDataExtractor.BodyPropertiesCanBeExtracted);
			return hashSet;
		}

		public IConversationTree Create(IEnumerable<IStorePropertyBag> queryResult, IEnumerable<PropertyDefinition> propertyDefinitions)
		{
			IConversationTreeNode rootNode = this.treeNodeFactory.CreateRootNode();
			if (queryResult == null)
			{
				return this.BuildTree(rootNode, null, null);
			}
			Dictionary<UniqueItemHash, List<IStorePropertyBag>> dictionary = this.AggregateDuplicates(queryResult);
			IList<IConversationTreeNode> nodes = this.InstantiateNodes(dictionary.Values);
			HashSet<PropertyDefinition> hashSet = new HashSet<PropertyDefinition>();
			if (propertyDefinitions != null)
			{
				hashSet.AddRange(propertyDefinitions);
			}
			return this.BuildTree(rootNode, nodes, hashSet);
		}

		private Dictionary<UniqueItemHash, List<IStorePropertyBag>> AggregateDuplicates(IEnumerable<IStorePropertyBag> propertyBags)
		{
			StoreObjectId defaultFolderId = this.MailboxSession.GetDefaultFolderId(DefaultFolderType.Inbox);
			StoreObjectId defaultFolderId2 = this.MailboxSession.GetDefaultFolderId(DefaultFolderType.SentItems);
			Dictionary<UniqueItemHash, List<IStorePropertyBag>> dictionary = new Dictionary<UniqueItemHash, List<IStorePropertyBag>>();
			foreach (IStorePropertyBag storePropertyBag in propertyBags)
			{
				StoreObjectId storeObjectId = storePropertyBag.TryGetProperty(StoreObjectSchema.ParentItemId) as StoreObjectId;
				UniqueItemHash key = UniqueItemHash.Create(storePropertyBag, storeObjectId.Equals(defaultFolderId2));
				if (dictionary.ContainsKey(key))
				{
					if (storeObjectId.Equals(defaultFolderId))
					{
						dictionary[key].Insert(0, storePropertyBag);
					}
					else if (storeObjectId.Equals(defaultFolderId2))
					{
						StoreObjectId storeObjectId2 = dictionary[key][0].TryGetProperty(StoreObjectSchema.ParentItemId) as StoreObjectId;
						if (storeObjectId2.Equals(defaultFolderId))
						{
							dictionary[key].Insert(1, storePropertyBag);
						}
						else
						{
							dictionary[key].Insert(0, storePropertyBag);
						}
					}
					else
					{
						dictionary[key].Add(storePropertyBag);
					}
				}
				else
				{
					dictionary.Add(key, new List<IStorePropertyBag>
					{
						storePropertyBag
					});
				}
			}
			return dictionary;
		}

		private IList<IConversationTreeNode> InstantiateNodes(ICollection<List<IStorePropertyBag>> propertyBagsOfTreeNodes)
		{
			List<IConversationTreeNode> list = new List<IConversationTreeNode>();
			foreach (List<IStorePropertyBag> storePropertyBags in propertyBagsOfTreeNodes)
			{
				list.Add(this.treeNodeFactory.CreateInstance(storePropertyBags));
			}
			return list;
		}

		private IConversationTree BuildTree(IConversationTreeNode rootNode, IList<IConversationTreeNode> nodes = null, HashSet<PropertyDefinition> loadedProperties = null)
		{
			nodes = (nodes ?? new List<IConversationTreeNode>());
			loadedProperties = (loadedProperties ?? new HashSet<PropertyDefinition>());
			ConversationTreeSortOrder sortOrder = ConversationTreeSortOrder.DeepTraversalAscending;
			foreach (IConversationTreeNode node in nodes)
			{
				rootNode.TryAddChild(node);
			}
			rootNode.SortChildNodes(sortOrder);
			return this.InternalInstantiate(sortOrder, rootNode, nodes, loadedProperties);
		}

		protected virtual IConversationTree InternalInstantiate(ConversationTreeSortOrder sortOrder, IConversationTreeNode rootNode, IList<IConversationTreeNode> nodes, HashSet<PropertyDefinition> loadedProperties)
		{
			return new ConversationTree(sortOrder, rootNode, nodes, loadedProperties);
		}

		public IConversationTree GetNewestSubTree(IConversationTree conversationTree, int count)
		{
			if (count <= 0)
			{
				throw new ArgumentException("Count should be greater than 0", "count");
			}
			ExTraceGlobals.ConversationTracer.TraceDebug<int, int>((long)this.GetHashCode(), "ConversationTreeFactory.GetNewestSubTree: count: {0}, tree size: {1}", count, conversationTree.Count);
			if (count >= conversationTree.Count)
			{
				return conversationTree;
			}
			IConversationTree trimmedConversationTree = null;
			ConversationTreeSortOrder sortOrder = ConversationTreeSortOrder.ChronologicalDescending;
			conversationTree.ExecuteSortedAction(sortOrder, delegate(ConversationTreeSortOrder treeOriginalSortOrder)
			{
				List<IStorePropertyBag> list = new List<IStorePropertyBag>(count);
				foreach (IConversationTreeNode conversationTreeNode in conversationTree)
				{
					if (count-- <= 0)
					{
						break;
					}
					list.AddRange(conversationTreeNode.StorePropertyBags);
				}
				trimmedConversationTree = this.Create(list, null);
				trimmedConversationTree.Sort(treeOriginalSortOrder);
			});
			return trimmedConversationTree;
		}

		protected IMailboxSession MailboxSession
		{
			get
			{
				return this.mailboxSession;
			}
		}

		private const ConversationTreeSortOrder DefaultSortOrder = ConversationTreeSortOrder.DeepTraversalAscending;

		public static readonly PropertyDefinition[] RequiredBuildTreeProperties = new PropertyDefinition[]
		{
			ItemSchema.Id,
			ItemSchema.BodyTag,
			InternalSchema.DisplayBccInternal,
			InternalSchema.DisplayCcInternal,
			InternalSchema.DisplayToInternal,
			ItemSchema.HasAttachment,
			ItemSchema.ConversationId,
			ItemSchema.ConversationFamilyId,
			StoreObjectSchema.ParentItemId,
			ItemSchema.ConversationTopic,
			ItemSchema.Subject,
			ItemSchema.ConversationIndex,
			ItemSchema.Categories,
			ItemSchema.ReceivedTime,
			ItemSchema.InternetMessageId,
			MessageItemSchema.IsDraft,
			StoreObjectSchema.IsRestricted,
			ItemSchema.ExchangeApplicationFlags,
			MessageItemSchema.ReplyToNames,
			ItemSchema.From,
			ItemSchema.Sender,
			StoreObjectSchema.ItemClass
		};

		private readonly ConversationTreeNodeFactory treeNodeFactory;

		private readonly IMailboxSession mailboxSession;
	}
}
