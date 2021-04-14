using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage.Conversations
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class TraversalChronologicalNodeSorter : IConversationTreeNodeSorter
	{
		private void AddNodeAndUnrelatedChildrenRecursively(PriorityQueue<IConversationTreeNode> candidates, IConversationTreeNode targetNode)
		{
			candidates.Enqueue(targetNode);
			foreach (IConversationTreeNode conversationTreeNode in targetNode.ChildNodes)
			{
				if (targetNode.GetRelationTo(conversationTreeNode) == ConversationTreeNodeRelation.Unrelated)
				{
					this.AddNodeAndUnrelatedChildrenRecursively(candidates, conversationTreeNode);
				}
			}
		}

		public List<IConversationTreeNode> Sort(IConversationTreeNode rootNode, ConversationTreeSortOrder sortOrder)
		{
			List<IConversationTreeNode> list = new List<IConversationTreeNode>();
			PriorityQueue<IConversationTreeNode> priorityQueue = new PriorityQueue<IConversationTreeNode>();
			this.AddNodeAndUnrelatedChildrenRecursively(priorityQueue, rootNode);
			while (priorityQueue.Count() > 0)
			{
				IConversationTreeNode conversationTreeNode = priorityQueue.Dequeue();
				if (conversationTreeNode.HasData)
				{
					conversationTreeNode.SortOrder = sortOrder;
					list.Add(conversationTreeNode);
				}
				foreach (IConversationTreeNode conversationTreeNode2 in conversationTreeNode.ChildNodes)
				{
					if (conversationTreeNode.GetRelationTo(conversationTreeNode2) != ConversationTreeNodeRelation.Unrelated)
					{
						this.AddNodeAndUnrelatedChildrenRecursively(priorityQueue, conversationTreeNode2);
					}
				}
			}
			if (sortOrder == ConversationTreeSortOrder.TraversalChronologicalDescending)
			{
				list.Reverse();
			}
			return list;
		}

		public static readonly IConversationTreeNodeSorter Instance = new TraversalChronologicalNodeSorter();
	}
}
