using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Data.Storage.Conversations
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal abstract class ConversationTreeNodeBase : IConversationTreeNode, IEnumerable<IConversationTreeNode>, IEnumerable, IComparable<IConversationTreeNode>
	{
		internal ConversationTreeNodeBase(IConversationTreeNodeSorter conversationTreeNodeSorter)
		{
			this.childNodes = new List<IConversationTreeNode>();
			this.readonlyChildNodes = new ReadOnlyCollection<IConversationTreeNode>(this.childNodes);
			this.sortOrder = ConversationTreeSortOrder.DeepTraversalAscending;
			this.childNodeSorter = conversationTreeNodeSorter;
		}

		public IList<IConversationTreeNode> ChildNodes
		{
			get
			{
				return this.readonlyChildNodes;
			}
		}

		public abstract ConversationId ConversationThreadId { get; }

		public IConversationTreeNode ParentNode { get; set; }

		public abstract List<IStorePropertyBag> StorePropertyBags { get; }

		public abstract bool UpdatePropertyBag(StoreObjectId itemId, IStorePropertyBag bag);

		public abstract bool HasAttachments { get; }

		public abstract bool HasBeenSubmitted { get; }

		public abstract bool IsSpecificMessageReplyStamped { get; }

		public abstract bool IsSpecificMessageReply { get; }

		public abstract ConversationId ConversationId { get; }

		public abstract bool HasData { get; }

		public abstract byte[] Index { get; }

		public abstract StoreObjectId MainStoreObjectId { get; }

		public abstract ExDateTime? ReceivedTime { get; }

		public abstract string ItemClass { get; }

		public abstract List<StoreObjectId> ToListStoreObjectId();

		private List<IConversationTreeNode> FlatSortNodes
		{
			get
			{
				if (this.flatSortNodes == null)
				{
					this.flatSortNodes = this.CalculateFlatSortNodes(this.SortOrder);
				}
				return this.flatSortNodes;
			}
			set
			{
				this.flatSortNodes = value;
			}
		}

		public static void SortByDate(List<IConversationTreeNode> nodes)
		{
			ConversationTreeNodeBase.SortByDate(nodes, true);
		}

		public static List<IConversationTreeNode> TrimToNewest(List<IConversationTreeNode> relevantNodes, int maxItemsToReturn)
		{
			List<IConversationTreeNode> list = new List<IConversationTreeNode>(relevantNodes);
			ConversationTreeNodeBase.SortByDate(list);
			if (list.Count > maxItemsToReturn)
			{
				list.RemoveRange(0, list.Count - maxItemsToReturn);
			}
			return list;
		}

		public static IComparer<IConversationTreeNode> ChronologicalComparer
		{
			get
			{
				return ConversationTreeNodeChronologicalComparer.Default;
			}
		}

		public static IEqualityComparer<IConversationTreeNode> EqualityComparer
		{
			get
			{
				return ConversationTreeNodeEqualityComparer.Default;
			}
		}

		private static void SortByDate(List<IConversationTreeNode> nodes, bool isAscending)
		{
			nodes.Sort((IConversationTreeNode left, IConversationTreeNode right) => ConversationTreeNodeBase.ChronologicalComparer.Compare(left, right));
			if (!isAscending)
			{
				nodes.Reverse();
			}
		}

		private static bool IsChronologicalSortOrder(ConversationTreeSortOrder sortOrder)
		{
			return sortOrder == ConversationTreeSortOrder.ChronologicalAscending || sortOrder == ConversationTreeSortOrder.ChronologicalDescending || sortOrder == ConversationTreeSortOrder.TraversalChronologicalAscending || sortOrder == ConversationTreeSortOrder.TraversalChronologicalDescending;
		}

		private static bool IsTraversalChronologicalSortOrder(ConversationTreeSortOrder sortOrder)
		{
			return sortOrder == ConversationTreeSortOrder.TraversalChronologicalAscending || sortOrder == ConversationTreeSortOrder.TraversalChronologicalDescending;
		}

		internal static int CompareNodesTraversal(IConversationTreeNode left, IConversationTreeNode right)
		{
			IList<byte> index = left.Index;
			IList<byte> index2 = right.Index;
			if (index == null && index2 == null)
			{
				return 0;
			}
			if (index == null)
			{
				return -1;
			}
			if (index2 == null)
			{
				return 1;
			}
			int num = Math.Min(index.Count, index2.Count);
			for (int i = 0; i < num; i++)
			{
				int num2 = (int)(index[i] - index2[i]);
				if (num2 != 0)
				{
					return num2;
				}
			}
			if (index.Count == index2.Count)
			{
				return right.ChildNodes.Count - left.ChildNodes.Count;
			}
			return index.Count - index2.Count;
		}

		public virtual ConversationTreeNodeRelation GetRelationTo(IConversationTreeNode otherNode)
		{
			if (!this.HasData && otherNode.HasData)
			{
				return ConversationTreeNodeRelation.Parent;
			}
			if (this.HasData && !otherNode.HasData)
			{
				return ConversationTreeNodeRelation.Child;
			}
			IList<byte> index = this.Index;
			IList<byte> index2 = otherNode.Index;
			if (index == null || index2 == null || index.Count == index2.Count)
			{
				return ConversationTreeNodeRelation.Unrelated;
			}
			int num = Math.Min(index.Count, index2.Count);
			for (int i = 0; i < num; i++)
			{
				if (index[i] != index2[i])
				{
					return ConversationTreeNodeRelation.Unrelated;
				}
			}
			if (index.Count != num)
			{
				return ConversationTreeNodeRelation.Child;
			}
			return ConversationTreeNodeRelation.Parent;
		}

		public void AddChild(IConversationTreeNode node)
		{
			if (!this.TryAddChild(node))
			{
				throw new ArgumentException("node");
			}
		}

		public bool TryAddChild(IConversationTreeNode node)
		{
			if (node.ParentNode != null)
			{
				throw new ArgumentException("Can't already node again", "node");
			}
			if (this.GetRelationTo(node) != ConversationTreeNodeRelation.Parent)
			{
				return false;
			}
			for (int i = this.childNodes.Count - 1; i >= 0; i--)
			{
				ConversationTreeNodeRelation relationTo = node.GetRelationTo(this.ChildNodes[i]);
				if (relationTo != ConversationTreeNodeRelation.Unrelated)
				{
					if (relationTo != ConversationTreeNodeRelation.Parent)
					{
						return this.childNodes[i].TryAddChild(node);
					}
					this.childNodes[i].ParentNode = null;
					node.AddChild(this.childNodes[i]);
					this.childNodes.RemoveAt(i);
				}
			}
			this.childNodes.Add(node);
			node.ParentNode = this;
			return true;
		}

		public void SortChildNodes(ConversationTreeSortOrder sortOrder)
		{
			if (this.SortOrder == sortOrder)
			{
				return;
			}
			if (ConversationTreeNodeBase.IsChronologicalSortOrder(sortOrder))
			{
				this.FlatSortNodes = this.CalculateFlatSortNodes(sortOrder);
			}
			else
			{
				this.TraversalSortChildNodes(sortOrder);
			}
			this.SortOrder = sortOrder;
		}

		public bool IsPartOf(StoreObjectId itemId)
		{
			return this.ToListStoreObjectId().Contains(itemId);
		}

		public abstract T GetValueOrDefault<T>(PropertyDefinition propertyDefinition, T defaultValue = default(T));

		public bool HasChildren
		{
			get
			{
				return this.childNodes.Count > 0;
			}
		}

		public abstract T GetValueOrDefault<T>(StoreObjectId itemId, PropertyDefinition propertyDefinition, T defaultValue = default(T));

		public abstract bool TryGetPropertyBag(StoreObjectId itemId, out IStorePropertyBag bag);

		public abstract IStorePropertyBag MainPropertyBag { get; }

		public ConversationTreeSortOrder SortOrder
		{
			get
			{
				return this.sortOrder;
			}
			set
			{
				this.sortOrder = value;
			}
		}

		private List<IConversationTreeNode> CalculateFlatSortNodes(ConversationTreeSortOrder sortOrder)
		{
			if (ConversationTreeNodeBase.IsTraversalChronologicalSortOrder(sortOrder))
			{
				return this.childNodeSorter.Sort(this, sortOrder);
			}
			return this.FlatSortChildNodes(sortOrder);
		}

		public void ApplyActionToChild(Action<IConversationTreeNode> action)
		{
			action(this);
			foreach (IConversationTreeNode conversationTreeNode in this.childNodes)
			{
				conversationTreeNode.ApplyActionToChild(action);
			}
		}

		private List<IConversationTreeNode> FlatSortChildNodes(ConversationTreeSortOrder sortOrder)
		{
			List<IConversationTreeNode> flatSortNodes = new List<IConversationTreeNode>(0);
			this.ApplyActionToChild(delegate(IConversationTreeNode treeNode)
			{
				treeNode.SortOrder = sortOrder;
				if (treeNode.HasData)
				{
					flatSortNodes.Add(treeNode);
				}
			});
			ConversationTreeNodeBase.SortByDate(flatSortNodes, sortOrder == ConversationTreeSortOrder.ChronologicalAscending);
			return flatSortNodes;
		}

		private void TraversalSortChildNodes(ConversationTreeSortOrder sortOrder)
		{
			for (int i = 0; i < this.childNodes.Count; i++)
			{
				this.childNodes[i].SortChildNodes(sortOrder);
			}
			this.childNodes.Sort(delegate(IConversationTreeNode left, IConversationTreeNode right)
			{
				int num = ConversationTreeNodeBase.CompareNodesTraversal(left, right);
				return (sortOrder == ConversationTreeSortOrder.DeepTraversalAscending) ? num : (-1 * num);
			});
		}

		public int CompareTo(IConversationTreeNode otherNode)
		{
			return ConversationTreeNodeChronologicalComparer.Default.Compare(this, otherNode);
		}

		public IEnumerator<IConversationTreeNode> GetEnumerator()
		{
			if (ConversationTreeNodeBase.IsChronologicalSortOrder(this.SortOrder))
			{
				foreach (IConversationTreeNode node in this.FlatSortNodes)
				{
					yield return node;
				}
			}
			else
			{
				foreach (IConversationTreeNode firstLevelIteration in this.childNodes)
				{
					if (this.SortOrder != ConversationTreeSortOrder.DeepTraversalDescending)
					{
						yield return firstLevelIteration;
					}
					foreach (IConversationTreeNode secondLevelIteration in firstLevelIteration)
					{
						yield return secondLevelIteration;
					}
					if (this.SortOrder == ConversationTreeSortOrder.DeepTraversalDescending)
					{
						yield return firstLevelIteration;
					}
				}
			}
			yield break;
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		private readonly List<IConversationTreeNode> childNodes;

		private readonly ReadOnlyCollection<IConversationTreeNode> readonlyChildNodes;

		private readonly IConversationTreeNodeSorter childNodeSorter;

		private ConversationTreeSortOrder sortOrder;

		private List<IConversationTreeNode> flatSortNodes;
	}
}
