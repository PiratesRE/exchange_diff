using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[Serializable]
	public class JsonDictionary<T> : ISerializable
	{
		protected JsonDictionary(SerializationInfo info, StreamingContext context)
		{
			this.rawDictionary = new Dictionary<string, T>(info.MemberCount);
			SerializationInfoEnumerator enumerator = info.GetEnumerator();
			while (enumerator.MoveNext())
			{
				try
				{
					this.rawDictionary[enumerator.Name] = (T)((object)enumerator.Value);
				}
				catch (InvalidCastException)
				{
					if (typeof(T).IsArray && enumerator.Value is object[])
					{
						this.rawDictionary[enumerator.Name] = (T)((object)Activator.CreateInstance(typeof(T), new object[]
						{
							((object[])enumerator.Value).Length
						}));
						Array.Copy((object[])enumerator.Value, this.rawDictionary[enumerator.Name] as Array, ((object[])enumerator.Value).Length);
					}
				}
			}
		}

		public JsonDictionary(Dictionary<string, T> dictionary)
		{
			this.rawDictionary = dictionary;
		}

		internal Dictionary<string, T> RawDictionary
		{
			get
			{
				return this.rawDictionary;
			}
		}

		public static implicit operator JsonDictionary<T>(Dictionary<string, T> dictionary)
		{
			return new JsonDictionary<T>(dictionary);
		}

		public static implicit operator Dictionary<string, T>(JsonDictionary<T> dictionary)
		{
			return dictionary.rawDictionary;
		}

		public void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			foreach (KeyValuePair<string, T> keyValuePair in this.rawDictionary)
			{
				info.AddValue(keyValuePair.Key, keyValuePair.Value, typeof(T));
			}
		}

		public JsonDictionary<T> Merge(JsonDictionary<T> from)
		{
			if (from != null)
			{
				foreach (KeyValuePair<string, T> keyValuePair in from.rawDictionary)
				{
					this.rawDictionary.Add(keyValuePair.Key, keyValuePair.Value);
				}
			}
			return this;
		}

		private Dictionary<string, T> rawDictionary;
	}
}
