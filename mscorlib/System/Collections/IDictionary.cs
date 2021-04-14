using System;
using System.Runtime.InteropServices;

namespace System.Collections
{
	[ComVisible(true)]
	[__DynamicallyInvokable]
	public interface IDictionary : ICollection, IEnumerable
	{
		[__DynamicallyInvokable]
		object this[object key]
		{
			[__DynamicallyInvokable]
			get;
			[__DynamicallyInvokable]
			set;
		}

		[__DynamicallyInvokable]
		ICollection Keys { [__DynamicallyInvokable] get; }

		[__DynamicallyInvokable]
		ICollection Values { [__DynamicallyInvokable] get; }

		[__DynamicallyInvokable]
		bool Contains(object key);

		[__DynamicallyInvokable]
		void Add(object key, object value);

		[__DynamicallyInvokable]
		void Clear();

		[__DynamicallyInvokable]
		bool IsReadOnly { [__DynamicallyInvokable] get; }

		[__DynamicallyInvokable]
		bool IsFixedSize { [__DynamicallyInvokable] get; }

		[__DynamicallyInvokable]
		IDictionaryEnumerator GetEnumerator();

		[__DynamicallyInvokable]
		void Remove(object key);
	}
}
