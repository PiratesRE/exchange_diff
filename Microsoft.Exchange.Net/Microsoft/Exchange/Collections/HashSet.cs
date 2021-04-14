using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Microsoft.Exchange.Net;

namespace Microsoft.Exchange.Collections
{
	[ComVisible(false)]
	[Serializable]
	internal class HashSet<T> : ICollection<T>, IEnumerable<T>, IEnumerable
	{
		public HashSet() : this(0, null)
		{
		}

		public HashSet(int capacity) : this(capacity, null)
		{
		}

		public HashSet(IEqualityComparer<T> comparer) : this(0, comparer)
		{
		}

		public HashSet(int capacity, IEqualityComparer<T> comparer)
		{
			if (capacity < 0)
			{
				throw new ArgumentOutOfRangeException("capacity");
			}
			if (capacity > 0)
			{
				this.Initialize(capacity);
			}
			if (comparer != null)
			{
				this.comparer = comparer;
				return;
			}
			this.comparer = EqualityComparer<T>.Default;
		}

		public HashSet(ICollection<T> collection) : this(collection, null)
		{
		}

		public HashSet(ICollection<T> collection, IEqualityComparer<T> comparer) : this((collection != null) ? collection.Count : 0, comparer)
		{
			if (collection == null)
			{
				throw new ArgumentNullException("collection");
			}
			foreach (T item in collection)
			{
				this.Add(item);
			}
		}

		public int Count
		{
			get
			{
				return this.usedEntriesCount - this.numberOfEntriesInFreeList;
			}
		}

		bool ICollection<!0>.IsReadOnly
		{
			get
			{
				return false;
			}
		}

		public void Add(T item)
		{
			if (!this.TryAdd(item))
			{
				throw new ArgumentException(NetException.DuplicateItem);
			}
		}

		public bool TryAdd(T item)
		{
			if (item == null)
			{
				throw new ArgumentNullException("item");
			}
			if (this.buckets == null)
			{
				this.Initialize(0);
			}
			int hashCode = this.GetHashCode(item);
			int num = hashCode % this.buckets.Length;
			for (int i = this.buckets[num]; i >= 0; i = this.entries[i].next)
			{
				if (this.entries[i].hashCode == hashCode && this.comparer.Equals(this.entries[i].item, item))
				{
					return false;
				}
			}
			bool flag;
			int freeEntry = this.GetFreeEntry(out flag);
			if (flag)
			{
				num = hashCode % this.buckets.Length;
			}
			this.entries[freeEntry].hashCode = hashCode;
			this.entries[freeEntry].next = this.buckets[num];
			this.entries[freeEntry].item = item;
			this.buckets[num] = freeEntry;
			this.version++;
			return true;
		}

		public void Clear()
		{
			if (this.usedEntriesCount > 0)
			{
				for (int i = 0; i < this.buckets.Length; i++)
				{
					this.buckets[i] = -1;
				}
				Array.Clear(this.entries, 0, this.usedEntriesCount);
				this.freeListStartIndex = -1;
				this.usedEntriesCount = 0;
				this.numberOfEntriesInFreeList = 0;
				this.version++;
			}
		}

		public bool Contains(T item)
		{
			if (item == null)
			{
				throw new ArgumentNullException("item");
			}
			if (this.buckets != null)
			{
				int hashCode = this.GetHashCode(item);
				for (int i = this.buckets[hashCode % this.buckets.Length]; i >= 0; i = this.entries[i].next)
				{
					if (this.entries[i].hashCode == hashCode && this.comparer.Equals(this.entries[i].item, item))
					{
						return true;
					}
				}
			}
			return false;
		}

		public void CopyTo(T[] array, int index)
		{
			if (array == null)
			{
				throw new ArgumentNullException("array");
			}
			if (index < 0 || index > array.Length - this.Count)
			{
				throw new ArgumentOutOfRangeException("index");
			}
			for (int i = 0; i < this.usedEntriesCount; i++)
			{
				if (this.entries[i].hashCode >= 0)
				{
					array[index++] = this.entries[i].item;
				}
			}
		}

		public HashSet<T>.Enumerator GetEnumerator()
		{
			return new HashSet<T>.Enumerator(this);
		}

