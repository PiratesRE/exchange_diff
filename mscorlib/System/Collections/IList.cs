using System;
using System.Runtime.InteropServices;

namespace System.Collections
{
	[ComVisible(true)]
	[__DynamicallyInvokable]
	public interface IList : ICollection, IEnumerable
	{
		[__DynamicallyInvokable]
		object this[int index]
		{
			[__DynamicallyInvokable]
			get;
			[__DynamicallyInvokable]
			set;
		}

		[__DynamicallyInvokable]
		int Add(object value);

		[__DynamicallyInvokable]
		bool Contains(object value);

		[__DynamicallyInvokable]
		void Clear();

		[__DynamicallyInvokable]
		bool IsReadOnly { [__DynamicallyInvokable] get; }

		[__DynamicallyInvokable]
		bool IsFixedSize { [__DynamicallyInvokable] get; }

		[__DynamicallyInvokable]
		int IndexOf(object value);

		[__DynamicallyInvokable]
		void Insert(int index, object value);

		[__DynamicallyInvokable]
		void Remove(object value);

		[__DynamicallyInvokable]
		void RemoveAt(int index);
	}
}
