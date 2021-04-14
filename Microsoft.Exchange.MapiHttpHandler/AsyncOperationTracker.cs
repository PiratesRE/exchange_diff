using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.MapiHttp
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class AsyncOperationTracker
	{
		public void Register(AsyncOperationInfo asyncOperationInfo)
		{
			lock (this.asyncOperationsLock)
			{
				this.activeAsyncOperations.Add(asyncOperationInfo);
			}
		}

		public void Complete(AsyncOperationInfo asyncOperationInfo)
		{
			lock (this.asyncOperationsLock)
			{
				this.activeAsyncOperations.Remove(asyncOperationInfo);
				if (this.completedAsyncOperations.Count >= 16)
				{
					this.completedAsyncOperations.Dequeue();
				}
				this.completedAsyncOperations.Enqueue(asyncOperationInfo);
				if (asyncOperationInfo.FailureException != null)
				{
					if (this.failedAsyncOperations.Count >= 16)
					{
						this.failedAsyncOperations.Dequeue();
					}
					this.failedAsyncOperations.Enqueue(asyncOperationInfo);
				}
			}
		}

		public void GetAsyncOperationInfo(out AsyncOperationInfo[] activeOperations, out AsyncOperationInfo[] completedOperations, out AsyncOperationInfo[] failedOperations)
		{
			lock (this.asyncOperationsLock)
			{
				activeOperations = this.activeAsyncOperations.ToArray();
				completedOperations = this.completedAsyncOperations.ToArray();
				failedOperations = this.failedAsyncOperations.ToArray();
			}
		}

		private readonly List<AsyncOperationInfo> activeAsyncOperations = new List<AsyncOperationInfo>();

		private readonly Queue<AsyncOperationInfo> completedAsyncOperations = new Queue<AsyncOperationInfo>(16);

		private readonly Queue<AsyncOperationInfo> failedAsyncOperations = new Queue<AsyncOperationInfo>(16);

		private readonly object asyncOperationsLock = new object();
	}
}
