using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Security.Permissions;
using System.Threading;

namespace System.Collections.Concurrent
{
	[ComVisible(false)]
	[DebuggerTypeProxy(typeof(Mscorlib_DictionaryDebugView<, >))]
	[DebuggerDisplay("Count = {Count}")]
	[__DynamicallyInvokable]
	[HostProtection(SecurityAction.LinkDemand, Synchronization = true, ExternalThreading = true)]
	[Serializable]
	public class ConcurrentDictionary<TKey, TValue> : IDictionary<TKey, TValue>, ICollection<KeyValuePair<TKey, TValue>>, IEnumerable<KeyValuePair<TKey, TValue>>, IEnumerable, IDictionary, ICollection, IReadOnlyDictionary<!0, !1>, IReadOnlyCollection<KeyValuePair<TKey, TValue>>
	{
		private static bool IsValueWriteAtomic()
		{
			Type typeFromHandle = typeof(TValue);
			if (typeFromHandle.IsClass)
			{
				return true;
			}
			switch (Type.GetTypeCode(typeFromHandle))
			{
			case TypeCode.Boolean:
			case TypeCode.Char:
			case TypeCode.SByte:
			case TypeCode.Byte:
			case TypeCode.Int16:
			case TypeCode.UInt16:
			case TypeCode.Int32:
			case TypeCode.UInt32:
			case TypeCode.Single:
				return true;
			case TypeCode.Int64:
			case TypeCode.UInt64:
			case TypeCode.Double:
				return IntPtr.Size == 8;
			default:
				return false;
			}
		}

		[__DynamicallyInvokable]
		public ConcurrentDictionary() : this(ConcurrentDictionary<TKey, TValue>.DefaultConcurrencyLevel, 31, true, EqualityComparer<TKey>.Default)
		{
		}

		[__DynamicallyInvokable]
		public ConcurrentDictionary(int concurrencyLevel, int capacity) : this(concurrencyLevel, capacity, false, EqualityComparer<TKey>.Default)
		{
		}

		[__DynamicallyInvokable]
		public ConcurrentDictionary(IEnumerable<KeyValuePair<TKey, TValue>> collection) : this(collection, EqualityComparer<TKey>.Default)
		{
		}

		[__DynamicallyInvokable]
		public ConcurrentDictionary(IEqualityComparer<TKey> comparer) : this(ConcurrentDictionary<TKey, TValue>.DefaultConcurrencyLevel, 31, true, comparer)
		{
		}

		[__DynamicallyInvokable]
		public ConcurrentDictionary(IEnumerable<KeyValuePair<TKey, TValue>> collection, IEqualityComparer<TKey> comparer) : this(comparer)
		{
			if (collection == null)
			{
				throw new ArgumentNullException("collection");
			}
			this.InitializeFromCollection(collection);
		}

		[__DynamicallyInvokable]
		public ConcurrentDictionary(int concurrencyLevel, IEnumerable<KeyValuePair<TKey, TValue>> collection, IEqualityComparer<TKey> comparer) : this(concurrencyLevel, 31, false, comparer)
		{
			if (collection == null)
			{
				throw new ArgumentNullException("collection");
			}
			if (comparer == null)
			{
				throw new ArgumentNullException("comparer");
			}
			this.InitializeFromCollection(collection);
		}

		private void InitializeFromCollection(IEnumerable<KeyValuePair<TKey, TValue>> collection)
		{
			foreach (KeyValuePair<TKey, TValue> keyValuePair in collection)
			{
				if (keyValuePair.Key == null)
				{
					throw new ArgumentNullException("key");
				}
				TValue tvalue;
				if (!this.TryAddInternal(keyValuePair.Key, keyValuePair.Value, false, false, out tvalue))
				{
					throw new ArgumentException(this.GetResource("ConcurrentDictionary_SourceContainsDuplicateKeys"));
				}
			}
			if (this.m_budget == 0)
			{
				this.m_budget = this.m_tables.m_buckets.Length / this.m_tables.m_locks.Length;
			}
		}

		[__DynamicallyInvokable]
		public ConcurrentDictionary(int concurrencyLevel, int capacity, IEqualityComparer<TKey> comparer) : this(concurrencyLevel, capacity, false, comparer)
		{
		}

		internal ConcurrentDictionary(int concurrencyLevel, int capacity, bool growLockArray, IEqualityComparer<TKey> comparer)
		{
			if (concurrencyLevel < 1)
			{
				throw new ArgumentOutOfRangeException("concurrencyLevel", this.GetResource("ConcurrentDictionary_ConcurrencyLevelMustBePositive"));
			}
			if (capacity < 0)
			{
				throw new ArgumentOutOfRangeException("capacity", this.GetResource("ConcurrentDictionary_CapacityMustNotBeNegative"));
			}
			if (comparer == null)
			{
				throw new ArgumentNullException("comparer");
			}
			if (capacity < concurrencyLevel)
			{
				capacity = concurrencyLevel;
			}
			object[] array = new object[concurrencyLevel];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = new object();
			}
			int[] countPerLock = new int[array.Length];
			ConcurrentDictionary<TKey, TValue>.Node[] array2 = new ConcurrentDictionary<TKey, TValue>.Node[capacity];
			this.m_tables = new ConcurrentDictionary<TKey, TValue>.Tables(array2, array, countPerLock, comparer);
			this.m_growLockArray = growLockArray;
			this.m_budget = array2.Length / array.Length;
		}

