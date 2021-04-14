using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Microsoft.Exchange.Transport.Scheduler.Processing
{
	internal class AsyncAutoResetEvent
	{
		public Task<bool> WaitAsync()
		{
			Task<bool> result;
			lock (this.waits)
			{
				if (this.signaled)
				{
					this.signaled = false;
					result = AsyncAutoResetEvent.Completed;
				}
				else
				{
					TaskCompletionSource<bool> taskCompletionSource = new TaskCompletionSource<bool>();
					this.waits.Enqueue(taskCompletionSource);
					result = taskCompletionSource.Task;
				}
			}
			return result;
		}

		public void Set()
		{
			TaskCompletionSource<bool> taskCompletionSource = null;
			lock (this.waits)
			{
				if (this.waits.Count > 0)
				{
					taskCompletionSource = this.waits.Dequeue();
				}
				else if (!this.signaled)
				{
					this.signaled = true;
				}
			}
			if (taskCompletionSource != null)
			{
				taskCompletionSource.SetResult(true);
			}
		}

		private static readonly Task<bool> Completed = Task.FromResult<bool>(true);

		private readonly Queue<TaskCompletionSource<bool>> waits = new Queue<TaskCompletionSource<bool>>();

		private bool signaled;
	}
}