		public bool Remove(T item)
		{
			if (item == null)
			{
				throw new ArgumentNullException("item");
			}
			if (this.buckets == null)
			{
				return false;
			}
			int hashCode = this.GetHashCode(item);
			int num = hashCode % this.buckets.Length;
			int num2 = -1;
			for (int i = this.buckets[num]; i >= 0; i = this.entries[i].next)
			{
				if (this.entries[i].hashCode == hashCode && this.comparer.Equals(this.entries[i].item, item))
				{
					if (num2 < 0)
					{
						this.buckets[num] = this.entries[i].next;
					}
					else
					{
						this.entries[num2].next = this.entries[i].next;
					}
					this.PutEntryIntoFreeList(i);
					this.version++;
					return true;
				}
				num2 = i;
			}
			return false;
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return new HashSet<T>.Enumerator(this);
		}

		IEnumerator<T> IEnumerable<!0>.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		public T[] ToArray()
		{
			T[] array = new T[this.Count];
			this.CopyTo(array, 0);
			return array;
		}

		private static int[] AllocateBuckets(int size)
		{
			int[] array = new int[size];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = -1;
			}
			return array;
		}

		private void Initialize(int capacity)
		{
			int prime = HashHelpers.GetPrime(capacity);
			this.buckets = HashSet<T>.AllocateBuckets(prime);
			this.entries = new HashSet<T>.Entry[prime];
			this.freeListStartIndex = -1;
		}

		private void Resize()
		{
			int prime = HashHelpers.GetPrime(this.usedEntriesCount * 2);
			int[] array = HashSet<T>.AllocateBuckets(prime);
			HashSet<T>.Entry[] array2 = new HashSet<T>.Entry[prime];
			Array.Copy(this.entries, 0, array2, 0, this.usedEntriesCount);
			for (int i = 0; i < this.usedEntriesCount; i++)
			{
				int num = array2[i].hashCode % prime;
				array2[i].next = array[num];
				array[num] = i;
			}
			this.buckets = array;
			this.entries = array2;
		}

		private void PutEntryIntoFreeList(int entryIndex)
		{
			this.entries[entryIndex].hashCode = -1;
			this.entries[entryIndex].next = this.freeListStartIndex;
			this.entries[entryIndex].item = default(T);
			this.freeListStartIndex = entryIndex;
			this.numberOfEntriesInFreeList++;
		}

		private int GetFreeEntry(out bool sizeChanged)
		{
			sizeChanged = false;
			int result;
			if (this.numberOfEntriesInFreeList > 0)
			{
				result = this.freeListStartIndex;
				this.freeListStartIndex = this.entries[this.freeListStartIndex].next;
				this.numberOfEntriesInFreeList--;
			}
			else
			{
				if (this.usedEntriesCount == this.entries.Length)
				{
					this.Resize();
					sizeChanged = true;
				}
				result = this.usedEntriesCount;
				this.usedEntriesCount++;
			}
			return result;
		}

		private int GetHashCode(T item)
		{
			return this.comparer.GetHashCode(item) & int.MaxValue;
		}

		private int[] buckets;

		private HashSet<T>.Entry[] entries;

		private int usedEntriesCount;

		private int freeListStartIndex;

		private int numberOfEntriesInFreeList;

		private int version;

		private IEqualityComparer<T> comparer;

		[Serializable]
		public struct Enumerator : IEnumerator<T>, IDisposable, IEnumerator
		{
			internal Enumerator(HashSet<T> set)
			{
				this.set = set;
				this.version = set.version;
				this.currentEntryIndex = 0;
				this.currentItem = default(T);
			}

			public T Current
			{
				get
				{
					return this.currentItem;
				}
			}

			object IEnumerator.Current
			{
				get
				{
					return this.Current;
				}
			}

			public bool MoveNext()
			{
				if (this.version != this.set.version)
				{
					throw new InvalidOperationException(NetException.CollectionChanged);
				}
				while (this.currentEntryIndex < this.set.usedEntriesCount)
				{
					if (this.set.entries[this.currentEntryIndex].hashCode >= 0)
					{
						this.currentItem = this.set.entries[this.currentEntryIndex].item;
						this.currentEntryIndex++;
						return true;
					}
					this.currentEntryIndex++;
				}
				this.currentEntryIndex = this.set.usedEntriesCount + 1;
				this.currentItem = default(T);
				return false;
			}

			public void Dispose()
			{
			}

			void IEnumerator.Reset()
			{
				this.currentEntryIndex = 0;
				this.currentItem = default(T);
			}

			private HashSet<T> set;

			private int version;

			private int currentEntryIndex;

			private T currentItem;
		}

		[Serializable]
		private struct Entry
		{
			public int hashCode;

			public int next;

			public T item;
		}
	}
}