		[__DynamicallyInvokable]
		public bool TryAdd(TKey key, TValue value)
		{
			if (key == null)
			{
				throw new ArgumentNullException("key");
			}
			TValue tvalue;
			return this.TryAddInternal(key, value, false, true, out tvalue);
		}

		[__DynamicallyInvokable]
		public bool ContainsKey(TKey key)
		{
			if (key == null)
			{
				throw new ArgumentNullException("key");
			}
			TValue tvalue;
			return this.TryGetValue(key, out tvalue);
		}

		[__DynamicallyInvokable]
		public bool TryRemove(TKey key, out TValue value)
		{
			if (key == null)
			{
				throw new ArgumentNullException("key");
			}
			return this.TryRemoveInternal(key, out value, false, default(TValue));
		}

		private bool TryRemoveInternal(TKey key, out TValue value, bool matchValue, TValue oldValue)
		{
			for (;;)
			{
				ConcurrentDictionary<TKey, TValue>.Tables tables = this.m_tables;
				IEqualityComparer<TKey> comparer = tables.m_comparer;
				int num;
				int num2;
				this.GetBucketAndLockNo(comparer.GetHashCode(key), out num, out num2, tables.m_buckets.Length, tables.m_locks.Length);
				object obj = tables.m_locks[num2];
				lock (obj)
				{
					if (tables != this.m_tables)
					{
						continue;
					}
					ConcurrentDictionary<TKey, TValue>.Node node = null;
					ConcurrentDictionary<TKey, TValue>.Node node2 = tables.m_buckets[num];
					while (node2 != null)
					{
						if (comparer.Equals(node2.m_key, key))
						{
							if (matchValue && !EqualityComparer<TValue>.Default.Equals(oldValue, node2.m_value))
							{
								value = default(TValue);
								return false;
							}
							if (node == null)
							{
								Volatile.Write<ConcurrentDictionary<TKey, TValue>.Node>(ref tables.m_buckets[num], node2.m_next);
							}
							else
							{
								node.m_next = node2.m_next;
							}
							value = node2.m_value;
							tables.m_countPerLock[num2]--;
							return true;
						}
						else
						{
							node = node2;
							node2 = node2.m_next;
						}
					}
				}
				break;
			}
			value = default(TValue);
			return false;
		}

		[__DynamicallyInvokable]
		public bool TryGetValue(TKey key, out TValue value)
		{
			if (key == null)
			{
				throw new ArgumentNullException("key");
			}
			ConcurrentDictionary<TKey, TValue>.Tables tables = this.m_tables;
			IEqualityComparer<TKey> comparer = tables.m_comparer;
			int num;
			int num2;
			this.GetBucketAndLockNo(comparer.GetHashCode(key), out num, out num2, tables.m_buckets.Length, tables.m_locks.Length);
			for (ConcurrentDictionary<TKey, TValue>.Node node = Volatile.Read<ConcurrentDictionary<TKey, TValue>.Node>(ref tables.m_buckets[num]); node != null; node = node.m_next)
			{
				if (comparer.Equals(node.m_key, key))
				{
					value = node.m_value;
					return true;
				}
			}
			value = default(TValue);
			return false;
		}

		[__DynamicallyInvokable]
		public bool TryUpdate(TKey key, TValue newValue, TValue comparisonValue)
		{
			if (key == null)
			{
				throw new ArgumentNullException("key");
			}
			IEqualityComparer<TValue> @default = EqualityComparer<TValue>.Default;
			bool result;
			for (;;)
			{
				ConcurrentDictionary<TKey, TValue>.Tables tables = this.m_tables;
				IEqualityComparer<TKey> comparer = tables.m_comparer;
				int hashCode = comparer.GetHashCode(key);
				int num;
				int num2;
				this.GetBucketAndLockNo(hashCode, out num, out num2, tables.m_buckets.Length, tables.m_locks.Length);
				object obj = tables.m_locks[num2];
				lock (obj)
				{
					if (tables != this.m_tables)
					{
						continue;
					}
					ConcurrentDictionary<TKey, TValue>.Node node = null;
					ConcurrentDictionary<TKey, TValue>.Node node2 = tables.m_buckets[num];
					while (node2 != null)
					{
						if (comparer.Equals(node2.m_key, key))
						{
							if (@default.Equals(node2.m_value, comparisonValue))
							{
								if (ConcurrentDictionary<TKey, TValue>.s_isValueWriteAtomic)
								{
									node2.m_value = newValue;
								}
								else
								{
									ConcurrentDictionary<TKey, TValue>.Node node3 = new ConcurrentDictionary<TKey, TValue>.Node(node2.m_key, newValue, hashCode, node2.m_next);
									if (node == null)
									{
										tables.m_buckets[num] = node3;
									}
									else
									{
										node.m_next = node3;
									}
								}
								return true;
							}
							return false;
						}
						else
						{
							node = node2;
							node2 = node2.m_next;
						}
					}
					result = false;
				}
				break;
			}
			return result;
		}

