using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class ReadOnlyDictionary<TKey, TValue> : IDictionary<TKey, TValue>, ICollection<KeyValuePair<TKey, TValue>>, IEnumerable<KeyValuePair<TKey, TValue>>, IEnumerable
	{
		public ReadOnlyDictionary()
		{
			this.wrappedDictionary = new Dictionary<TKey, TValue>();
		}

		public ReadOnlyDictionary(IDictionary<TKey, TValue> dictionary)
		{
			if (dictionary == null)
			{
				throw new ArgumentNullException("dictionary");
			}
			this.wrappedDictionary = dictionary;
		}

		public IDictionary<TKey, TValue> WrappedDictionary
		{
			get
			{
				return this.wrappedDictionary;
			}
		}

		public int Count
		{
			get
			{
				return this.wrappedDictionary.Count;
			}
		}

		public bool IsReadOnly
		{
			get
			{
				return this.wrappedDictionary.IsReadOnly;
			}
		}

		public ICollection<TKey> Keys
		{
			get
			{
				return this.wrappedDictionary.Keys;
			}
		}

		public ICollection<TValue> Values
		{
			get
			{
				return this.wrappedDictionary.Values;
			}
		}

		public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
		{
			return this.wrappedDictionary.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		public void Add(KeyValuePair<TKey, TValue> item)
		{
			throw this.CreateReadOnlyException(item);
		}

		public void Clear()
		{
			throw this.CreateReadOnlyException();
		}

		public bool Contains(KeyValuePair<TKey, TValue> item)
		{
			return this.wrappedDictionary.Contains(item);
		}

		public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
		{
			this.wrappedDictionary.CopyTo(array, arrayIndex);
		}

		public bool Remove(KeyValuePair<TKey, TValue> item)
		{
			throw this.CreateReadOnlyException(item);
		}

		public bool ContainsKey(TKey key)
		{
			return this.wrappedDictionary.ContainsKey(key);
		}

		public void Add(TKey key, TValue value)
		{
			throw this.CreateReadOnlyException(new KeyValuePair<TKey, TValue>(key, value));
		}

		public bool Remove(TKey key)
		{
			throw this.CreateReadOnlyException(key);
		}

		public bool TryGetValue(TKey key, out TValue value)
		{
			return this.wrappedDictionary.TryGetValue(key, out value);
		}

		public TValue this[TKey key]
		{
			get
			{
				return this.wrappedDictionary[key];
			}
			set
			{
				throw this.CreateReadOnlyException(new KeyValuePair<TKey, TValue>(key, value));
			}
		}

		private Exception CreateReadOnlyException()
		{
			throw new InvalidOperationException("Modification attempt on a read-only dictionary");
		}

		private Exception CreateReadOnlyException(TKey key)
		{
			throw new InvalidOperationException(string.Format("Modification attempt on a read-only dictionary for key: {0},", key));
		}

		private Exception CreateReadOnlyException(KeyValuePair<TKey, TValue> item)
		{
			throw new InvalidOperationException(string.Format("Modification attempt on a read-only dictionary for item: {0}", item));
		}

		private readonly IDictionary<TKey, TValue> wrappedDictionary;
	}
}
