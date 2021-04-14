using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.InfoWorker.Common.MessageTracking
{
	internal abstract class TreeManager
	{
		public TreeManager()
		{
		}

		public virtual ICollection<Node> GetPathToLeaf(string leafId)
		{
			if (this.recipientsDroppedDueToDuplication != null && this.recipientsDroppedDueToDuplication.Contains(leafId))
			{
				TraceWrapper.SearchLibraryTracer.TraceError<string>(this.GetHashCode(), "Not returning path for key {0} because it is on the list of duplicate recipients.", leafId);
				return TreeManager.EmptyNodeCollection;
			}
			Node node = this.GetBestResultLeaf(leafId);
			if (node == null)
			{
				return TreeManager.EmptyNodeCollection;
			}
			LinkedList<Node> linkedList = new LinkedList<Node>();
			while (node != this.root)
			{
				linkedList.AddFirst(node);
				node = node.Parent;
			}
			return linkedList;
		}

		public ICollection<Node> GetLeafNodes()
		{
			Dictionary<string, LinkedList<Node>>.KeyCollection keys = this.leafRecordTable.Keys;
			if (keys == null || keys.Count == 0)
			{
				return TreeManager.EmptyNodeCollection;
			}
			LinkedList<Node> linkedList = new LinkedList<Node>();
			foreach (string text in keys)
			{
				if (!text.Equals(string.Empty))
				{
					if (this.recipientsDroppedDueToDuplication != null && this.recipientsDroppedDueToDuplication.Contains(text))
					{
						TraceWrapper.SearchLibraryTracer.TraceError<string>(this.GetHashCode(), "Dropping record for key {0} because it is on the list of duplicate recipients.", text);
					}
					else
					{
						linkedList.AddLast(this.GetBestResultLeaf(text));
					}
				}
			}
			return linkedList;
		}

		protected bool Insert(Node node, TreeManager.DoPreInsertoinProcessingDelegate doPreInsertionProcessing, TreeManager.DoPostInsertionProcessingDelegate doPostInsertionProcessing)
		{
			bool flag;
			Node node2 = this.FindParent(node.ParentKey, node, out flag);
			if (node2 == null)
			{
				TraceWrapper.SearchLibraryTracer.TraceDebug<string>(this.GetHashCode(), "Cannot insert node because no suitable parent with key {0} is found.", node.ParentKey);
				return false;
			}
			if (doPreInsertionProcessing != null && !doPreInsertionProcessing(node2, node))
			{
				TraceWrapper.SearchLibraryTracer.TraceDebug<string, string>(this.GetHashCode(), "node {0} not inserted based on shouldChildrenBeInsertedUnderParent check for parent node {1}", node.Key, node2.Key);
				return false;
			}
			bool flag2 = this.InsertChildUnderParent(node2, node, doPreInsertionProcessing, doPostInsertionProcessing);
			if (flag2 && !flag)
			{
				this.RemoveNodeFromLeafRecords(node2);
			}
			return true;
		}

		protected bool InsertAllChildrenForOneNode(string parentKey, IList<Node> nodes, TreeManager.DoPreInsertoinProcessingDelegate doPreInsertionProcessing, TreeManager.DoPostInsertionProcessingDelegate doPostInsertionProcessing)
		{
			if (nodes == null || nodes.Count == 0)
			{
				return this.RemoveKeyFromLeafSet(parentKey);
			}
			bool flag;
			Node node = this.FindParent(parentKey, nodes[0], out flag);
			if (node == null)
			{
				TraceWrapper.SearchLibraryTracer.TraceDebug<string>(this.GetHashCode(), "Cannot insert any children because no suitable parent with key {0} is found.", parentKey);
				return false;
			}
			bool flag2 = false;
			foreach (Node childNode in nodes)
			{
				if (this.InsertChildUnderParent(node, childNode, doPreInsertionProcessing, doPostInsertionProcessing))
				{
					flag2 = true;
				}
			}
			if (flag2 && !flag)
			{
				this.RemoveNodeFromLeafRecords(node);
			}
			return true;
		}

		protected abstract Node DisambiguateParentRecord(LinkedList<Node> possibleParents, Node node);

		protected abstract bool IsNodeRootChildCandidate(Node node);

		protected abstract int GetNodePriorities(Node node, out int secondaryPriority);

		protected void RemoveNodeFromLeafRecords(Node node)
		{
			LinkedList<Node> leavesByKey = this.GetLeavesByKey(node.Key);
			if (leavesByKey != null)
			{
				leavesByKey.Remove(node);
				if (leavesByKey.Count == 0)
				{
					this.leafRecordTable.Remove(node.Key);
				}
			}
		}

		private Node GetBestResultLeaf(string leafId)
		{
			LinkedList<Node> leavesByKey = this.GetLeavesByKey(leafId);
			if (leavesByKey == null || leavesByKey.Count == 0)
			{
				return null;
			}
			if (leavesByKey.Count == 1)
			{
				return leavesByKey.First.Value;
			}
			Node result = null;
			int num = int.MaxValue;
			int num2 = int.MaxValue;
			foreach (Node node in leavesByKey)
			{
				int num3;
				int nodePriorities = this.GetNodePriorities(node, out num3);
				if (nodePriorities < num || (nodePriorities == num && num3 <= num2))
				{
					result = node;
					num = nodePriorities;
					num2 = num3;
				}
			}
			return result;
		}

		private Node GetLeaf(string leafId)
		{
			LinkedList<Node> leavesByKey = this.GetLeavesByKey(leafId);
			if (leavesByKey == null || leavesByKey.Count < 1)
			{
				return null;
			}
			return leavesByKey.Last.Value;
		}

		private Node FindParent(string parentKey, Node node, out bool parentIsRoot)
		{
			if (this.root == null)
			{
				this.root = new Node(string.Empty, string.Empty, node.Value);
			}
			LinkedList<Node> leavesByKey = this.GetLeavesByKey(parentKey);
			Node bestParent = this.GetBestParent(leavesByKey, node, out parentIsRoot);
			if (bestParent == null)
			{
				TraceWrapper.SearchLibraryTracer.TraceDebug<string>(this.GetHashCode(), "No suitable parent with key {0} is found.", parentKey);
			}
			return bestParent;
		}

		private Node GetBestParent(LinkedList<Node> possibleParents, Node node, out bool bestParentIsRoot)
		{
			bestParentIsRoot = false;
			Node node2 = this.DisambiguateParentRecord(possibleParents, node);
			if (node2 == null)
			{
				if (!this.IsNodeRootChildCandidate(node))
				{
					return null;
				}
				TraceWrapper.SearchLibraryTracer.TraceDebug<string>(this.GetHashCode(), "Inserting node with key '{0}' as a child of the root.", node.Key);
				bestParentIsRoot = true;
				node2 = this.Root;
			}
			return node2;
		}

		private bool InsertChildUnderParent(Node parentNode, Node childNode, TreeManager.DoPreInsertoinProcessingDelegate doPreInsertionProcessing, TreeManager.DoPostInsertionProcessingDelegate doPostInsertionProcessing)
		{
			if (doPreInsertionProcessing != null && !doPreInsertionProcessing(parentNode, childNode))
			{
				TraceWrapper.SearchLibraryTracer.TraceDebug<string, string>(this.GetHashCode(), "node list with first node {0} not inserted based on pre insertion check for parent node {1}", childNode.Key, parentNode.Key);
				return false;
			}
			parentNode.AddChild(childNode);
			Node node = doPostInsertionProcessing(parentNode, childNode);
			this.InsertLeafNode(node);
			return true;
		}

		private void InsertLeafNode(Node node)
		{
			string key = node.Key;
			LinkedList<Node> linkedList = this.GetLeavesByKey(key);
			if (linkedList == null)
			{
				linkedList = new LinkedList<Node>();
			}
			linkedList.AddLast(node);
			this.leafRecordTable[key] = linkedList;
		}

		protected LinkedList<Node> GetLeavesByKey(string key)
		{
			LinkedList<Node> result = null;
			this.leafRecordTable.TryGetValue(key, out result);
			return result;
		}

		internal bool RemoveKeyFromLeafSet(string key)
		{
			return this.leafRecordTable.Remove(key);
		}

		internal void DropRecipientDueToPotentialDuplication(string key)
		{
			if (this.recipientsDroppedDueToDuplication == null)
			{
				this.recipientsDroppedDueToDuplication = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
			}
			if (!this.recipientsDroppedDueToDuplication.Contains(key))
			{
				this.recipientsDroppedDueToDuplication.Add(key);
			}
		}

		public Node Root
		{
			get
			{
				return this.root;
			}
		}

		private Node root;

		private static readonly ICollection<Node> EmptyNodeCollection = new Node[0];

		private Dictionary<string, LinkedList<Node>> leafRecordTable = new Dictionary<string, LinkedList<Node>>(StringComparer.OrdinalIgnoreCase);

		private HashSet<string> recipientsDroppedDueToDuplication;

		public delegate bool DoPreInsertoinProcessingDelegate(Node parent, Node child);

		public delegate Node DoPostInsertionProcessingDelegate(Node parent, Node child);
	}
}