		[__DynamicallyInvokable]
		public void Clear()
		{
			int toExclusive = 0;
			try
			{
				this.AcquireAllLocks(ref toExclusive);
				ConcurrentDictionary<TKey, TValue>.Tables tables = new ConcurrentDictionary<TKey, TValue>.Tables(new ConcurrentDictionary<TKey, TValue>.Node[31], this.m_tables.m_locks, new int[this.m_tables.m_countPerLock.Length], this.m_tables.m_comparer);
				this.m_tables = tables;
				this.m_budget = Math.Max(1, tables.m_buckets.Length / tables.m_locks.Length);
			}
			finally
			{
				this.ReleaseLocks(0, toExclusive);
			}
		}

		[__DynamicallyInvokable]
		void ICollection<KeyValuePair<!0, !1>>.CopyTo(KeyValuePair<TKey, TValue>[] array, int index)
		{
			if (array == null)
			{
				throw new ArgumentNullException("array");
			}
			if (index < 0)
			{
				throw new ArgumentOutOfRangeException("index", this.GetResource("ConcurrentDictionary_IndexIsNegative"));
			}
			int toExclusive = 0;
			try
			{
				this.AcquireAllLocks(ref toExclusive);
				int num = 0;
				int num2 = 0;
				while (num2 < this.m_tables.m_locks.Length && num >= 0)
				{
					num += this.m_tables.m_countPerLock[num2];
					num2++;
				}
				if (array.Length - num < index || num < 0)
				{
					throw new ArgumentException(this.GetResource("ConcurrentDictionary_ArrayNotLargeEnough"));
				}
				this.CopyToPairs(array, index);
			}
			finally
			{
				this.ReleaseLocks(0, toExclusive);
			}
		}

		[__DynamicallyInvokable]
		public KeyValuePair<TKey, TValue>[] ToArray()
		{
			int toExclusive = 0;
			checked
			{
				KeyValuePair<TKey, TValue>[] result;
				try
				{
					this.AcquireAllLocks(ref toExclusive);
					int num = 0;
					for (int i = 0; i < this.m_tables.m_locks.Length; i++)
					{
						num += this.m_tables.m_countPerLock[i];
					}
					if (num == 0)
					{
						result = Array.Empty<KeyValuePair<TKey, TValue>>();
					}
					else
					{
						KeyValuePair<TKey, TValue>[] array = new KeyValuePair<TKey, TValue>[num];
						this.CopyToPairs(array, 0);
						result = array;
					}
				}
				finally
				{
					this.ReleaseLocks(0, toExclusive);
				}
				return result;
			}
		}

		private void CopyToPairs(KeyValuePair<TKey, TValue>[] array, int index)
		{
			foreach (ConcurrentDictionary<TKey, TValue>.Node node in this.m_tables.m_buckets)
			{
				while (node != null)
				{
					array[index] = new KeyValuePair<TKey, TValue>(node.m_key, node.m_value);
					index++;
					node = node.m_next;
				}
			}
		}

		private void CopyToEntries(DictionaryEntry[] array, int index)
		{
			foreach (ConcurrentDictionary<TKey, TValue>.Node node in this.m_tables.m_buckets)
			{
				while (node != null)
				{
					array[index] = new DictionaryEntry(node.m_key, node.m_value);
					index++;
					node = node.m_next;
				}
			}
		}

		private void CopyToObjects(object[] array, int index)
		{
			foreach (ConcurrentDictionary<TKey, TValue>.Node node in this.m_tables.m_buckets)
			{
				while (node != null)
				{
					array[index] = new KeyValuePair<TKey, TValue>(node.m_key, node.m_value);
					index++;
					node = node.m_next;
				}
			}
		}

		[__DynamicallyInvokable]
		public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
		{
			ConcurrentDictionary<TKey, TValue>.Node[] buckets = this.m_tables.m_buckets;
			int num;
			for (int i = 0; i < buckets.Length; i = num + 1)
			{
				ConcurrentDictionary<TKey, TValue>.Node current;
				for (current = Volatile.Read<ConcurrentDictionary<TKey, TValue>.Node>(ref buckets[i]); current != null; current = current.m_next)
				{
					yield return new KeyValuePair<TKey, TValue>(current.m_key, current.m_value);
				}
				current = null;
				num = i;
			}
			yield break;
		}

