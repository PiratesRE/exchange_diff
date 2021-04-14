using System;
using System.Collections.Generic;

namespace System.Collections.Concurrent
{
	[__DynamicallyInvokable]
	public interface IProducerConsumerCollection<T> : IEnumerable<!0>, IEnumerable, ICollection
	{
		[__DynamicallyInvokable]
		void CopyTo(T[] array, int index);

		[__DynamicallyInvokable]
		bool TryAdd(T item);

		[__DynamicallyInvokable]
		bool TryTake(out T item);

		[__DynamicallyInvokable]
		T[] ToArray();
	}
}
