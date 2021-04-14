using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace Microsoft.Exchange.Server.Storage.Common
{
	public sealed class PersistentAvlTree<TKey, TValue> : ICollection<KeyValuePair<TKey, TValue>>, IEnumerable<KeyValuePair<!0, !1>>, IEnumerable where TKey : IComparable<TKey>
	{
		private PersistentAvlTree()
		{
			this.root = null;
		}

		private PersistentAvlTree(PersistentAvlTree<TKey, TValue>.Node<TKey, TValue> root)
		{
			this.root = root;
		}

		public static PersistentAvlTree<TKey, TValue> Empty
		{
			get
			{
				return PersistentAvlTree<TKey, TValue>.empty;
			}
		}

		public int Count
		{
			get
			{
				return PersistentAvlTree<TKey, TValue>.Node<TKey, TValue>.GetCount(this.root);
			}
		}

		int ICollection<KeyValuePair<!0, !1>>.Count
		{
			get
			{
				return this.Count;
			}
		}

		bool ICollection<KeyValuePair<!0, !1>>.IsReadOnly
		{
			get
			{
				return true;
			}
		}

		public TValue this[TKey key]
		{
			get
			{
				PersistentAvlTree<TKey, TValue>.Node<TKey, TValue> node = this.root;
				while (node != null)
				{
					int num = key.CompareTo(node.Key);
					if (num == 0)
					{
						return node.Value;
					}
					if (num < 0)
					{
						node = node.Left;
					}
					else
					{
						node = node.Right;
					}
				}
				throw new KeyNotFoundException();
			}
		}

		public PersistentAvlTree<TKey, TValue> Add(TKey key, TValue value)
		{
			return new PersistentAvlTree<TKey, TValue>(PersistentAvlTree<TKey, TValue>.InternalInsert(this.root, ref key, ref value));
		}

		public bool Contains(TKey key)
		{
			PersistentAvlTree<TKey, TValue>.Node<TKey, TValue> node = this.root;
			while (node != null)
			{
				int num = key.CompareTo(node.Key);
				if (num == 0)
				{
					return true;
				}
				if (num < 0)
				{
					node = node.Left;
				}
				else
				{
					node = node.Right;
				}
			}
			return false;
		}

		public TValue GetValue(TKey key)
		{
			PersistentAvlTree<TKey, TValue>.Node<TKey, TValue> node = this.root;
			while (node != null)
			{
				int num = key.CompareTo(node.Key);
				if (num == 0)
				{
					return node.Value;
				}
				if (num < 0)
				{
					node = node.Left;
				}
				else
				{
					node = node.Right;
				}
			}
			throw new KeyNotFoundException();
		}

		public bool TryGetValue(TKey key, out TValue value)
		{
			PersistentAvlTree<TKey, TValue>.Node<TKey, TValue> node = this.root;
			while (node != null)
			{
				int num = key.CompareTo(node.Key);
				if (num == 0)
				{
					value = node.Value;
					return true;
				}
				if (num < 0)
				{
					node = node.Left;
				}
				else
				{
					node = node.Right;
				}
			}
			value = default(TValue);
			return false;
		}

		public PersistentAvlTree<TKey, TValue> SetValue(TKey key, TValue value)
		{
			return new PersistentAvlTree<TKey, TValue>(PersistentAvlTree<TKey, TValue>.InternalSetValue(this.root, ref key, ref value));
		}

		public PersistentAvlTree<TKey, TValue> Remove(TKey key)
		{
			bool flag;
			PersistentAvlTree<TKey, TValue>.Node<TKey, TValue> node = PersistentAvlTree<TKey, TValue>.InternalRemove(this.root, ref key, out flag);
			if (!flag)
			{
				throw new KeyNotFoundException("The key is not found in the tree.");
			}
			if (node == null)
			{
				return PersistentAvlTree<TKey, TValue>.Empty;
			}
			return new PersistentAvlTree<TKey, TValue>(node);
		}

		public PersistentAvlTree<TKey, TValue> Remove(TKey key, out bool successful)
		{
			PersistentAvlTree<TKey, TValue>.Node<TKey, TValue> node = PersistentAvlTree<TKey, TValue>.InternalRemove(this.root, ref key, out successful);
			if (object.ReferenceEquals(node, this.root))
			{
				return this;
			}
			if (node == null)
			{
				return PersistentAvlTree<TKey, TValue>.Empty;
			}
			return new PersistentAvlTree<TKey, TValue>(node);
		}

		public IEnumerable<TValue> GetValuesLmr()
		{
			return PersistentAvlTree<TKey, TValue>.TraverseInLeftMiddleRightOrder<TValue>(this.root, PersistentAvlTree<TKey, TValue>.ValueConverter);
		}

		public IEnumerable<KeyValuePair<TKey, TValue>> GetMembersLmr()
		{
			return PersistentAvlTree<TKey, TValue>.TraverseInLeftMiddleRightOrder<KeyValuePair<TKey, TValue>>(this.root, PersistentAvlTree<TKey, TValue>.KeyValuePairConverter);
		}

		public IEnumerable<TKey> GetKeysLmr()
		{
			return PersistentAvlTree<TKey, TValue>.TraverseInLeftMiddleRightOrder<TKey>(this.root, PersistentAvlTree<TKey, TValue>.KeyConverter);
		}

		public IEnumerable<TKey> GetKeysMlr()
		{
			return PersistentAvlTree<TKey, TValue>.TraverseInMiddleLeftRightOrder<TKey>(this.root, PersistentAvlTree<TKey, TValue>.KeyConverter);
		}

		public IEnumerable<TKey> GetKeysMrl()
		{
			return PersistentAvlTree<TKey, TValue>.TraverseInMiddleRightLeftOrder<TKey>(this.root, PersistentAvlTree<TKey, TValue>.KeyConverter);
		}

		void ICollection<KeyValuePair<!0, !1>>.Add(KeyValuePair<TKey, TValue> item)
		{
			throw new NotSupportedException("The collection is read-only.");
		}

		void ICollection<KeyValuePair<!0, !1>>.Clear()
		{
			throw new NotSupportedException("The collection is read-only.");
		}

		bool ICollection<KeyValuePair<!0, !1>>.Contains(KeyValuePair<TKey, TValue> item)
		{
			return this.Contains(item.Key);
		}

		void ICollection<KeyValuePair<!0, !1>>.CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
		{
			IEnumerable<KeyValuePair<TKey, TValue>> membersLmr = this.GetMembersLmr();
			int num = arrayIndex;
			foreach (KeyValuePair<TKey, TValue> keyValuePair in membersLmr)
			{
				array[num] = keyValuePair;
				num++;
			}
		}

		bool ICollection<KeyValuePair<!0, !1>>.Remove(KeyValuePair<TKey, TValue> item)
		{
			throw new NotSupportedException("The collection is read-only.");
		}

		IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<!0, !1>>.GetEnumerator()
		{
			return this.GetMembersLmr().GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetMembersLmr().GetEnumerator();
		}

		private static PersistentAvlTree<TKey, TValue>.Node<TKey, TValue> InternalInsert(PersistentAvlTree<TKey, TValue>.Node<TKey, TValue> node, ref TKey key, ref TValue value)
		{
			PersistentAvlTree<TKey, TValue>.Node<TKey, TValue> node2;
			if (node == null)
			{
				node2 = new PersistentAvlTree<TKey, TValue>.Node<TKey, TValue>(null, null, key, value);
			}
			else
			{
				int num = key.CompareTo(node.Key);
				if (num == 0)
				{
					throw new ArgumentException("An element with the same key already exists in the tree.", "key");
				}
				if (num < 0)
				{
					node2 = new PersistentAvlTree<TKey, TValue>.Node<TKey, TValue>(PersistentAvlTree<TKey, TValue>.InternalInsert(node.Left, ref key, ref value), node.Right, node.Key, node.Value);
				}
				else
				{
					node2 = new PersistentAvlTree<TKey, TValue>.Node<TKey, TValue>(node.Left, PersistentAvlTree<TKey, TValue>.InternalInsert(node.Right, ref key, ref value), node.Key, node.Value);
				}
			}
			return PersistentAvlTree<TKey, TValue>.BalanceNode(node2);
		}

		private static PersistentAvlTree<TKey, TValue>.Node<TKey, TValue> InternalSetValue(PersistentAvlTree<TKey, TValue>.Node<TKey, TValue> node, ref TKey key, ref TValue value)
		{
			PersistentAvlTree<TKey, TValue>.Node<TKey, TValue> node2;
			if (node == null)
			{
				node2 = new PersistentAvlTree<TKey, TValue>.Node<TKey, TValue>(null, null, key, value);
			}
			else
			{
				int num = key.CompareTo(node.Key);
				if (num == 0)
				{
					node2 = new PersistentAvlTree<TKey, TValue>.Node<TKey, TValue>(node.Left, node.Right, key, value);
				}
				else if (num < 0)
				{
					node2 = new PersistentAvlTree<TKey, TValue>.Node<TKey, TValue>(PersistentAvlTree<TKey, TValue>.InternalSetValue(node.Left, ref key, ref value), node.Right, node.Key, node.Value);
				}
				else
				{
					node2 = new PersistentAvlTree<TKey, TValue>.Node<TKey, TValue>(node.Left, PersistentAvlTree<TKey, TValue>.InternalSetValue(node.Right, ref key, ref value), node.Key, node.Value);
				}
			}
			return PersistentAvlTree<TKey, TValue>.BalanceNode(node2);
		}

		private static PersistentAvlTree<TKey, TValue>.Node<TKey, TValue> InternalRemove(PersistentAvlTree<TKey, TValue>.Node<TKey, TValue> node, ref TKey key, out bool found)
		{
			if (node == null)
			{
				found = false;
				return null;
			}
			int num = key.CompareTo(node.Key);
			if (num == 0)
			{
				found = true;
				return PersistentAvlTree<TKey, TValue>.RemoveRoot(node);
			}
			PersistentAvlTree<TKey, TValue>.Node<TKey, TValue> node2;
			if (num < 0)
			{
				node2 = PersistentAvlTree<TKey, TValue>.InternalRemove(node.Left, ref key, out found);
				if (!object.ReferenceEquals(node2, node.Left))
				{
					node2 = new PersistentAvlTree<TKey, TValue>.Node<TKey, TValue>(node2, node.Right, node.Key, node.Value);
				}
				else
				{
					node2 = node;
				}
			}
			else
			{
				node2 = PersistentAvlTree<TKey, TValue>.InternalRemove(node.Right, ref key, out found);
				if (!object.ReferenceEquals(node2, node.Right))
				{
					node2 = new PersistentAvlTree<TKey, TValue>.Node<TKey, TValue>(node.Left, node2, node.Key, node.Value);
				}
				else
				{
					node2 = node;
				}
			}
			if (!object.ReferenceEquals(node2, node))
			{
				node2 = PersistentAvlTree<TKey, TValue>.BalanceNode(node2);
			}
			return node2;
		}

		private static PersistentAvlTree<TKey, TValue>.Node<TKey, TValue> BalanceNode(PersistentAvlTree<TKey, TValue>.Node<TKey, TValue> root)
		{
			PersistentAvlTree<TKey, TValue>.Node<TKey, TValue> result = root;
			if (root != null)
			{
				if (root.Balance == 2)
				{
					if (root.Right.Balance == 1)
					{
						result = new PersistentAvlTree<TKey, TValue>.Node<TKey, TValue>(new PersistentAvlTree<TKey, TValue>.Node<TKey, TValue>(root.Left, root.Right.Left, root.Key, root.Value), root.Right.Right, root.Right.Key, root.Right.Value);
					}
					else if (root.Right.Balance == -1)
					{
						result = new PersistentAvlTree<TKey, TValue>.Node<TKey, TValue>(new PersistentAvlTree<TKey, TValue>.Node<TKey, TValue>(root.Left, root.Right.Left.Left, root.Key, root.Value), new PersistentAvlTree<TKey, TValue>.Node<TKey, TValue>(root.Right.Left.Right, root.Right.Right, root.Right.Key, root.Right.Value), root.Right.Left.Key, root.Right.Left.Value);
					}
					else
					{
						result = new PersistentAvlTree<TKey, TValue>.Node<TKey, TValue>(new PersistentAvlTree<TKey, TValue>.Node<TKey, TValue>(root.Left, root.Right.Left, root.Key, root.Value), root.Right.Right, root.Right.Key, root.Right.Value);
					}
				}
				else if (root.Balance == -2)
				{
					if (root.Left.Balance == -1)
					{
						result = new PersistentAvlTree<TKey, TValue>.Node<TKey, TValue>(root.Left.Left, new PersistentAvlTree<TKey, TValue>.Node<TKey, TValue>(root.Left.Right, root.Right, root.Key, root.Value), root.Left.Key, root.Left.Value);
					}
					else if (root.Left.Balance == 1)
					{
						result = new PersistentAvlTree<TKey, TValue>.Node<TKey, TValue>(new PersistentAvlTree<TKey, TValue>.Node<TKey, TValue>(root.Left.Left, root.Left.Right.Left, root.Left.Key, root.Left.Value), new PersistentAvlTree<TKey, TValue>.Node<TKey, TValue>(root.Left.Right.Right, root.Right, root.Key, root.Value), root.Left.Right.Key, root.Left.Right.Value);
					}
					else
					{
						result = new PersistentAvlTree<TKey, TValue>.Node<TKey, TValue>(root.Left.Left, new PersistentAvlTree<TKey, TValue>.Node<TKey, TValue>(root.Left.Right, root.Right, root.Key, root.Value), root.Left.Key, root.Left.Value);
					}
				}
			}
			return result;
		}

		private static PersistentAvlTree<TKey, TValue>.Node<TKey, TValue> RemoveRoot(PersistentAvlTree<TKey, TValue>.Node<TKey, TValue> root)
		{
			if (root.Left == null && root.Right == null)
			{
				return null;
			}
			if (root.Left == null)
			{
				return root.Right;
			}
			if (root.Right == null)
			{
				return root.Left;
			}
			if (root.Balance == 1)
			{
				PersistentAvlTree<TKey, TValue>.Node<TKey, TValue> node;
				PersistentAvlTree<TKey, TValue>.Node<TKey, TValue> right = PersistentAvlTree<TKey, TValue>.RemoveLeftMost(root.Right, out node);
				return PersistentAvlTree<TKey, TValue>.BalanceNode(new PersistentAvlTree<TKey, TValue>.Node<TKey, TValue>(root.Left, right, node.Key, node.Value));
			}
			PersistentAvlTree<TKey, TValue>.Node<TKey, TValue> node2;
			PersistentAvlTree<TKey, TValue>.Node<TKey, TValue> left = PersistentAvlTree<TKey, TValue>.RemoveRightMost(root.Left, out node2);
			return PersistentAvlTree<TKey, TValue>.BalanceNode(new PersistentAvlTree<TKey, TValue>.Node<TKey, TValue>(left, root.Right, node2.Key, node2.Value));
		}

		private static PersistentAvlTree<TKey, TValue>.Node<TKey, TValue> RemoveLeftMost(PersistentAvlTree<TKey, TValue>.Node<TKey, TValue> node, out PersistentAvlTree<TKey, TValue>.Node<TKey, TValue> leftMost)
		{
			if (node.Left == null)
			{
				leftMost = node;
				return node.Right;
			}
			return PersistentAvlTree<TKey, TValue>.BalanceNode(new PersistentAvlTree<TKey, TValue>.Node<TKey, TValue>(PersistentAvlTree<TKey, TValue>.RemoveLeftMost(node.Left, out leftMost), node.Right, node.Key, node.Value));
		}

		private static PersistentAvlTree<TKey, TValue>.Node<TKey, TValue> RemoveRightMost(PersistentAvlTree<TKey, TValue>.Node<TKey, TValue> node, out PersistentAvlTree<TKey, TValue>.Node<TKey, TValue> rightMost)
		{
			if (node.Right == null)
			{
				rightMost = node;
				return node.Left;
			}
			return PersistentAvlTree<TKey, TValue>.BalanceNode(new PersistentAvlTree<TKey, TValue>.Node<TKey, TValue>(node.Left, PersistentAvlTree<TKey, TValue>.RemoveRightMost(node.Right, out rightMost), node.Key, node.Value));
		}

		[Conditional("DEBUG")]
		private static void VerifyBalance(PersistentAvlTree<TKey, TValue>.Node<TKey, TValue> node)
		{
		}

		private static IEnumerable<R> TraverseInLeftMiddleRightOrder<R>(PersistentAvlTree<TKey, TValue>.Node<TKey, TValue> root, Converter<PersistentAvlTree<TKey, TValue>.Node<TKey, TValue>, R> converter)
		{
			if (root != null)
			{
				Stack<PersistentAvlTree<TKey, TValue>.Node<TKey, TValue>> stack = new Stack<PersistentAvlTree<TKey, TValue>.Node<TKey, TValue>>(root.Height);
				PersistentAvlTree<TKey, TValue>.Node<TKey, TValue> node = root;
				do
				{
					if (node != null)
					{
						stack.Push(node);
						node = node.Left;
					}
					else
					{
						node = stack.Pop();
						yield return converter(node);
						node = node.Right;
					}
				}
				while (node != null || stack.Count > 0);
			}
			yield break;
		}

		private static IEnumerable<R> TraverseInMiddleLeftRightOrder<R>(PersistentAvlTree<TKey, TValue>.Node<TKey, TValue> root, Converter<PersistentAvlTree<TKey, TValue>.Node<TKey, TValue>, R> converter)
		{
			if (root != null)
			{
				Stack<PersistentAvlTree<TKey, TValue>.Node<TKey, TValue>> stack = new Stack<PersistentAvlTree<TKey, TValue>.Node<TKey, TValue>>(root.Height);
				PersistentAvlTree<TKey, TValue>.Node<TKey, TValue> node = root;
				do
				{
					if (node == null)
					{
						node = stack.Pop();
					}
					yield return converter(node);
					if (node.Right != null)
					{
						stack.Push(node.Right);
					}
					node = node.Left;
				}
				while (stack.Count > 0);
			}
			yield break;
		}

		private static IEnumerable<R> TraverseInMiddleRightLeftOrder<R>(PersistentAvlTree<TKey, TValue>.Node<TKey, TValue> root, Converter<PersistentAvlTree<TKey, TValue>.Node<TKey, TValue>, R> converter)
		{
			if (root != null)
			{
				Stack<PersistentAvlTree<TKey, TValue>.Node<TKey, TValue>> stack = new Stack<PersistentAvlTree<TKey, TValue>.Node<TKey, TValue>>(root.Height);
				PersistentAvlTree<TKey, TValue>.Node<TKey, TValue> node = root;
				do
				{
					if (node == null)
					{
						node = stack.Pop();
					}
					yield return converter(node);
					if (node.Left != null)
					{
						stack.Push(node.Left);
					}
					node = node.Right;
				}
				while (stack.Count > 0);
			}
			yield break;
		}

		private static readonly PersistentAvlTree<TKey, TValue> empty = new PersistentAvlTree<TKey, TValue>();

		private static readonly Converter<PersistentAvlTree<TKey, TValue>.Node<TKey, TValue>, TKey> KeyConverter = (PersistentAvlTree<TKey, TValue>.Node<TKey, TValue> node) => node.Key;

		private static readonly Converter<PersistentAvlTree<TKey, TValue>.Node<TKey, TValue>, TValue> ValueConverter = (PersistentAvlTree<TKey, TValue>.Node<TKey, TValue> node) => node.Value;

		private static readonly Converter<PersistentAvlTree<TKey, TValue>.Node<TKey, TValue>, KeyValuePair<TKey, TValue>> KeyValuePairConverter = (PersistentAvlTree<TKey, TValue>.Node<TKey, TValue> node) => new KeyValuePair<TKey, TValue>(node.Key, node.Value);

		private readonly PersistentAvlTree<TKey, TValue>.Node<TKey, TValue> root;

		private delegate R Convertor<T, R>(T value);

		private sealed class Node<K, V>
		{
			internal Node(PersistentAvlTree<TKey, TValue>.Node<K, V> left, PersistentAvlTree<TKey, TValue>.Node<K, V> right, K key, V value)
			{
				this.Left = left;
				this.Right = right;
				this.Key = key;
				this.Value = value;
				this.Height = 1 + Math.Max(PersistentAvlTree<TKey, TValue>.Node<K, V>.GetHeight(left), PersistentAvlTree<TKey, TValue>.Node<K, V>.GetHeight(right));
				this.Count = 1 + PersistentAvlTree<TKey, TValue>.Node<K, V>.GetCount(left) + PersistentAvlTree<TKey, TValue>.Node<K, V>.GetCount(right);
				this.Balance = PersistentAvlTree<TKey, TValue>.Node<K, V>.GetHeight(right) - PersistentAvlTree<TKey, TValue>.Node<K, V>.GetHeight(left);
			}

			internal static int GetHeight(PersistentAvlTree<TKey, TValue>.Node<K, V> node)
			{
				if (node == null)
				{
					return 0;
				}
				return node.Height;
			}

			internal static int GetCount(PersistentAvlTree<TKey, TValue>.Node<K, V> node)
			{
				if (node == null)
				{
					return 0;
				}
				return node.Count;
			}

			internal readonly PersistentAvlTree<TKey, TValue>.Node<K, V> Left;

			internal readonly PersistentAvlTree<TKey, TValue>.Node<K, V> Right;

			internal readonly K Key;

			internal readonly V Value;

			internal readonly int Height;

			internal readonly int Count;

			internal readonly int Balance;
		}
	}
}