		private bool TryAddInternal(TKey key, TValue value, bool updateIfExists, bool acquireLock, out TValue resultingValue)
		{
			ConcurrentDictionary<TKey, TValue>.Tables tables;
			IEqualityComparer<TKey> comparer;
			bool flag;
			bool flag3;
			for (;;)
			{
				tables = this.m_tables;
				comparer = tables.m_comparer;
				int hashCode = comparer.GetHashCode(key);
				int num;
				int num2;
				this.GetBucketAndLockNo(hashCode, out num, out num2, tables.m_buckets.Length, tables.m_locks.Length);
				flag = false;
				bool flag2 = false;
				flag3 = false;
				try
				{
					if (acquireLock)
					{
						Monitor.Enter(tables.m_locks[num2], ref flag2);
					}
					if (tables != this.m_tables)
					{
						continue;
					}
					int num3 = 0;
					ConcurrentDictionary<TKey, TValue>.Node node = null;
					for (ConcurrentDictionary<TKey, TValue>.Node node2 = tables.m_buckets[num]; node2 != null; node2 = node2.m_next)
					{
						if (comparer.Equals(node2.m_key, key))
						{
							if (updateIfExists)
							{
								if (ConcurrentDictionary<TKey, TValue>.s_isValueWriteAtomic)
								{
									node2.m_value = value;
								}
								else
								{
									ConcurrentDictionary<TKey, TValue>.Node node3 = new ConcurrentDictionary<TKey, TValue>.Node(node2.m_key, value, hashCode, node2.m_next);
									if (node == null)
									{
										tables.m_buckets[num] = node3;
									}
									else
									{
										node.m_next = node3;
									}
								}
								resultingValue = value;
							}
							else
							{
								resultingValue = node2.m_value;
							}
							return false;
						}
						node = node2;
						num3++;
					}
					if (num3 > 100 && HashHelpers.IsWellKnownEqualityComparer(comparer))
					{
						flag = true;
						flag3 = true;
					}
					Volatile.Write<ConcurrentDictionary<TKey, TValue>.Node>(ref tables.m_buckets[num], new ConcurrentDictionary<TKey, TValue>.Node(key, value, hashCode, tables.m_buckets[num]));
					checked
					{
						tables.m_countPerLock[num2]++;
						if (tables.m_countPerLock[num2] > this.m_budget)
						{
							flag = true;
						}
					}
				}
				finally
				{
					if (flag2)
					{
						Monitor.Exit(tables.m_locks[num2]);
					}
				}
				break;
			}
			if (flag)
			{
				if (flag3)
				{
					this.GrowTable(tables, (IEqualityComparer<TKey>)HashHelpers.GetRandomizedEqualityComparer(comparer), true, this.m_keyRehashCount);
				}
				else
				{
					this.GrowTable(tables, tables.m_comparer, false, this.m_keyRehashCount);
				}
			}
			resultingValue = value;
			return true;
		}

		[__DynamicallyInvokable]
		public TValue this[TKey key]
		{
			[__DynamicallyInvokable]
			get
			{
				TValue result;
				if (!this.TryGetValue(key, out result))
				{
					throw new KeyNotFoundException();
				}
				return result;
			}
			[__DynamicallyInvokable]
			set
			{
				if (key == null)
				{
					throw new ArgumentNullException("key");
				}
				TValue tvalue;
				this.TryAddInternal(key, value, true, true, out tvalue);
			}
		}

		[__DynamicallyInvokable]
		public int Count
		{
			[__DynamicallyInvokable]
			get
			{
				int toExclusive = 0;
				int countInternal;
				try
				{
					this.AcquireAllLocks(ref toExclusive);
					countInternal = this.GetCountInternal();
				}
				finally
				{
					this.ReleaseLocks(0, toExclusive);
				}
				return countInternal;
			}
		}

		private int GetCountInternal()
		{
			int num = 0;
			for (int i = 0; i < this.m_tables.m_countPerLock.Length; i++)
			{
				num += this.m_tables.m_countPerLock[i];
			}
			return num;
		}

		[__DynamicallyInvokable]
		public TValue GetOrAdd(TKey key, Func<TKey, TValue> valueFactory)
		{
			if (key == null)
			{
				throw new ArgumentNullException("key");
			}
			if (valueFactory == null)
			{
				throw new ArgumentNullException("valueFactory");
			}
			TValue result;
			if (this.TryGetValue(key, out result))
			{
				return result;
			}
			this.TryAddInternal(key, valueFactory(key), false, true, out result);
			return result;
		}

		[__DynamicallyInvokable]
		public TValue GetOrAdd(TKey key, TValue value)
		{
			if (key == null)
			{
				throw new ArgumentNullException("key");
			}
			TValue result;
			this.TryAddInternal(key, value, false, true, out result);
			return result;
		}

		public TValue GetOrAdd<TArg>(TKey key, Func<TKey, TArg, TValue> valueFactory, TArg factoryArgument)
		{
			if (key == null)
			{
				throw new ArgumentNullException("key");
			}
			if (valueFactory == null)
			{
				throw new ArgumentNullException("valueFactory");
			}
			TValue result;
			if (!this.TryGetValue(key, out result))
			{
				this.TryAddInternal(key, valueFactory(key, factoryArgument), false, true, out result);
			}
			return result;
		}

		public TValue AddOrUpdate<TArg>(TKey key, Func<TKey, TArg, TValue> addValueFactory, Func<TKey, TValue, TArg, TValue> updateValueFactory, TArg factoryArgument)
		{
			if (key == null)
			{
				throw new ArgumentNullException("key");
			}
			if (addValueFactory == null)
			{
				throw new ArgumentNullException("addValueFactory");
			}
			if (updateValueFactory == null)
			{
				throw new ArgumentNullException("updateValueFactory");
			}
			TValue tvalue2;
			for (;;)
			{
				TValue tvalue;
				TValue result;
				if (this.TryGetValue(key, out tvalue))
				{
					tvalue2 = updateValueFactory(key, tvalue, factoryArgument);
					if (this.TryUpdate(key, tvalue2, tvalue))
					{
						break;
					}
				}
				else if (this.TryAddInternal(key, addValueFactory(key, factoryArgument), false, true, out result))
				{
					return result;
				}
			}
			return tvalue2;
		}

