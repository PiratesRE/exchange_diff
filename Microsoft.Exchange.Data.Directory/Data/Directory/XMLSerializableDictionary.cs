using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.Data.Directory
{
	[Serializable]
	public class XMLSerializableDictionary<TKey, TValue> : XMLSerializableDictionaryProxy<Dictionary<TKey, TValue>, TKey, TValue> where TValue : class
	{
	}
}
