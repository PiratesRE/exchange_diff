using System;

namespace Microsoft.Exchange.Collections
{
	internal delegate bool MruDictionarySerializerWrite<TKey, TValue>(TKey key, TValue value, out string[] values);
}