		[__DynamicallyInvokable]
		public TValue AddOrUpdate(TKey key, Func<TKey, TValue> addValueFactory, Func<TKey, TValue, TValue> updateValueFactory)
		{
			if (key == null)
			{
				throw new ArgumentNullException("key");
			}
			if (addValueFactory == null)
			{
				throw new ArgumentNullException("addValueFactory");
			}
			if (updateValueFactory == null)
			{
				throw new ArgumentNullException("updateValueFactory");
			}
			TValue tvalue2;
			for (;;)
			{
				TValue tvalue;
				if (this.TryGetValue(key, out tvalue))
				{
					tvalue2 = updateValueFactory(key, tvalue);
					if (this.TryUpdate(key, tvalue2, tvalue))
					{
						break;
					}
				}
				else
				{
					tvalue2 = addValueFactory(key);
					TValue result;
					if (this.TryAddInternal(key, tvalue2, false, true, out result))
					{
						return result;
					}
				}
			}
			return tvalue2;
		}

		[__DynamicallyInvokable]
		public TValue AddOrUpdate(TKey key, TValue addValue, Func<TKey, TValue, TValue> updateValueFactory)
		{
			if (key == null)
			{
				throw new ArgumentNullException("key");
			}
			if (updateValueFactory == null)
			{
				throw new ArgumentNullException("updateValueFactory");
			}
			TValue tvalue2;
			for (;;)
			{
				TValue tvalue;
				TValue result;
				if (this.TryGetValue(key, out tvalue))
				{
					tvalue2 = updateValueFactory(key, tvalue);
					if (this.TryUpdate(key, tvalue2, tvalue))
					{
						break;
					}
				}
				else if (this.TryAddInternal(key, addValue, false, true, out result))
				{
					return result;
				}
			}
			return tvalue2;
		}

		[__DynamicallyInvokable]
		public bool IsEmpty
		{
			[__DynamicallyInvokable]
			get
			{
				int toExclusive = 0;
				try
				{
					this.AcquireAllLocks(ref toExclusive);
					for (int i = 0; i < this.m_tables.m_countPerLock.Length; i++)
					{
						if (this.m_tables.m_countPerLock[i] != 0)
						{
							return false;
						}
					}
				}
				finally
				{
					this.ReleaseLocks(0, toExclusive);
				}
				return true;
			}
		}

		[__DynamicallyInvokable]
		void IDictionary<!0, !1>.Add(TKey key, TValue value)
		{
			if (!this.TryAdd(key, value))
			{
				throw new ArgumentException(this.GetResource("ConcurrentDictionary_KeyAlreadyExisted"));
			}
		}

		[__DynamicallyInvokable]
		bool IDictionary<!0, !1>.Remove(TKey key)
		{
			TValue tvalue;
			return this.TryRemove(key, out tvalue);
		}

		[__DynamicallyInvokable]
		public ICollection<TKey> Keys
		{
			[__DynamicallyInvokable]
			get
			{
				return this.GetKeys();
			}
		}

		[__DynamicallyInvokable]
		IEnumerable<TKey> IReadOnlyDictionary<!0, !1>.Keys
		{
			[__DynamicallyInvokable]
			get
			{
				return this.GetKeys();
			}
		}

		[__DynamicallyInvokable]
		public ICollection<TValue> Values
		{
			[__DynamicallyInvokable]
			get
			{
				return this.GetValues();
			}
		}

		[__DynamicallyInvokable]
		IEnumerable<TValue> IReadOnlyDictionary<!0, !1>.Values
		{
			[__DynamicallyInvokable]
			get
			{
				return this.GetValues();
			}
		}

		[__DynamicallyInvokable]
		void ICollection<KeyValuePair<!0, !1>>.Add(KeyValuePair<TKey, TValue> keyValuePair)
		{
			((IDictionary<!0, !1>)this).Add(keyValuePair.Key, keyValuePair.Value);
		}

		[__DynamicallyInvokable]
		bool ICollection<KeyValuePair<!0, !1>>.Contains(KeyValuePair<TKey, TValue> keyValuePair)
		{
			TValue x;
			return this.TryGetValue(keyValuePair.Key, out x) && EqualityComparer<TValue>.Default.Equals(x, keyValuePair.Value);
		}

		[__DynamicallyInvokable]
		bool ICollection<KeyValuePair<!0, !1>>.IsReadOnly
		{
			[__DynamicallyInvokable]
			get
			{
				return false;
			}
		}

		[__DynamicallyInvokable]
		bool ICollection<KeyValuePair<!0, !1>>.Remove(KeyValuePair<TKey, TValue> keyValuePair)
		{
			if (keyValuePair.Key == null)
			{
				throw new ArgumentNullException(this.GetResource("ConcurrentDictionary_ItemKeyIsNull"));
			}
			TValue tvalue;
			return this.TryRemoveInternal(keyValuePair.Key, out tvalue, true, keyValuePair.Value);
		}

