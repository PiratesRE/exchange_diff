using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

namespace Microsoft.Exchange.Server.Storage.Common
{
	public sealed class LockFreeDictionary<TKey, TValue> : IDictionary<TKey, TValue>, ICollection<KeyValuePair<TKey, TValue>>, IEnumerable<KeyValuePair<TKey, TValue>>, IEnumerable
	{
		public LockFreeDictionary() : this(null)
		{
		}

		public LockFreeDictionary(IEnumerable<KeyValuePair<TKey, TValue>> initialValues)
		{
			this.dictionary = new Dictionary<TKey, TValue>();
			if (initialValues != null)
			{
				foreach (KeyValuePair<TKey, TValue> keyValuePair in initialValues)
				{
					this.dictionary[keyValuePair.Key] = keyValuePair.Value;
				}
			}
		}

		public TValue this[TKey key]
		{
			get
			{
				return this.dictionary[key];
			}
			set
			{
				throw new NotSupportedException();
			}
		}

		public ICollection<TKey> Keys
		{
			get
			{
				return this.dictionary.Keys;
			}
		}

		public ICollection<TValue> Values
		{
			get
			{
				return this.dictionary.Values;
			}
		}

		public bool ContainsKey(TKey key)
		{
			return this.dictionary.ContainsKey(key);
		}

		public void Add(TKey key, TValue value)
		{
			Dictionary<TKey, TValue> dictionary;
			Dictionary<TKey, TValue> dictionary2;
			do
			{
				dictionary = this.dictionary;
				dictionary2 = new Dictionary<TKey, TValue>(dictionary.Count + 1);
				foreach (KeyValuePair<TKey, TValue> keyValuePair in dictionary)
				{
					dictionary2.Add(keyValuePair.Key, keyValuePair.Value);
				}
				dictionary2[key] = value;
			}
			while (Interlocked.CompareExchange<Dictionary<TKey, TValue>>(ref this.dictionary, dictionary2, dictionary) != dictionary);
		}

		public bool TryAdd(TKey key, TValue value)
		{
			if (this.dictionary.ContainsKey(key))
			{
				return false;
			}
			for (;;)
			{
				Dictionary<TKey, TValue> dictionary = this.dictionary;
				Dictionary<TKey, TValue> dictionary2 = new Dictionary<TKey, TValue>(dictionary.Count + 1);
				foreach (KeyValuePair<TKey, TValue> keyValuePair in dictionary)
				{
					dictionary2.Add(keyValuePair.Key, keyValuePair.Value);
				}
				if (dictionary2.ContainsKey(key))
				{
					break;
				}
				dictionary2.Add(key, value);
				if (Interlocked.CompareExchange<Dictionary<TKey, TValue>>(ref this.dictionary, dictionary2, dictionary) == dictionary)
				{
					return true;
				}
			}
			return false;
		}

		public bool Remove(TKey key)
		{
			if (this.dictionary.ContainsKey(key))
			{
				Dictionary<TKey, TValue> dictionary;
				Dictionary<TKey, TValue> dictionary2;
				do
				{
					dictionary = this.dictionary;
					dictionary2 = new Dictionary<TKey, TValue>(dictionary.Count);
					bool flag = false;
					foreach (KeyValuePair<TKey, TValue> keyValuePair in dictionary)
					{
						TKey key2 = keyValuePair.Key;
						if (key2.Equals(key))
						{
							flag = true;
						}
						else
						{
							dictionary2.Add(keyValuePair.Key, keyValuePair.Value);
						}
					}
					if (!flag)
					{
						return false;
					}
				}
				while (Interlocked.CompareExchange<Dictionary<TKey, TValue>>(ref this.dictionary, dictionary2, dictionary) != dictionary);
				return true;
			}
			return false;
		}

		public bool TryGetValue(TKey key, out TValue value)
		{
			return this.dictionary.TryGetValue(key, out value);
		}

		public int Count
		{
			get
			{
				return this.dictionary.Count;
			}
		}

		public bool IsReadOnly
		{
			get
			{
				return false;
			}
		}

		public void Add(KeyValuePair<TKey, TValue> item)
		{
			throw new NotSupportedException();
		}

		public void Clear()
		{
			throw new NotSupportedException();
		}

		public bool Contains(KeyValuePair<TKey, TValue> item)
		{
			return ((ICollection<KeyValuePair<TKey, TValue>>)this.dictionary).Contains(item);
		}

		public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
		{
			((ICollection<KeyValuePair<TKey, TValue>>)this.dictionary).CopyTo(array, arrayIndex);
		}

		public bool Remove(KeyValuePair<TKey, TValue> item)
		{
			throw new NotSupportedException();
		}

		public Dictionary<TKey, TValue>.Enumerator GetEnumerator()
		{
			return this.dictionary.GetEnumerator();
		}

		IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<!0, !1>>.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return ((IEnumerable)this.dictionary).GetEnumerator();
		}

		private Dictionary<TKey, TValue> dictionary;
	}
}
