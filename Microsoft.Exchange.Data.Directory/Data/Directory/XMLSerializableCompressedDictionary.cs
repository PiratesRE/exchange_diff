using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.Data.Directory
{
	[Serializable]
	public class XMLSerializableCompressedDictionary<TKey, TValue> : XMLSerializableDictionary<TKey, TValue> where TValue : class
	{
		protected override XMLSerializableDictionaryProxy<Dictionary<TKey, TValue>, TKey, TValue>.InternalKeyValuePair CreateKeyValuePair(TKey key, TValue value)
		{
			return new XMLSerializableDictionaryProxy<Dictionary<TKey, TValue>, TKey, TValue>.InternalKeyValuePair(key, value, 1024);
		}
	}
}