		[__DynamicallyInvokable]
		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		[__DynamicallyInvokable]
		void IDictionary.Add(object key, object value)
		{
			if (key == null)
			{
				throw new ArgumentNullException("key");
			}
			if (!(key is TKey))
			{
				throw new ArgumentException(this.GetResource("ConcurrentDictionary_TypeOfKeyIncorrect"));
			}
			TValue value2;
			try
			{
				value2 = (TValue)((object)value);
			}
			catch (InvalidCastException)
			{
				throw new ArgumentException(this.GetResource("ConcurrentDictionary_TypeOfValueIncorrect"));
			}
			((IDictionary<!0, !1>)this).Add((TKey)((object)key), value2);
		}

		[__DynamicallyInvokable]
		bool IDictionary.Contains(object key)
		{
			if (key == null)
			{
				throw new ArgumentNullException("key");
			}
			return key is TKey && this.ContainsKey((TKey)((object)key));
		}

		[__DynamicallyInvokable]
		IDictionaryEnumerator IDictionary.GetEnumerator()
		{
			return new ConcurrentDictionary<TKey, TValue>.DictionaryEnumerator(this);
		}

		[__DynamicallyInvokable]
		bool IDictionary.IsFixedSize
		{
			[__DynamicallyInvokable]
			get
			{
				return false;
			}
		}

		[__DynamicallyInvokable]
		bool IDictionary.IsReadOnly
		{
			[__DynamicallyInvokable]
			get
			{
				return false;
			}
		}

		[__DynamicallyInvokable]
		ICollection IDictionary.Keys
		{
			[__DynamicallyInvokable]
			get
			{
				return this.GetKeys();
			}
		}

		[__DynamicallyInvokable]
		void IDictionary.Remove(object key)
		{
			if (key == null)
			{
				throw new ArgumentNullException("key");
			}
			if (key is TKey)
			{
				TValue tvalue;
				this.TryRemove((TKey)((object)key), out tvalue);
			}
		}

		[__DynamicallyInvokable]
		ICollection IDictionary.Values
		{
			[__DynamicallyInvokable]
			get
			{
				return this.GetValues();
			}
		}

		[__DynamicallyInvokable]
		object IDictionary.this[object key]
		{
			[__DynamicallyInvokable]
			get
			{
				if (key == null)
				{
					throw new ArgumentNullException("key");
				}
				TValue tvalue;
				if (key is TKey && this.TryGetValue((TKey)((object)key), out tvalue))
				{
					return tvalue;
				}
				return null;
			}
			[__DynamicallyInvokable]
			set
			{
				if (key == null)
				{
					throw new ArgumentNullException("key");
				}
				if (!(key is TKey))
				{
					throw new ArgumentException(this.GetResource("ConcurrentDictionary_TypeOfKeyIncorrect"));
				}
				if (!(value is TValue))
				{
					throw new ArgumentException(this.GetResource("ConcurrentDictionary_TypeOfValueIncorrect"));
				}
				this[(TKey)((object)key)] = (TValue)((object)value);
			}
		}

		[__DynamicallyInvokable]
		void ICollection.CopyTo(Array array, int index)
		{
			if (array == null)
			{
				throw new ArgumentNullException("array");
			}
			if (index < 0)
			{
				throw new ArgumentOutOfRangeException("index", this.GetResource("ConcurrentDictionary_IndexIsNegative"));
			}
			int toExclusive = 0;
			try
			{
				this.AcquireAllLocks(ref toExclusive);
				ConcurrentDictionary<TKey, TValue>.Tables tables = this.m_tables;
				int num = 0;
				int num2 = 0;
				while (num2 < tables.m_locks.Length && num >= 0)
				{
					num += tables.m_countPerLock[num2];
					num2++;
				}
				if (array.Length - num < index || num < 0)
				{
					throw new ArgumentException(this.GetResource("ConcurrentDictionary_ArrayNotLargeEnough"));
				}
				KeyValuePair<TKey, TValue>[] array2 = array as KeyValuePair<TKey, TValue>[];
				if (array2 != null)
				{
					this.CopyToPairs(array2, index);
				}
				else
				{
					DictionaryEntry[] array3 = array as DictionaryEntry[];
					if (array3 != null)
					{
						this.CopyToEntries(array3, index);
					}
					else
					{
						object[] array4 = array as object[];
						if (array4 == null)
						{
							throw new ArgumentException(this.GetResource("ConcurrentDictionary_ArrayIncorrectType"), "array");
						}
						this.CopyToObjects(array4, index);
					}
				}
			}
			finally
			{
				this.ReleaseLocks(0, toExclusive);
			}
		}

		[__DynamicallyInvokable]
		bool ICollection.IsSynchronized
		{
			[__DynamicallyInvokable]
			get
			{
				return false;
			}
		}

		[__DynamicallyInvokable]
		object ICollection.SyncRoot
		{
			[__DynamicallyInvokable]
			get
			{
				throw new NotSupportedException(Environment.GetResourceString("ConcurrentCollection_SyncRoot_NotSupported"));
			}
		}

