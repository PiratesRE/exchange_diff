using System;
using System.Collections;
using System.Collections.Generic;

namespace System.Threading.Tasks
{
	internal interface IProducerConsumerQueue<T> : IEnumerable<!0>, IEnumerable
	{
		void Enqueue(T item);

		bool TryDequeue(out T result);

		bool IsEmpty { get; }

		int Count { get; }

		int GetCountSafe(object syncObj);
	}
}
