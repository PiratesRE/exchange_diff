using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace Microsoft.Exchange.Collections
{
	public class Btree<TKey, TValue> : IEnumerable<TValue>, IEnumerable where TKey : IComparable<TKey> where TValue : ISortKey<TKey>
	{
		public Btree(int nodeDensity)
		{
			this.nodeDensity = ((nodeDensity > 5) ? nodeDensity : 5);
			this.root = new Btree<TKey, TValue>.Node
			{
				Key = default(TKey)
			};
			this.count = 0;
			this.version = 0;
		}

		public bool TryGetValue(TKey key, out TValue value)
		{
			if (this.root.Type == Btree<TKey, TValue>.NodeType.Empty)
			{
				value = default(TValue);
				return false;
			}
			TValue[] array = null;
			int num = 0;
			if (this.root.Type == Btree<TKey, TValue>.NodeType.InnerNode)
			{
				Btree<TKey, TValue>.Node[] children = this.root.Children;
				int num2 = this.root.Count;
				while (array == null)
				{
					int num3;
					Btree<TKey, TValue>.BinarySearch<Btree<TKey, TValue>.Node>(key, children, num2, out num3);
					Btree<TKey, TValue>.Node node = children[(num3 < 0) ? 0 : num3];
					if (node.Type == Btree<TKey, TValue>.NodeType.LeafNode)
					{
						array = node.LeafValues;
						num = node.Count;
					}
					else
					{
						children = node.Children;
						num2 = node.Count;
					}
				}
			}
			else
			{
				array = this.root.LeafValues;
				num = this.root.Count;
			}
			int num4;
			if (!Btree<TKey, TValue>.BinarySearch<TValue>(key, array, num, out num4))
			{
				value = default(TValue);
				return false;
			}
			value = array[num4];
			return true;
		}

		public bool TryGetFirst(out TValue value)
		{
			if (this.root.Type == Btree<TKey, TValue>.NodeType.Empty)
			{
				value = default(TValue);
				return false;
			}
			Btree<TKey, TValue>.Node node = this.root;
			while (node.Type == Btree<TKey, TValue>.NodeType.InnerNode)
			{
				node = node.Children[0];
			}
			value = node.LeafValues[0];
			return true;
		}

		public bool TryGetLast(out TValue value)
		{
			if (this.root.Type == Btree<TKey, TValue>.NodeType.Empty)
			{
				value = default(TValue);
				return false;
			}
			Btree<TKey, TValue>.Node node = this.root;
			while (node.Type == Btree<TKey, TValue>.NodeType.InnerNode)
			{
				node = node.Children[node.Count - 1];
			}
			value = node.LeafValues[node.Count - 1];
			return true;
		}

		public void Add(TValue value)
		{
			if (this.root.Type == Btree<TKey, TValue>.NodeType.Empty)
			{
				this.root.Type = Btree<TKey, TValue>.NodeType.LeafNode;
				this.root.LeafValues = new TValue[this.nodeDensity];
				this.root.LeafValues[0] = value;
				this.root.Count = 1;
				this.root.Key = value.SortKey;
				this.count = 1;
				this.version++;
				return;
			}
			if (this.root.Type == Btree<TKey, TValue>.NodeType.LeafNode)
			{
				Btree<TKey, TValue>.InsertToLeaf(ref this.root, value);
			}
			else
			{
				Btree<TKey, TValue>.InsertToInner(ref this.root, value);
			}
			if (this.root.IsFull)
			{
				Btree<TKey, TValue>.Node[] array = new Btree<TKey, TValue>.Node[this.nodeDensity];
				if (this.root.Type == Btree<TKey, TValue>.NodeType.LeafNode)
				{
					array[0] = this.root;
					Btree<TKey, TValue>.Node node = Btree<TKey, TValue>.SplitLeaf(ref array[0]);
					array[1] = node;
				}
				else
				{
					array[0] = this.root;
					Btree<TKey, TValue>.Node node2 = Btree<TKey, TValue>.SplitInner(ref array[0]);
					array[1] = node2;
				}
				this.root = new Btree<TKey, TValue>.Node
				{
					Type = Btree<TKey, TValue>.NodeType.InnerNode,
					Count = 2,
					Children = array,
					Key = array[0].Key
				};
			}
			this.count++;
			this.version++;
		}

		public bool Remove(TKey key, out TValue value)
		{
			if (this.root.Type == Btree<TKey, TValue>.NodeType.Empty)
			{
				value = default(TValue);
				return false;
			}
			bool flag;
			if (this.root.Type == Btree<TKey, TValue>.NodeType.LeafNode)
			{
				flag = Btree<TKey, TValue>.RemoveFromLeaf(ref this.root, key, out value);
				if (flag)
				{
					if (this.root.Count == 0)
					{
						this.root = default(Btree<TKey, TValue>.Node);
						this.root.Key = default(TKey);
					}
					this.count--;
					this.version++;
				}
			}
			else
			{
				flag = Btree<TKey, TValue>.RemoveFromInner(ref this.root, key, out value);
				if (flag)
				{
					if (this.root.Count == 1)
					{
						this.root = this.root.Children[0];
					}
					this.count--;
					this.version++;
				}
			}
			return flag;
		}

		public void Clear()
		{
			this.root = new Btree<TKey, TValue>.Node
			{
				Key = default(TKey)
			};
			this.count = 0;
			this.version++;
		}

		public int Count
		{
			get
			{
				return this.count;
			}
		}

		public Btree<TKey, TValue>.Enumerator GetEnumerator()
		{
			return new Btree<TKey, TValue>.Enumerator(this);
		}

		IEnumerator<TValue> IEnumerable<!1>.GetEnumerator()
		{
			return new Btree<TKey, TValue>.Enumerator(this);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return new Btree<TKey, TValue>.Enumerator(this);
		}

		private static bool RemoveFromLeaf(ref Btree<TKey, TValue>.Node leaf, TKey key, out TValue value)
		{
			int num;
			if (!Btree<TKey, TValue>.BinarySearch<TValue>(key, leaf.LeafValues, leaf.Count, out num))
			{
				value = default(TValue);
				return false;
			}
			value = leaf.LeafValues[num];
			Btree<TKey, TValue>.RemoveAt<TValue>(leaf.LeafValues, ref leaf.Count, num);
			if (num == 0 && leaf.Count > 0)
			{
				leaf.Key = leaf.LeafValues[0].SortKey;
			}
			return true;
		}

		private static bool RemoveFromInner(ref Btree<TKey, TValue>.Node innerNode, TKey key, out TValue value)
		{
			Btree<TKey, TValue>.Node[] children = innerNode.Children;
			int num;
			Btree<TKey, TValue>.BinarySearch<Btree<TKey, TValue>.Node>(key, children, innerNode.Count, out num);
			if (num < 0)
			{
				num = 0;
			}
			bool flag;
			if (children[num].Type == Btree<TKey, TValue>.NodeType.LeafNode)
			{
				flag = Btree<TKey, TValue>.RemoveFromLeaf(ref children[num], key, out value);
			}
			else
			{
				flag = Btree<TKey, TValue>.RemoveFromInner(ref children[num], key, out value);
			}
			if (!flag)
			{
				return false;
			}
			if (num == 0)
			{
				innerNode.Key = children[0].Key;
			}
			if (children[num].NeedRebalance && innerNode.Count > 1)
			{
				int num2;
				if (num > 0 && num < innerNode.Count - 1)
				{
					if (children[num - 1].Count + children[num].Count < children.Length)
					{
						num2 = num - 1;
					}
					else if (children[num].Count + children[num + 1].Count < children.Length)
					{
						num2 = num;
					}
					else if (children[num - 1].Count < children[num + 1].Count)
					{
						num2 = num;
					}
					else
					{
						num2 = num - 1;
					}
				}
				else if (num == 0)
				{
					num2 = 0;
				}
				else
				{
					num2 = num - 1;
				}
				int num3 = num2 + 1;
				Btree<TKey, TValue>.Rebalance(ref children[num2], ref children[num3]);
				if (children[num3].Count == 0)
				{
					Btree<TKey, TValue>.RemoveAt<Btree<TKey, TValue>.Node>(children, ref innerNode.Count, num3);
				}
			}
			return true;
		}

		private static Btree<TKey, TValue>.Node SplitLeaf(ref Btree<TKey, TValue>.Node leafNode)
		{
			TValue[] leafValues = leafNode.LeafValues;
			TValue[] array = new TValue[leafValues.Length];
			int num = leafNode.Count / 2;
			int num2 = leafValues.Length - num;
			Array.Copy(leafValues, num, array, 0, num2);
			Array.Clear(leafValues, num, num2);
			leafNode.Count -= num2;
			return new Btree<TKey, TValue>.Node
			{
				Type = Btree<TKey, TValue>.NodeType.LeafNode,
				Count = num2,
				LeafValues = array,
				Key = array[0].SortKey
			};
		}

		private static Btree<TKey, TValue>.Node SplitInner(ref Btree<TKey, TValue>.Node innerNode)
		{
			Btree<TKey, TValue>.Node[] children = innerNode.Children;
			Btree<TKey, TValue>.Node[] array = new Btree<TKey, TValue>.Node[children.Length];
			int num = innerNode.Count / 2;
			int num2 = children.Length - num;
			Array.Copy(children, num, array, 0, num2);
			Array.Clear(children, num, num2);
			innerNode.Count -= num2;
			return new Btree<TKey, TValue>.Node
			{
				Type = Btree<TKey, TValue>.NodeType.InnerNode,
				Count = num2,
				Children = array,
				Key = array[0].Key
			};
		}

		private static void Rebalance(ref Btree<TKey, TValue>.Node left, ref Btree<TKey, TValue>.Node right)
		{
			int num = (left.Type == Btree<TKey, TValue>.NodeType.LeafNode) ? left.LeafValues.Length : left.Children.Length;
			if (left.Count + right.Count < num)
			{
				if (left.Type == Btree<TKey, TValue>.NodeType.InnerNode)
				{
					Array.Copy(right.Children, 0, left.Children, left.Count, right.Count);
					right.Children = null;
				}
				else
				{
					Array.Copy(right.LeafValues, 0, left.LeafValues, left.Count, right.Count);
					right.LeafValues = null;
				}
				left.Count += right.Count;
				right.Count = 0;
				return;
			}
			if (left.Count + right.Count > num)
			{
				int num2 = (left.Count + right.Count) / 2;
				if (left.Count < num2)
				{
					int num3 = num2 - left.Count;
					if (left.Type == Btree<TKey, TValue>.NodeType.InnerNode)
					{
						Array.Copy(right.Children, 0, left.Children, left.Count, num3);
						Array.Copy(right.Children, num3, right.Children, 0, right.Count - num3);
						right.Count -= num3;
						Array.Clear(right.Children, right.Count, num3);
						right.Key = right.Children[0].Key;
					}
					else
					{
						Array.Copy(right.LeafValues, 0, left.LeafValues, left.Count, num3);
						Array.Copy(right.LeafValues, num3, right.LeafValues, 0, right.Count - num3);
						right.Count -= num3;
						Array.Clear(right.LeafValues, right.Count, num3);
						right.Key = right.LeafValues[0].SortKey;
					}
					left.Count += num3;
					return;
				}
				if (left.Count > num2)
				{
					int num4 = left.Count - num2;
					if (left.Type == Btree<TKey, TValue>.NodeType.InnerNode)
					{
						Array.Copy(right.Children, 0, right.Children, num4, right.Count);
						Array.Copy(left.Children, num2, right.Children, 0, num4);
						Array.Clear(left.Children, num2, num4);
						right.Key = right.Children[0].Key;
					}
					else
					{
						Array.Copy(right.LeafValues, 0, right.LeafValues, num4, right.Count);
						Array.Copy(left.LeafValues, num2, right.LeafValues, 0, num4);
						Array.Clear(left.LeafValues, num2, num4);
						right.Key = right.LeafValues[0].SortKey;
					}
					left.Count -= num4;
					right.Count += num4;
				}
			}
		}

		private static void InsertAt<T>(T[] items, ref int count, int index, T value)
		{
			if (index < count)
			{
				Array.Copy(items, index, items, index + 1, count - index);
			}
			items[index] = value;
			count++;
		}

		private static void RemoveAt<T>(T[] items, ref int count, int index)
		{
			Array.Copy(items, index + 1, items, index, count - index);
			items[count] = default(T);
			count--;
		}

		private static void InsertToLeaf(ref Btree<TKey, TValue>.Node leaf, TValue value)
		{
			int num;
			if (Btree<TKey, TValue>.BinarySearch<TValue>(value.SortKey, leaf.LeafValues, leaf.Count, out num))
			{
				throw new ArgumentException("An element with the same key already exists in the tree.", "key");
			}
			Btree<TKey, TValue>.InsertAt<TValue>(leaf.LeafValues, ref leaf.Count, num + 1, value);
			if (num < 0)
			{
				leaf.Key = value.SortKey;
			}
		}

		private static void InsertToInner(ref Btree<TKey, TValue>.Node innerNode, TValue value)
		{
			Btree<TKey, TValue>.Node[] children = innerNode.Children;
			TKey sortKey = value.SortKey;
			int num = children.Length;
			int num2;
			Btree<TKey, TValue>.BinarySearch<Btree<TKey, TValue>.Node>(sortKey, children, innerNode.Count, out num2);
			if (num2 < 0)
			{
				num2 = 0;
			}
			if (children[num2].Type == Btree<TKey, TValue>.NodeType.LeafNode)
			{
				Btree<TKey, TValue>.InsertToLeaf(ref children[num2], value);
				if (children[num2].IsFull)
				{
					if (num2 < innerNode.Count - 1 && children[num2 + 1].Count <= num * 3 / 4)
					{
						Btree<TKey, TValue>.Rebalance(ref children[num2], ref children[num2 + 1]);
					}
					else if (num2 > 0 && children[num2 - 1].Count <= num * 3 / 4)
					{
						Btree<TKey, TValue>.Rebalance(ref children[num2 - 1], ref children[num2]);
					}
					else
					{
						Btree<TKey, TValue>.Node value2 = Btree<TKey, TValue>.SplitLeaf(ref children[num2]);
						Btree<TKey, TValue>.InsertAt<Btree<TKey, TValue>.Node>(children, ref innerNode.Count, num2 + 1, value2);
					}
				}
			}
			else
			{
				Btree<TKey, TValue>.InsertToInner(ref children[num2], value);
				if (children[num2].IsFull)
				{
					if (num2 < innerNode.Count - 1 && children[num2 + 1].Count <= num * 3 / 4)
					{
						Btree<TKey, TValue>.Rebalance(ref children[num2], ref children[num2 + 1]);
					}
					else if (num2 > 0 && children[num2 - 1].Count <= num * 3 / 4)
					{
						Btree<TKey, TValue>.Rebalance(ref children[num2 - 1], ref children[num2]);
					}
					else
					{
						Btree<TKey, TValue>.Node value3 = Btree<TKey, TValue>.SplitInner(ref children[num2]);
						Btree<TKey, TValue>.InsertAt<Btree<TKey, TValue>.Node>(children, ref innerNode.Count, num2 + 1, value3);
					}
				}
			}
			if (num2 == 0)
			{
				innerNode.Key = children[0].Key;
			}
		}

		private static bool BinarySearch<TData>(TKey key, TData[] data, int count, out int index) where TData : ISortKey<TKey>
		{
			int i = 0;
			int num = count - 1;
			while (i <= num)
			{
				int num2 = (i + num) / 2;
				int num3 = key.CompareTo(data[num2].SortKey);
				if (num3 < 0)
				{
					num = num2 - 1;
				}
				else
				{
					if (num3 <= 0)
					{
						index = num2;
						return true;
					}
					i = num2 + 1;
				}
			}
			index = num;
			return false;
		}

		[Conditional("DEBUG")]
		private static void AssertConsistent(Btree<TKey, TValue>.Node node)
		{
			if (node.Type == Btree<TKey, TValue>.NodeType.LeafNode)
			{
				return;
			}
			Btree<TKey, TValue>.NodeType type = node.Type;
		}

		private const int MinimumNodeDensity = 5;

		private Btree<TKey, TValue>.Node root;

		private readonly int nodeDensity;

		private int count;

		private int version;

		private enum NodeType
		{
			Empty,
			InnerNode,
			LeafNode
		}

		[DebuggerDisplay("[{Type}, {Count}, First={Key}]")]
		private struct Node : ISortKey<TKey>
		{
			public TKey SortKey
			{
				get
				{
					return this.Key;
				}
			}

			public bool IsFull
			{
				get
				{
					if (this.Type == Btree<TKey, TValue>.NodeType.InnerNode)
					{
						return this.Count == this.Children.Length;
					}
					return this.Type == Btree<TKey, TValue>.NodeType.LeafNode && this.Count == this.LeafValues.Length;
				}
			}

			public bool NeedRebalance
			{
				get
				{
					if (this.Type == Btree<TKey, TValue>.NodeType.InnerNode)
					{
						return this.Count <= this.Children.Length / 2;
					}
					return this.Type == Btree<TKey, TValue>.NodeType.LeafNode && this.Count <= this.LeafValues.Length / 2;
				}
			}

			public Btree<TKey, TValue>.NodeType Type;

			public int Count;

			public TKey Key;

			public TValue[] LeafValues;

			public Btree<TKey, TValue>.Node[] Children;
		}

		public struct Enumerator : IEnumerator<TValue>, IDisposable, IEnumerator
		{
			public Enumerator(Btree<TKey, TValue> btree)
			{
				this.btree = btree;
				this.version = btree.version;
				this.node = default(Btree<TKey, TValue>.Node);
				this.valueIndex = -1;
				this.leafIndex = 0;
				this.current = default(TValue);
			}

			public TValue Current
			{
				get
				{
					return this.current;
				}
			}

			object IEnumerator.Current
			{
				get
				{
					return this.current;
				}
			}

			public bool MoveNext()
			{
				if (this.version != this.btree.version)
				{
					throw new InvalidOperationException("Collection changed.");
				}
				if (this.node.Type == Btree<TKey, TValue>.NodeType.Empty && !Btree<TKey, TValue>.Enumerator.MoveFirst(this.btree, ref this.node))
				{
					return false;
				}
				if (this.node.Type == Btree<TKey, TValue>.NodeType.LeafNode)
				{
					if (this.valueIndex < this.node.Count - 1)
					{
						this.valueIndex++;
						this.current = this.node.LeafValues[this.valueIndex];
						return true;
					}
					return false;
				}
				else
				{
					if (this.node.Type == Btree<TKey, TValue>.NodeType.InnerNode)
					{
						if (this.valueIndex < this.node.Children[this.leafIndex].Count - 1)
						{
							this.valueIndex++;
						}
						else if (this.leafIndex < this.node.Count - 1)
						{
							this.leafIndex++;
							this.valueIndex = 0;
						}
						else
						{
							if (!Btree<TKey, TValue>.Enumerator.FindNext(this.btree.root, this.current.SortKey, ref this.node))
							{
								return false;
							}
							this.leafIndex = 0;
							this.valueIndex = 0;
						}
						this.current = this.node.Children[this.leafIndex].LeafValues[this.valueIndex];
						return true;
					}
					this.current = default(TValue);
					return false;
				}
			}

			public void Reset()
			{
				this.version = this.btree.version;
				this.node = default(Btree<TKey, TValue>.Node);
				this.valueIndex = -1;
				this.leafIndex = 0;
				this.current = default(TValue);
			}

			public void Dispose()
			{
			}

			public Btree<TKey, TValue>.Enumerator Clone()
			{
				return this;
			}

			private static bool MoveFirst(Btree<TKey, TValue> btree, ref Btree<TKey, TValue>.Node node)
			{
				if (btree.root.Type == Btree<TKey, TValue>.NodeType.Empty)
				{
					return false;
				}
				if (btree.root.Type == Btree<TKey, TValue>.NodeType.LeafNode)
				{
					node = btree.root;
					return true;
				}
				node = btree.root;
				while (node.Children[0].Type == Btree<TKey, TValue>.NodeType.InnerNode)
				{
					node = node.Children[0];
				}
				return true;
			}

			private static bool FindNext(Btree<TKey, TValue>.Node root, TKey key, ref Btree<TKey, TValue>.Node node)
			{
				if (root.Type != Btree<TKey, TValue>.NodeType.InnerNode || root.Children[0].Type == Btree<TKey, TValue>.NodeType.LeafNode)
				{
					return false;
				}
				Btree<TKey, TValue>.Node? node2 = null;
				int num = -1;
				Btree<TKey, TValue>.Node value = root;
				while (value.Children[0].Type == Btree<TKey, TValue>.NodeType.InnerNode)
				{
					int num2;
					Btree<TKey, TValue>.BinarySearch<Btree<TKey, TValue>.Node>(key, value.Children, value.Count, out num2);
					if (num2 < value.Count - 1)
					{
						node2 = new Btree<TKey, TValue>.Node?(value);
						num = num2 + 1;
					}
					value = value.Children[num2];
				}
				if (node2 == null)
				{
					return false;
				}
				node = node2.Value.Children[num];
				while (node.Children[0].Type == Btree<TKey, TValue>.NodeType.InnerNode)
				{
					node = node.Children[0];
				}
				return true;
			}

			private Btree<TKey, TValue> btree;

			private int version;

			private Btree<TKey, TValue>.Node node;

			private int valueIndex;

			private int leafIndex;

			private TValue current;
		}
	}
}