		private void GrowTable(ConcurrentDictionary<TKey, TValue>.Tables tables, IEqualityComparer<TKey> newComparer, bool regenerateHashKeys, int rehashCount)
		{
			int toExclusive = 0;
			try
			{
				this.AcquireLocks(0, 1, ref toExclusive);
				if (regenerateHashKeys && rehashCount == this.m_keyRehashCount)
				{
					tables = this.m_tables;
				}
				else
				{
					if (tables != this.m_tables)
					{
						return;
					}
					long num = 0L;
					for (int i = 0; i < tables.m_countPerLock.Length; i++)
					{
						num += (long)tables.m_countPerLock[i];
					}
					if (num < (long)(tables.m_buckets.Length / 4))
					{
						this.m_budget = 2 * this.m_budget;
						if (this.m_budget < 0)
						{
							this.m_budget = int.MaxValue;
						}
						return;
					}
				}
				int num2 = 0;
				bool flag = false;
				object[] array;
				checked
				{
					try
					{
						num2 = tables.m_buckets.Length * 2 + 1;
						while (num2 % 3 == 0 || num2 % 5 == 0 || num2 % 7 == 0)
						{
							num2 += 2;
						}
						if (num2 > 2146435071)
						{
							flag = true;
						}
					}
					catch (OverflowException)
					{
						flag = true;
					}
					if (flag)
					{
						num2 = 2146435071;
						this.m_budget = int.MaxValue;
					}
					this.AcquireLocks(1, tables.m_locks.Length, ref toExclusive);
					array = tables.m_locks;
				}
				if (this.m_growLockArray && tables.m_locks.Length < 1024)
				{
					array = new object[tables.m_locks.Length * 2];
					Array.Copy(tables.m_locks, array, tables.m_locks.Length);
					for (int j = tables.m_locks.Length; j < array.Length; j++)
					{
						array[j] = new object();
					}
				}
				ConcurrentDictionary<TKey, TValue>.Node[] array2 = new ConcurrentDictionary<TKey, TValue>.Node[num2];
				int[] array3 = new int[array.Length];
				for (int k = 0; k < tables.m_buckets.Length; k++)
				{
					checked
					{
						ConcurrentDictionary<TKey, TValue>.Node next;
						for (ConcurrentDictionary<TKey, TValue>.Node node = tables.m_buckets[k]; node != null; node = next)
						{
							next = node.m_next;
							int hashcode = node.m_hashcode;
							if (regenerateHashKeys)
							{
								hashcode = newComparer.GetHashCode(node.m_key);
							}
							int num3;
							int num4;
							this.GetBucketAndLockNo(hashcode, out num3, out num4, array2.Length, array.Length);
							array2[num3] = new ConcurrentDictionary<TKey, TValue>.Node(node.m_key, node.m_value, hashcode, array2[num3]);
							array3[num4]++;
						}
					}
				}
				if (regenerateHashKeys)
				{
					this.m_keyRehashCount++;
				}
				this.m_budget = Math.Max(1, array2.Length / array.Length);
				this.m_tables = new ConcurrentDictionary<TKey, TValue>.Tables(array2, array, array3, newComparer);
			}
			finally
			{
				this.ReleaseLocks(0, toExclusive);
			}
		}

		private void GetBucketAndLockNo(int hashcode, out int bucketNo, out int lockNo, int bucketCount, int lockCount)
		{
			bucketNo = (hashcode & int.MaxValue) % bucketCount;
			lockNo = bucketNo % lockCount;
		}

		private static int DefaultConcurrencyLevel
		{
			get
			{
				return PlatformHelper.ProcessorCount;
			}
		}

		private void AcquireAllLocks(ref int locksAcquired)
		{
			if (CDSCollectionETWBCLProvider.Log.IsEnabled())
			{
				CDSCollectionETWBCLProvider.Log.ConcurrentDictionary_AcquiringAllLocks(this.m_tables.m_buckets.Length);
			}
			this.AcquireLocks(0, 1, ref locksAcquired);
			this.AcquireLocks(1, this.m_tables.m_locks.Length, ref locksAcquired);
		}

		private void AcquireLocks(int fromInclusive, int toExclusive, ref int locksAcquired)
		{
			object[] locks = this.m_tables.m_locks;
			for (int i = fromInclusive; i < toExclusive; i++)
			{
				bool flag = false;
				try
				{
					Monitor.Enter(locks[i], ref flag);
				}
				finally
				{
					if (flag)
					{
						locksAcquired++;
					}
				}
			}
		}

		private void ReleaseLocks(int fromInclusive, int toExclusive)
		{
			for (int i = fromInclusive; i < toExclusive; i++)
			{
				Monitor.Exit(this.m_tables.m_locks[i]);
			}
		}

		private ReadOnlyCollection<TKey> GetKeys()
		{
			int toExclusive = 0;
			ReadOnlyCollection<TKey> result;
			try
			{
				this.AcquireAllLocks(ref toExclusive);
				int countInternal = this.GetCountInternal();
				if (countInternal < 0)
				{
					throw new OutOfMemoryException();
				}
				List<TKey> list = new List<TKey>(countInternal);
				for (int i = 0; i < this.m_tables.m_buckets.Length; i++)
				{
					for (ConcurrentDictionary<TKey, TValue>.Node node = this.m_tables.m_buckets[i]; node != null; node = node.m_next)
					{
						list.Add(node.m_key);
					}
				}
				result = new ReadOnlyCollection<TKey>(list);
			}
			finally
			{
				this.ReleaseLocks(0, toExclusive);
			}
			return result;
		}

