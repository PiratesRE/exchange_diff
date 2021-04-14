using System;
using System.Collections;
using System.Collections.Generic;

namespace Microsoft.Exchange.Hygiene.Data.Configuration
{
	internal sealed class AppConfigCollection : IDictionary<string, string>, ICollection<KeyValuePair<string, string>>, IEnumerable<KeyValuePair<string, string>>, IEnumerable
	{
		public AppConfigCollection()
		{
			this.dictionary = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
		}

		public int Count
		{
			get
			{
				return this.dictionary.Count;
			}
		}

		bool ICollection<KeyValuePair<string, string>>.IsReadOnly
		{
			get
			{
				return false;
			}
		}

		public ICollection<string> Keys
		{
			get
			{
				return this.dictionary.Keys;
			}
		}

		public ICollection<string> Values
		{
			get
			{
				return this.dictionary.Values;
			}
		}

		public string this[string key]
		{
			get
			{
				if (key == null)
				{
					throw new ArgumentNullException("key");
				}
				if (key.Length > 255)
				{
					throw new ArgumentOutOfRangeException("key");
				}
				return this.dictionary[key];
			}
			set
			{
				if (key == null)
				{
					throw new ArgumentNullException("key");
				}
				if (key.Length > 255)
				{
					throw new ArgumentOutOfRangeException("key");
				}
				this.dictionary[key] = value;
			}
		}

		public void Add(string key, string value)
		{
			if (key == null)
			{
				throw new ArgumentNullException("key");
			}
			if (key.Length > 255)
			{
				throw new ArgumentOutOfRangeException("key");
			}
			this.dictionary.Add(key, value);
		}

		public bool Remove(string key)
		{
			if (key == null)
			{
				throw new ArgumentNullException("key");
			}
			if (key.Length > 255)
			{
				throw new ArgumentOutOfRangeException("key");
			}
			return this.dictionary.Remove(key);
		}

		public void Clear()
		{
			this.dictionary.Clear();
		}

		public bool ContainsKey(string key)
		{
			if (key == null)
			{
				throw new ArgumentNullException("key");
			}
			if (key.Length > 255)
			{
				throw new ArgumentOutOfRangeException("key");
			}
			return this.dictionary.ContainsKey(key);
		}

		public bool TryGetValue(string key, out string value)
		{
			if (key == null)
			{
				throw new ArgumentNullException("key");
			}
			if (key.Length > 255)
			{
				throw new ArgumentOutOfRangeException("key");
			}
			return this.dictionary.TryGetValue(key, out value);
		}

		void ICollection<KeyValuePair<string, string>>.Add(KeyValuePair<string, string> item)
		{
			this.Add(item.Key, item.Value);
		}

		bool ICollection<KeyValuePair<string, string>>.Contains(KeyValuePair<string, string> item)
		{
			if (item.Key == null)
			{
				throw new ArgumentNullException("key");
			}
			if (item.Key.Length > 255)
			{
				throw new ArgumentOutOfRangeException("key");
			}
			return ((ICollection<KeyValuePair<string, string>>)this.dictionary).Contains(item);
		}

		void ICollection<KeyValuePair<string, string>>.CopyTo(KeyValuePair<string, string>[] array, int arrayIndex)
		{
			((ICollection<KeyValuePair<string, string>>)this.dictionary).CopyTo(array, arrayIndex);
		}

		bool ICollection<KeyValuePair<string, string>>.Remove(KeyValuePair<string, string> item)
		{
			if (item.Key == null)
			{
				throw new ArgumentNullException("key");
			}
			if (item.Key.Length > 255)
			{
				throw new ArgumentOutOfRangeException("key");
			}
			return ((ICollection<KeyValuePair<string, string>>)this.dictionary).Remove(item);
		}

		IEnumerator<KeyValuePair<string, string>> IEnumerable<KeyValuePair<string, string>>.GetEnumerator()
		{
			return this.dictionary.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.dictionary.GetEnumerator();
		}

		private Dictionary<string, string> dictionary;
	}
}
