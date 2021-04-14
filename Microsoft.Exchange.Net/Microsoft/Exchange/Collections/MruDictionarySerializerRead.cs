using System;

namespace Microsoft.Exchange.Collections
{
	internal delegate bool MruDictionarySerializerRead<TKey, TValue>(string[] values, out TKey key, out TValue value);
}