		private ReadOnlyCollection<TValue> GetValues()
		{
			int toExclusive = 0;
			ReadOnlyCollection<TValue> result;
			try
			{
				this.AcquireAllLocks(ref toExclusive);
				int countInternal = this.GetCountInternal();
				if (countInternal < 0)
				{
					throw new OutOfMemoryException();
				}
				List<TValue> list = new List<TValue>(countInternal);
				for (int i = 0; i < this.m_tables.m_buckets.Length; i++)
				{
					for (ConcurrentDictionary<TKey, TValue>.Node node = this.m_tables.m_buckets[i]; node != null; node = node.m_next)
					{
						list.Add(node.m_value);
					}
				}
				result = new ReadOnlyCollection<TValue>(list);
			}
			finally
			{
				this.ReleaseLocks(0, toExclusive);
			}
			return result;
		}

		[Conditional("DEBUG")]
		private void Assert(bool condition)
		{
		}

		private string GetResource(string key)
		{
			return Environment.GetResourceString(key);
		}

		[OnSerializing]
		private void OnSerializing(StreamingContext context)
		{
			ConcurrentDictionary<TKey, TValue>.Tables tables = this.m_tables;
			this.m_serializationArray = this.ToArray();
			this.m_serializationConcurrencyLevel = tables.m_locks.Length;
			this.m_serializationCapacity = tables.m_buckets.Length;
			this.m_comparer = (IEqualityComparer<TKey>)HashHelpers.GetEqualityComparerForSerialization(tables.m_comparer);
		}

		[OnDeserialized]
		private void OnDeserialized(StreamingContext context)
		{
			KeyValuePair<TKey, TValue>[] serializationArray = this.m_serializationArray;
			ConcurrentDictionary<TKey, TValue>.Node[] buckets = new ConcurrentDictionary<TKey, TValue>.Node[this.m_serializationCapacity];
			int[] countPerLock = new int[this.m_serializationConcurrencyLevel];
			object[] array = new object[this.m_serializationConcurrencyLevel];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = new object();
			}
			this.m_tables = new ConcurrentDictionary<TKey, TValue>.Tables(buckets, array, countPerLock, this.m_comparer);
			this.InitializeFromCollection(serializationArray);
			this.m_serializationArray = null;
		}

		[NonSerialized]
		private volatile ConcurrentDictionary<TKey, TValue>.Tables m_tables;

		internal IEqualityComparer<TKey> m_comparer;

		[NonSerialized]
		private readonly bool m_growLockArray;

		[OptionalField]
		private int m_keyRehashCount;

		[NonSerialized]
		private int m_budget;

		private KeyValuePair<TKey, TValue>[] m_serializationArray;

		private int m_serializationConcurrencyLevel;

		private int m_serializationCapacity;

		private const int DEFAULT_CAPACITY = 31;

		private const int MAX_LOCK_NUMBER = 1024;

		private static readonly bool s_isValueWriteAtomic = ConcurrentDictionary<TKey, TValue>.IsValueWriteAtomic();

		private class Tables
		{
			internal Tables(ConcurrentDictionary<TKey, TValue>.Node[] buckets, object[] locks, int[] countPerLock, IEqualityComparer<TKey> comparer)
			{
				this.m_buckets = buckets;
				this.m_locks = locks;
				this.m_countPerLock = countPerLock;
				this.m_comparer = comparer;
			}

			internal readonly ConcurrentDictionary<TKey, TValue>.Node[] m_buckets;

			internal readonly object[] m_locks;

			internal volatile int[] m_countPerLock;

			internal readonly IEqualityComparer<TKey> m_comparer;
		}

		private class Node
		{
			internal Node(TKey key, TValue value, int hashcode, ConcurrentDictionary<TKey, TValue>.Node next)
			{
				this.m_key = key;
				this.m_value = value;
				this.m_next = next;
				this.m_hashcode = hashcode;
			}

			internal TKey m_key;

			internal TValue m_value;

			internal volatile ConcurrentDictionary<TKey, TValue>.Node m_next;

			internal int m_hashcode;
		}

		private class DictionaryEnumerator : IDictionaryEnumerator, IEnumerator
		{
			internal DictionaryEnumerator(ConcurrentDictionary<TKey, TValue> dictionary)
			{
				this.m_enumerator = dictionary.GetEnumerator();
			}

			public DictionaryEntry Entry
			{
				get
				{
					KeyValuePair<TKey, TValue> keyValuePair = this.m_enumerator.Current;
					object key = keyValuePair.Key;
					keyValuePair = this.m_enumerator.Current;
					return new DictionaryEntry(key, keyValuePair.Value);
				}
			}

			public object Key
			{
				get
				{
					KeyValuePair<TKey, TValue> keyValuePair = this.m_enumerator.Current;
					return keyValuePair.Key;
				}
			}

			public object Value
			{
				get
				{
					KeyValuePair<TKey, TValue> keyValuePair = this.m_enumerator.Current;
					return keyValuePair.Value;
				}
			}

			public object Current
			{
				get
				{
					return this.Entry;
				}
			}

			public bool MoveNext()
			{
				return this.m_enumerator.MoveNext();
			}

			public void Reset()
			{
				this.m_enumerator.Reset();
			}

			private IEnumerator<KeyValuePair<TKey, TValue>> m_enumerator;
		}
	}
}
