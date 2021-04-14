using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.Clients.Owa.Core
{
	internal class NavigationNodeList<NodeType> : IList<NodeType>, ICollection<NodeType>, IEnumerable<NodeType>, IEnumerable where NodeType : NavigationNode
	{
		public NavigationNodeList()
		{
			this.data = new List<NodeType>();
		}

		public void Insert(int index, NodeType node)
		{
			if (index < 0 || index > this.Count)
			{
				throw new ArgumentOutOfRangeException(string.Format("Invalid position to insert: {0}.", index));
			}
			this.OnBeforeNodeAdd(node);
			this.data.Insert(index, node);
			this.OnAfterNodeAdd(index);
		}

		public void Add(NodeType node)
		{
			this.OnBeforeNodeAdd(node);
			this.data.Add(node);
			this.OnAfterNodeAdd(this.Count - 1);
		}

		public bool Remove(NodeType node)
		{
			return this.data.Remove(node);
		}

		public void RemoveAt(int index)
		{
			this.data.RemoveAt(index);
		}

		public int IndexOf(NodeType item)
		{
			return this.data.IndexOf(item);
		}

		public NodeType this[int index]
		{
			get
			{
				return this.data[index];
			}
			set
			{
				if (index < 0 || index >= this.Count)
				{
					throw new ArgumentOutOfRangeException(string.Format("Invalid position to modify: {0}.", index));
				}
				this.OnBeforeNodeAdd(value);
				this.data[index] = value;
				this.OnAfterNodeAdd(index);
			}
		}

		public void Clear()
		{
			this.data.Clear();
		}

		public bool Contains(NodeType item)
		{
			return this.data.Contains(item);
		}

		public void CopyTo(NodeType[] array, int arrayIndex)
		{
			this.data.CopyTo(array, arrayIndex);
		}

		public int Count
		{
			get
			{
				return this.data.Count;
			}
		}

		public bool IsReadOnly
		{
			get
			{
				return false;
			}
		}

		public IEnumerator<NodeType> GetEnumerator()
		{
			return this.data.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		public int FindChildByNodeId(StoreObjectId nodeId)
		{
			for (int i = 0; i < this.Count; i++)
			{
				NodeType nodeType = this[i];
				if (nodeType.NavigationNodeId != null)
				{
					NodeType nodeType2 = this[i];
					if (nodeType2.NavigationNodeId.ObjectId.Equals(nodeId))
					{
						return i;
					}
				}
			}
			return -1;
		}

		public NavigationNode RemoveChildByNodeId(StoreObjectId nodeId)
		{
			for (int i = 0; i < this.Count; i++)
			{
				NodeType nodeType = this[i];
				if (nodeType.NavigationNodeId != null)
				{
					NodeType nodeType2 = this[i];
					if (nodeType2.NavigationNodeId.ObjectId.Equals(nodeId))
					{
						NavigationNode result = this[i];
						this.RemoveAt(i);
						return result;
					}
				}
			}
			return null;
		}

		internal void CopyToList(NavigationNodeList<NodeType> nodeList)
		{
			foreach (NodeType nodeType in this.data)
			{
				nodeList.data.Add((NodeType)((object)((ICloneable)((object)nodeType)).Clone()));
			}
		}

		private void CheckSequence(int index, NodeType node)
		{
			byte[] array = null;
			byte[] array2 = null;
			if (index > 0)
			{
				NodeType nodeType = this[index - 1];
				array2 = nodeType.NavigationNodeOrdinal;
			}
			if (index < this.Count - 1)
			{
				NodeType nodeType2 = this[index + 1];
				array = nodeType2.NavigationNodeOrdinal;
			}
			byte[] navigationNodeOrdinal = node.NavigationNodeOrdinal;
			if (navigationNodeOrdinal == null || (array != null && Utilities.CompareByteArrays(navigationNodeOrdinal, array) >= 0) || (array2 != null && Utilities.CompareByteArrays(navigationNodeOrdinal, array2) <= 0))
			{
				if (Utilities.CompareByteArrays(array2, array) == 0)
				{
					int i;
					for (i = index - 1; i >= 0; i--)
					{
						NodeType nodeType3 = this[i];
						if (Utilities.CompareByteArrays(nodeType3.NavigationNodeOrdinal, array2) != 0)
						{
							break;
						}
					}
					int j;
					for (j = index + 1; j < this.Count; j++)
					{
						byte[] array3 = array;
						NodeType nodeType4 = this[j];
						if (Utilities.CompareByteArrays(array3, nodeType4.NavigationNodeOrdinal) != 0)
						{
							break;
						}
					}
					byte[] array4;
					if (i >= 0)
					{
						NodeType nodeType5 = this[i];
						array4 = nodeType5.NavigationNodeOrdinal;
					}
					else
					{
						array4 = null;
					}
					array2 = array4;
					byte[] array5;
					if (j < this.Count)
					{
						NodeType nodeType6 = this[j];
						array5 = nodeType6.NavigationNodeOrdinal;
					}
					else
					{
						array5 = null;
					}
					array = array5;
					for (int k = i + 1; k < j; k++)
					{
						NodeType nodeType7 = this[k];
						nodeType7.NavigationNodeOrdinal = BinaryOrdinalGenerator.GetInbetweenOrdinalValue(array2, array);
						NodeType nodeType8 = this[k];
						array2 = nodeType8.NavigationNodeOrdinal;
					}
					return;
				}
				node.NavigationNodeOrdinal = BinaryOrdinalGenerator.GetInbetweenOrdinalValue(array2, array);
			}
		}

		private void ThrowIfDuplicateExists(NodeType node)
		{
			foreach (NodeType nodeType in this.data)
			{
				if (node.Equals(nodeType))
				{
					throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, "This is a duplicate node for folder: {0}", new object[]
					{
						node.Subject
					}));
				}
			}
		}

		protected virtual void OnBeforeNodeAdd(NodeType item)
		{
			this.ThrowIfDuplicateExists(item);
		}

		protected virtual void OnAfterNodeAdd(int index)
		{
			this.CheckSequence(index, this[index]);
		}

		private readonly List<NodeType> data;
	}
}
