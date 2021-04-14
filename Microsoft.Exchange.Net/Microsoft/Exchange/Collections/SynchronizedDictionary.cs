using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Exchange.Threading;

namespace Microsoft.Exchange.Collections
{
	internal sealed class SynchronizedDictionary<K, V> : IDictionary<K, V>, ICollection<KeyValuePair<K, V>>, IEnumerable<KeyValuePair<K, V>>, IEnumerable
	{
		public SynchronizedDictionary() : this(0, null)
		{
		}

		public SynchronizedDictionary(int capacity) : this(capacity, null)
		{
		}

		public SynchronizedDictionary(IEqualityComparer<K> comparer) : this(0, comparer)
		{
		}

		public SynchronizedDictionary(int capacity, IEqualityComparer<K> comparer)
		{
			this.dictionary = new Dictionary<K, V>(capacity, comparer);
			this.readerWriterLock = new FastReaderWriterLock();
		}

		public int Count
		{
			get
			{
				int count;
				try
				{
					this.readerWriterLock.AcquireReaderLock(-1);
					count = this.dictionary.Count;
				}
				finally
				{
					this.readerWriterLock.ReleaseReaderLock();
				}
				return count;
			}
		}

		public ICollection<K> Keys
		{
			get
			{
				return this.dictionary.Keys;
			}
		}

		public ICollection<V> Values
		{
			get
			{
				return this.dictionary.Values;
			}
		}

		public V this[K key]
		{
			get
			{
				V result;
				try
				{
					this.readerWriterLock.AcquireReaderLock(-1);
					result = this.dictionary[key];
				}
				finally
				{
					this.readerWriterLock.ReleaseReaderLock();
				}
				return result;
			}
			set
			{
				try
				{
					this.readerWriterLock.AcquireWriterLock(-1);
					this.dictionary[key] = value;
				}
				finally
				{
					this.readerWriterLock.ReleaseWriterLock();
				}
			}
		}

		public void Add(K key, V value)
		{
			try
			{
				this.readerWriterLock.AcquireWriterLock(-1);
				this.dictionary.Add(key, value);
			}
			finally
			{
				this.readerWriterLock.ReleaseWriterLock();
			}
		}

		public V AddIfNotExists(K key, SynchronizedDictionary<K, V>.CreationalMethod creationalMethod)
		{
			V v;
			if (this.TryGetValue(key, out v))
			{
				return v;
			}
			V result;
			try
			{
				this.readerWriterLock.AcquireWriterLock(-1);
				if (this.dictionary.TryGetValue(key, out v))
				{
					result = v;
				}
				else
				{
					v = creationalMethod(key);
					this.dictionary[key] = v;
					result = v;
				}
			}
			finally
			{
				this.readerWriterLock.ReleaseWriterLock();
			}
			return result;
		}

		public void Clear()
		{
			try
			{
				this.readerWriterLock.AcquireWriterLock(-1);
				this.dictionary.Clear();
			}
			finally
			{
				this.readerWriterLock.ReleaseWriterLock();
			}
		}

		public bool ContainsKey(K key)
		{
			bool result;
			try
			{
				this.readerWriterLock.AcquireReaderLock(-1);
				result = this.dictionary.ContainsKey(key);
			}
			finally
			{
				this.readerWriterLock.ReleaseReaderLock();
			}
			return result;
		}

		public IEnumerator<KeyValuePair<K, V>> GetEnumerator()
		{
			return this.dictionary.GetEnumerator();
		}

		public bool Remove(K key)
		{
			bool result;
			try
			{
				this.readerWriterLock.AcquireWriterLock(-1);
				result = this.dictionary.Remove(key);
			}
			finally
			{
				this.readerWriterLock.ReleaseWriterLock();
			}
			return result;
		}

		public bool TryGetValue(K key, out V value)
		{
			bool result;
			try
			{
				this.readerWriterLock.AcquireReaderLock(-1);
				result = this.dictionary.TryGetValue(key, out value);
			}
			finally
			{
				this.readerWriterLock.ReleaseReaderLock();
			}
			return result;
		}

