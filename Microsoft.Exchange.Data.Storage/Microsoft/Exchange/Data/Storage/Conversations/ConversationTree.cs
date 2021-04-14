using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Data.Storage.Conversations
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class ConversationTree : IConversationTree, ICollection<IConversationTreeNode>, IEnumerable<IConversationTreeNode>, IEnumerable
	{
		internal ConversationTree(ConversationTreeSortOrder sortOrder, IConversationTreeNode rootNode, IList<IConversationTreeNode> nodes, HashSet<PropertyDefinition> loadedProperties)
		{
			this.rootNode = rootNode;
			this.nodes = nodes;
			this.loadedProperties = loadedProperties;
			this.sortOrder = sortOrder;
		}

		public bool TryGetConversationTreeNode(StoreObjectId storeObjectId, out IConversationTreeNode conversationTreeNode)
		{
			return this.StoreIdToNode.TryGetValue(storeObjectId, out conversationTreeNode);
		}

		public int Count
		{
			get
			{
				return this.GetNodeCount(true);
			}
		}

		public int GetNodeCount(bool includeSubmitted)
		{
			if (includeSubmitted)
			{
				return this.nodes.Count;
			}
			return this.nodes.Count((IConversationTreeNode node) => !node.HasBeenSubmitted);
		}

		public string Topic
		{
			get
			{
				if (this.conversationTopic == null)
				{
					foreach (IConversationTreeNode conversationTreeNode in this)
					{
						this.conversationTopic = conversationTreeNode.GetValueOrDefault<string>(ItemSchema.ConversationTopic, null);
						if (!string.IsNullOrEmpty(this.conversationTopic))
						{
							break;
						}
					}
				}
				return this.conversationTopic;
			}
		}

		public byte[] ConversationCreatorSID
		{
			get
			{
				if (this.RootMessageNode != null)
				{
					return this.RootMessageNode.GetValueOrDefault<byte[]>(ItemSchema.ConversationCreatorSID, null);
				}
				return null;
			}
		}

		public EffectiveRights EffectiveRights
		{
			get
			{
				if (this.RootMessageNode != null)
				{
					return this.RootMessageNode.GetValueOrDefault<EffectiveRights>(StoreObjectSchema.EffectiveRights, EffectiveRights.None);
				}
				return EffectiveRights.None;
			}
		}

		public void Sort(ConversationTreeSortOrder sortOrder)
		{
			EnumValidator.ThrowIfInvalid<ConversationTreeSortOrder>(sortOrder, "sortOrder");
			this.sortOrder = sortOrder;
			this.rootNode.SortChildNodes(sortOrder);
		}

		public T GetValueOrDefault<T>(StoreObjectId itemId, PropertyDefinition propertyDefinition, T defaultValue = default(T))
		{
			IConversationTreeNode conversationTreeNode = null;
			if (this.TryGetConversationTreeNode(itemId, out conversationTreeNode))
			{
				return conversationTreeNode.GetValueOrDefault<T>(itemId, propertyDefinition, defaultValue);
			}
			throw new ArgumentException("No ConversationTreeNode can be found for the passed StoreObjectId");
		}

		public bool IsPropertyLoaded(PropertyDefinition propertyDefinition)
		{
			return this.loadedProperties.Contains(propertyDefinition);
		}

		public IEnumerable<IStorePropertyBag> StorePropertyBags
		{
			get
			{
				if (this.allPropertyBags == null)
				{
					this.allPropertyBags = this.nodes.SelectMany((IConversationTreeNode node) => node.StorePropertyBags);
				}
				return this.allPropertyBags;
			}
		}

		public Dictionary<IConversationTreeNode, IConversationTreeNode> BuildPreviousNodeGraph()
		{
			Dictionary<IConversationTreeNode, IConversationTreeNode> previousNodeMap = new Dictionary<IConversationTreeNode, IConversationTreeNode>(ConversationTreeNodeBase.EqualityComparer);
			this.ExecuteSortedAction(ConversationTreeSortOrder.ChronologicalAscending, delegate(ConversationTreeSortOrder param0)
			{
				for (int i = 1; i < this.nodes.Count; i++)
				{
					previousNodeMap.Add(this.nodes[i], this.nodes[i - 1]);
				}
			});
			return previousNodeMap;
		}

		public void ExecuteSortedAction(ConversationTreeSortOrder sortOrder, SortedActionDelegate action)
		{
			ConversationTreeSortOrder treeOriginalSortOrder = this.sortOrder;
			this.Sort(sortOrder);
			action(treeOriginalSortOrder);
			this.Sort(treeOriginalSortOrder);
		}

		public IConversationTreeNode RootMessageNode
		{
			get
			{
				if (this.rootMessageNode == null)
				{
					this.rootMessageNode = this.FirstDeliveredNode;
					while (this.rootMessageNode != null && this.rootMessageNode.ParentNode != null && this.rootMessageNode.ParentNode.HasData)
					{
						this.rootMessageNode = this.rootMessageNode.ParentNode;
					}
				}
				return this.rootMessageNode;
			}
		}

		public StoreObjectId RootMessageId
		{
			get
			{
				if (this.RootMessageNode != null)
				{
					return this.RootMessageNode.MainStoreObjectId;
				}
				return null;
			}
		}

		private IConversationTreeNode FirstDeliveredNode
		{
			get
			{
				if (this.firstDeliveredNode == null)
				{
					if (this.Count == 0)
					{
						return null;
					}
					this.firstDeliveredNode = this.ElementAt(0);
					foreach (IConversationTreeNode y in this)
					{
						if (ConversationTreeNodeBase.ChronologicalComparer.Compare(this.firstDeliveredNode, y) > 0)
						{
							this.firstDeliveredNode = y;
						}
					}
				}
				return this.firstDeliveredNode;
			}
		}

		public bool Remove(IConversationTreeNode item)
		{
			throw new NotSupportedException("The method or operation is not implemented.");
		}

		void ICollection<IConversationTreeNode>.Clear()
		{
			throw new NotSupportedException("The method or operation is not implemented.");
		}

		void ICollection<IConversationTreeNode>.Add(IConversationTreeNode item)
		{
			throw new NotSupportedException("The method or operation is not implemented.");
		}

		bool ICollection<IConversationTreeNode>.Contains(IConversationTreeNode item)
		{
			foreach (IConversationTreeNode y in this.nodes)
			{
				if (ConversationTreeNodeBase.EqualityComparer.Equals(item, y))
				{
					return true;
				}
			}
			return false;
		}

		void ICollection<IConversationTreeNode>.CopyTo(IConversationTreeNode[] array, int arrayIndex)
		{
			this.nodes.CopyTo(array, arrayIndex);
		}

		bool ICollection<IConversationTreeNode>.IsReadOnly
		{
			get
			{
				return true;
			}
		}

		public IConversationTreeNode RootNode
		{
			get
			{
				return this.rootNode;
			}
		}

		private Dictionary<StoreObjectId, IConversationTreeNode> StoreIdToNode
		{
			get
			{
				if (this.storeIdToNode == null)
				{
					this.storeIdToNode = this.BuildMapPropertyBagsToNode();
				}
				return this.storeIdToNode;
			}
		}

		private Dictionary<StoreObjectId, IConversationTreeNode> BuildMapPropertyBagsToNode()
		{
			Dictionary<StoreObjectId, IConversationTreeNode> dictionary = new Dictionary<StoreObjectId, IConversationTreeNode>();
			foreach (IConversationTreeNode conversationTreeNode in this.nodes)
			{
				foreach (StoreObjectId key in conversationTreeNode.ToListStoreObjectId())
				{
					if (!dictionary.ContainsKey(key))
					{
						dictionary.Add(key, conversationTreeNode);
					}
				}
			}
			return dictionary;
		}

		public IEnumerator<IConversationTreeNode> GetEnumerator()
		{
			return this.rootNode.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		private readonly HashSet<PropertyDefinition> loadedProperties;

		private readonly IList<IConversationTreeNode> nodes;

		private readonly IConversationTreeNode rootNode;

		private Dictionary<StoreObjectId, IConversationTreeNode> storeIdToNode;

		private string conversationTopic;

		private ConversationTreeSortOrder sortOrder;

		private IConversationTreeNode rootMessageNode;

		private IConversationTreeNode firstDeliveredNode;

		private IEnumerable<IStorePropertyBag> allPropertyBags;
	}
}
