using System;

namespace System.Collections.Generic
{
	[__DynamicallyInvokable]
	public interface IReadOnlyDictionary<TKey, TValue> : IReadOnlyCollection<KeyValuePair<TKey, TValue>>, IEnumerable<KeyValuePair<!0, !1>>, IEnumerable
	{
		[__DynamicallyInvokable]
		bool ContainsKey(TKey key);

		[__DynamicallyInvokable]
		bool TryGetValue(TKey key, out TValue value);

		[__DynamicallyInvokable]
		TValue this[TKey key]
		{
			[__DynamicallyInvokable]
			get;
		}

		[__DynamicallyInvokable]
		IEnumerable<TKey> Keys { [__DynamicallyInvokable] get; }

		[__DynamicallyInvokable]
		IEnumerable<TValue> Values { [__DynamicallyInvokable] get; }
	}
}
