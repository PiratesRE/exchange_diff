using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Collections
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class MruDictionary<TKey, TValue> : IEnumerable<KeyValuePair<TKey, TValue>>, IEnumerable
	{
		public event EventHandler<MruDictionaryElementRemovedEventArgs<TKey, TValue>> OnRemoved;

		public event EventHandler<MruDictionaryElementReplacedEventArgs<TKey, TValue>> OnReplaced;

		public MruDictionary(int maxCapacity, IComparer<TKey> keyComparer, IMruDictionaryPerfCounters perfCounters)
		{
			if (maxCapacity < 1)
			{
				throw new ArgumentOutOfRangeException("maxCapacity", maxCapacity, "maxCapacity must be greater than 0!");
			}
			if (keyComparer == null)
			{
				throw new ArgumentNullException("keyComparer");
			}
			this.maxCapacity = maxCapacity;
			this.actualKeyComparer = keyComparer;
			this.dictionaryTime = new Dictionary<TKey, DateTime>(new MruEqualityComparer<TKey>(this.actualKeyComparer));
			this.perfCounters = (perfCounters ?? NoopMruDictionaryPerfCounters.Instance);
		}

		public int Count
		{
			get
			{
				int count;
				lock (this.SyncRoot)
				{
					count = this.dictionaryTime.Count;
				}
				return count;
			}
		}

		public bool TryGetValue(TKey key, out TValue value)
		{
			value = default(TValue);
			if (key == null)
			{
				return false;
			}
			bool flag = false;
			lock (this.SyncRoot)
			{
				DateTime lastAccessedTime;
				if (this.dictionaryTime.TryGetValue(key, out lastAccessedTime))
				{
					MruDictionaryInternalKey<TKey> key2 = new MruDictionaryInternalKey<TKey>(key, this.actualKeyComparer, lastAccessedTime);
					value = this.dictionaryData[key2];
					MruDictionaryInternalKey<TKey> mruDictionaryInternalKey = new MruDictionaryInternalKey<TKey>(key, this.actualKeyComparer, DateTime.UtcNow);
					this.dictionaryData.Remove(key2);
					this.dictionaryData.Add(mruDictionaryInternalKey, value);
					this.dictionaryTime[key] = mruDictionaryInternalKey.LastAccessedTime;
					flag = true;
				}
			}
			if (flag)
			{
				this.perfCounters.CacheHit();
			}
			else
			{
				this.perfCounters.CacheMiss();
			}
			return flag;
		}

		public void Add(TKey key, TValue value)
		{
			if (key == null)
			{
				throw new ArgumentNullException("key");
			}
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			TKey tkey = default(TKey);
			TValue tvalue = default(TValue);
			bool flag = false;
			bool flag2 = false;
			lock (this.SyncRoot)
			{
				DateTime lastAccessedTime;
				if (this.dictionaryTime.TryGetValue(key, out lastAccessedTime))
				{
					MruDictionaryInternalKey<TKey> key2 = new MruDictionaryInternalKey<TKey>(key, this.actualKeyComparer, lastAccessedTime);
					tvalue = this.dictionaryData[key2];
					tkey = key;
					this.dictionaryData.Remove(key2);
					flag2 = true;
				}
				else if (this.dictionaryTime.Count >= this.maxCapacity)
				{
					KeyValuePair<MruDictionaryInternalKey<TKey>, TValue> keyValuePair = this.dictionaryData.First<KeyValuePair<MruDictionaryInternalKey<TKey>, TValue>>();
					tkey = keyValuePair.Key.OriginalKey;
					tvalue = keyValuePair.Value;
					this.dictionaryData.Remove(keyValuePair.Key);
					this.dictionaryTime.Remove(keyValuePair.Key.OriginalKey);
					flag = true;
				}
				MruDictionaryInternalKey<TKey> mruDictionaryInternalKey = new MruDictionaryInternalKey<TKey>(key, this.actualKeyComparer, DateTime.UtcNow);
				this.dictionaryData.Add(mruDictionaryInternalKey, value);
				this.dictionaryTime[key] = mruDictionaryInternalKey.LastAccessedTime;
			}
			this.perfCounters.CacheAdd(flag2, flag);
			if (flag)
			{
				this.InvokeOnRemovedEventHandler(tkey, tvalue);
				return;
			}
			if (flag2)
			{
				this.InvokeOnReplacedEventHandler(tkey, tvalue, key, value);
			}
		}

		public bool Remove(TKey key)
		{
			if (key == null)
			{
				throw new ArgumentNullException("key");
			}
			TValue value = default(TValue);
			bool flag = false;
			lock (this.SyncRoot)
			{
				DateTime lastAccessedTime;
				if (this.dictionaryTime.TryGetValue(key, out lastAccessedTime))
				{
					MruDictionaryInternalKey<TKey> key2 = new MruDictionaryInternalKey<TKey>(key, this.actualKeyComparer, lastAccessedTime);
					value = this.dictionaryData[key2];
					this.dictionaryData.Remove(key2);
					this.dictionaryTime.Remove(key);
					flag = true;
				}
			}
			if (flag)
			{
				this.perfCounters.CacheRemove();
				this.InvokeOnRemovedEventHandler(key, value);
			}
			return flag;
		}

		private void InvokeOnRemovedEventHandler(TKey key, TValue value)
		{
			EventHandler<MruDictionaryElementRemovedEventArgs<TKey, TValue>> onRemoved = this.OnRemoved;
			if (onRemoved != null)
			{
				KeyValuePair<TKey, TValue> keyValuePair = new KeyValuePair<TKey, TValue>(key, value);
				MruDictionaryElementRemovedEventArgs<TKey, TValue> e = new MruDictionaryElementRemovedEventArgs<TKey, TValue>(keyValuePair);
				onRemoved(this, e);
			}
		}

		private void InvokeOnReplacedEventHandler(TKey oldKey, TValue oldValue, TKey newKey, TValue newValue)
		{
			EventHandler<MruDictionaryElementReplacedEventArgs<TKey, TValue>> onReplaced = this.OnReplaced;
			if (onReplaced != null)
			{
				KeyValuePair<TKey, TValue> oldKeyValuePair = new KeyValuePair<TKey, TValue>(oldKey, oldValue);
				KeyValuePair<TKey, TValue> newKeyValuePair = new KeyValuePair<TKey, TValue>(newKey, newValue);
				MruDictionaryElementReplacedEventArgs<TKey, TValue> e = new MruDictionaryElementReplacedEventArgs<TKey, TValue>(oldKeyValuePair, newKeyValuePair);
				onReplaced(this, e);
			}
		}

		public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
		{
			return new MruDictionary<TKey, TValue>.Mlu2MruEnumerator(this);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		public readonly object SyncRoot = new object();

		private readonly int maxCapacity;

		private readonly SortedDictionary<MruDictionaryInternalKey<TKey>, TValue> dictionaryData = new SortedDictionary<MruDictionaryInternalKey<TKey>, TValue>();

		private readonly Dictionary<TKey, DateTime> dictionaryTime;

		private readonly IComparer<TKey> actualKeyComparer;

		private IMruDictionaryPerfCounters perfCounters;

		internal sealed class Mlu2MruEnumerator : IEnumerator<KeyValuePair<TKey, TValue>>, IDisposable, IEnumerator
		{
			internal Mlu2MruEnumerator(MruDictionary<TKey, TValue> dictionary)
			{
				this.internalEnumerator = dictionary.dictionaryData.GetEnumerator();
			}

			public KeyValuePair<TKey, TValue> Current
			{
				get
				{
					KeyValuePair<MruDictionaryInternalKey<TKey>, TValue> keyValuePair = this.internalEnumerator.Current;
					return new KeyValuePair<TKey, TValue>(keyValuePair.Key.OriginalKey, keyValuePair.Value);
				}
			}

			public void Dispose()
			{
				this.internalEnumerator.Dispose();
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
				return this.internalEnumerator.MoveNext();
			}

			public void Reset()
			{
				this.internalEnumerator.Reset();
			}

			private readonly IEnumerator<KeyValuePair<MruDictionaryInternalKey<TKey>, TValue>> internalEnumerator;
		}
	}
}
