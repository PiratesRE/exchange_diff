using System;
using System.Threading;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage.Auditing
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal interface IQueue<T> : IDisposable
	{
		void Send(T data);

		IQueueMessage<T> GetNext(int timeoutInMilliseconds, CancellationToken cancel);

		void Commit(IQueueMessage<T> message);

		void Rollback(IQueueMessage<T> message);
	}
}
