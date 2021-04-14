using System;
using System.Runtime.CompilerServices;

namespace System.Collections.Generic
{
	[TypeDependency("System.SZArrayHelper")]
	[__DynamicallyInvokable]
	public interface ICollection<T> : IEnumerable<T>, IEnumerable
	{
		[__DynamicallyInvokable]
		int Count { [__DynamicallyInvokable] get; }

		[__DynamicallyInvokable]
		bool IsReadOnly { [__DynamicallyInvokable] get; }

		[__DynamicallyInvokable]
		void Add(T item);

		[__DynamicallyInvokable]
		void Clear();

		[__DynamicallyInvokable]
		bool Contains(T item);

		[__DynamicallyInvokable]
		void CopyTo(T[] array, int arrayIndex);

		[__DynamicallyInvokable]
		bool Remove(T item);
	}
}