		public void ForEach(Predicate<V> predicate, Action<K, V> action)
		{
			try
			{
				this.readerWriterLock.AcquireReaderLock(-1);
				foreach (KeyValuePair<K, V> keyValuePair in this.dictionary)
				{
					if (predicate == null || predicate(keyValuePair.Value))
					{
						action(keyValuePair.Key, keyValuePair.Value);
					}
				}
			}
			finally
			{
				this.readerWriterLock.ReleaseReaderLock();
			}
		}

		public void ForKey(K key, Action<K, V> action)
		{
			try
			{
				this.readerWriterLock.AcquireReaderLock(-1);
				V arg;
				if (this.dictionary.TryGetValue(key, out arg))
				{
					action(key, arg);
				}
			}
			finally
			{
				this.readerWriterLock.ReleaseReaderLock();
			}
		}

		public void ForEachKey(IEnumerable<K> keys, V defaultValue, Action<K, V> action)
		{
			try
			{
				this.readerWriterLock.AcquireReaderLock(-1);
				foreach (K k in keys)
				{
					V arg;
					if (!this.dictionary.TryGetValue(k, out arg))
					{
						arg = defaultValue;
					}
					action(k, arg);
				}
			}
			finally
			{
				this.readerWriterLock.ReleaseReaderLock();
			}
		}

		public void RemoveAll(Predicate<V> predicate)
		{
			List<K> keysToRemove = new List<K>(this.Count);
			this.ForEach(predicate, delegate(K key, V value)
			{
				keysToRemove.Add(key);
			});
			if (keysToRemove.Count != 0)
			{
				try
				{
					this.readerWriterLock.AcquireWriterLock(-1);
					foreach (K key2 in keysToRemove)
					{
						V obj;
						if (this.dictionary.TryGetValue(key2, out obj) && (predicate == null || predicate(obj)))
						{
							this.dictionary.Remove(key2);
						}
					}
				}
				finally
				{
					this.readerWriterLock.ReleaseWriterLock();
				}
			}
		}

		void ICollection<KeyValuePair<!0, !1>>.Add(KeyValuePair<K, V> item)
		{
			try
			{
				this.readerWriterLock.AcquireWriterLock(-1);
				((ICollection<KeyValuePair<!0, !1>>)this.dictionary).Add(item);
			}
			finally
			{
				this.readerWriterLock.ReleaseWriterLock();
			}
		}

		bool ICollection<KeyValuePair<!0, !1>>.Contains(KeyValuePair<K, V> item)
		{
			bool result;
			try
			{
				this.readerWriterLock.AcquireReaderLock(-1);
				result = ((ICollection<KeyValuePair<!0, !1>>)this.dictionary).Contains(item);
			}
			finally
			{
				this.readerWriterLock.ReleaseReaderLock();
			}
			return result;
		}

		void ICollection<KeyValuePair<!0, !1>>.CopyTo(KeyValuePair<K, V>[] array, int arrayIndex)
		{
			try
			{
				this.readerWriterLock.AcquireReaderLock(-1);
				((ICollection<KeyValuePair<!0, !1>>)this.dictionary).CopyTo(array, arrayIndex);
			}
			finally
			{
				this.readerWriterLock.ReleaseReaderLock();
			}
		}

		bool ICollection<KeyValuePair<!0, !1>>.IsReadOnly
		{
			get
			{
				return ((ICollection<KeyValuePair<K, V>>)this.dictionary).IsReadOnly;
			}
		}

		bool ICollection<KeyValuePair<!0, !1>>.Remove(KeyValuePair<K, V> item)
		{
			bool result;
			try
			{
				this.readerWriterLock.AcquireWriterLock(-1);
				result = ((ICollection<KeyValuePair<!0, !1>>)this.dictionary).Remove(item);
			}
			finally
			{
				this.readerWriterLock.ReleaseWriterLock();
			}
			return result;
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.dictionary.GetEnumerator();
		}

		private Dictionary<K, V> dictionary;

		private FastReaderWriterLock readerWriterLock;

		internal delegate V CreationalMethod(K key);
	}
}
