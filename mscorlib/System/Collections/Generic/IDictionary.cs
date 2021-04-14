using System;

namespace System.Collections.Generic
{
	[__DynamicallyInvokable]
	public interface IDictionary<TKey, TValue> : ICollection<KeyValuePair<!0, !1>>, IEnumerable<KeyValuePair<!0, !1>>, IEnumerable
	{
		[__DynamicallyInvokable]
		TValue this[TKey key]
		{
			[__DynamicallyInvokable]
			get;
			[__DynamicallyInvokable]
			set;
		}

		[__DynamicallyInvokable]
		ICollection<TKey> Keys { [__DynamicallyInvokable] get; }

		[__DynamicallyInvokable]
		ICollection<TValue> Values { [__DynamicallyInvokable] get; }

		[__DynamicallyInvokable]
		bool ContainsKey(TKey key);

		[__DynamicallyInvokable]
		void Add(TKey key, TValue value);

		[__DynamicallyInvokable]
		bool Remove(TKey key);

		[__DynamicallyInvokable]
		bool TryGetValue(TKey key, out TValue value);
	}
}
