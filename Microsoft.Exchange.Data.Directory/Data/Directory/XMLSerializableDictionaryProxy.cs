using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Data.Directory
{
	[Serializable]
	public class XMLSerializableDictionaryProxy<T, TKey, TValue> : XMLSerializableBase where T : IDictionary<TKey, TValue>, new() where TValue : class
	{
		public XMLSerializableDictionaryProxy()
		{
			this.Dictionary = ((default(T) == null) ? Activator.CreateInstance<T>() : default(T));
		}

		public XMLSerializableDictionaryProxy(T dictionary)
		{
			this.Dictionary = dictionary;
		}

		public static implicit operator T(XMLSerializableDictionaryProxy<T, TKey, TValue> dictionaryProxy)
		{
			return dictionaryProxy.Dictionary;
		}

		[XmlIgnore]
		internal T Dictionary { get; private set; }

		[XmlIgnore]
		internal int Count
		{
			get
			{
				T dictionary = this.Dictionary;
				return dictionary.Count;
			}
		}

		[XmlIgnore]
		internal bool IsReadOnly
		{
			get
			{
				T dictionary = this.Dictionary;
				return dictionary.IsReadOnly;
			}
		}

		[XmlIgnore]
		internal ICollection<TKey> Keys
		{
			get
			{
				T dictionary = this.Dictionary;
				return dictionary.Keys;
			}
		}

		[XmlIgnore]
		internal ICollection<TValue> Values
		{
			get
			{
				T dictionary = this.Dictionary;
				return dictionary.Values;
			}
		}

		[XmlIgnore]
		internal TValue this[TKey key]
		{
			get
			{
				T dictionary = this.Dictionary;
				return dictionary[key];
			}
			set
			{
				T dictionary = this.Dictionary;
				dictionary[key] = value;
			}
		}

		[XmlArray(ElementName = "Pairs")]
		[XmlArrayItem(ElementName = "Pair")]
		public XMLSerializableDictionaryProxy<T, TKey, TValue>.InternalKeyValuePair[] Pairs
		{
			get
			{
				List<XMLSerializableDictionaryProxy<T, TKey, TValue>.InternalKeyValuePair> list = new List<XMLSerializableDictionaryProxy<T, TKey, TValue>.InternalKeyValuePair>(this.Count);
				foreach (KeyValuePair<TKey, TValue> keyValuePair in this)
				{
					list.Add(this.CreateKeyValuePair(keyValuePair.Key, keyValuePair.Value));
				}
				return list.ToArray();
			}
			set
			{
				this.Clear();
				if (value != null)
				{
					for (int i = 0; i < value.Length; i++)
					{
						XMLSerializableDictionaryProxy<T, TKey, TValue>.InternalKeyValuePair internalKeyValuePair = value[i];
						this.Add(internalKeyValuePair.Key, internalKeyValuePair.Value);
					}
				}
			}
		}

		protected virtual XMLSerializableDictionaryProxy<T, TKey, TValue>.InternalKeyValuePair CreateKeyValuePair(TKey key, TValue value)
		{
			return new XMLSerializableDictionaryProxy<T, TKey, TValue>.InternalKeyValuePair(key, value, int.MaxValue);
		}

		public void Add(KeyValuePair<TKey, TValue> item)
		{
			T dictionary = this.Dictionary;
			dictionary.Add(item);
		}

		public void Add(TKey key, TValue value)
		{
			T dictionary = this.Dictionary;
			dictionary.Add(key, value);
		}

		public void Clear()
		{
			T dictionary = this.Dictionary;
			dictionary.Clear();
		}

		public bool Contains(KeyValuePair<TKey, TValue> item)
		{
			T dictionary = this.Dictionary;
			return dictionary.Contains(item);
		}

		public bool ContainsKey(TKey key)
		{
			T dictionary = this.Dictionary;
			return dictionary.ContainsKey(key);
		}

		public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
		{
			T dictionary = this.Dictionary;
			dictionary.CopyTo(array, arrayIndex);
		}

		public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
		{
			T dictionary = this.Dictionary;
			return dictionary.GetEnumerator();
		}

		public bool Remove(KeyValuePair<TKey, TValue> item)
		{
			T dictionary = this.Dictionary;
			return dictionary.Remove(item);
		}

		public bool Remove(TKey key)
		{
			T dictionary = this.Dictionary;
			return dictionary.Remove(key);
		}

		public bool TryGetValue(TKey key, out TValue value)
		{
			T dictionary = this.Dictionary;
			return dictionary.TryGetValue(key, out value);
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			foreach (KeyValuePair<TKey, TValue> keyValuePair in this)
			{
				stringBuilder.AppendFormat("{0}{1}={2}", (stringBuilder.Length > 0) ? "," : "", keyValuePair.Key, keyValuePair.Value);
			}
			return stringBuilder.ToString();
		}

		[Serializable]
		public class InternalKeyValuePair : XMLSerializableBase
		{
			public InternalKeyValuePair()
			{
			}

			public InternalKeyValuePair(TKey key, TValue value, int compressionThreshold = 2147483647)
			{
				this.compressionThreshold = compressionThreshold;
				this.Key = key;
				this.Value = value;
			}

			[XmlElement("K")]
			public TKey Key { get; set; }

			[XmlElement("V")]
			public virtual TValue UncompressedValue { get; set; }

			[XmlElement("CV")]
			public XMLSerializableCompressed<TValue> CompressedValue { get; set; }

			[XmlIgnore]
			public TValue Value
			{
				get
				{
					if (this.CompressedValue != null)
					{
						return this.CompressedValue.Value;
					}
					return this.UncompressedValue;
				}
				set
				{
					if (value == null)
					{
						this.UncompressedValue = default(TValue);
						this.CompressedValue = null;
						return;
					}
					string text = value as string;
					if (text != null && text.Length > this.compressionThreshold)
					{
						if (this.CompressedValue == null)
						{
							this.CompressedValue = new XMLSerializableCompressed<TValue>();
						}
						this.CompressedValue.Value = value;
						this.UncompressedValue = default(TValue);
						return;
					}
					this.UncompressedValue = value;
					this.CompressedValue = null;
				}
			}

			public override string ToString()
			{
				return string.Format("{0}={1}", this.Key, this.Value);
			}

			private readonly int compressionThreshold;
		}
	}
}
