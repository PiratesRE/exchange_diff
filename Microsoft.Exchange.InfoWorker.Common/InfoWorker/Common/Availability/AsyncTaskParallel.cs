using System;
using System.Collections.Generic;
using System.Threading;

namespace Microsoft.Exchange.InfoWorker.Common.Availability
{
	internal sealed class AsyncTaskParallel : AsyncTaskWithChildTasks
	{
		public AsyncTaskParallel(IList<AsyncTask> tasks) : base(tasks)
		{
			this.pendintTasksCount = tasks.Count;
		}

		public override void BeginInvoke(TaskCompleteCallback callback)
		{
			base.BeginInvoke(callback);
			for (int i = 0; i < base.Tasks.Count; i++)
			{
				AsyncTask asyncTask = base.Tasks[i];
				asyncTask.BeginInvoke(new TaskCompleteCallback(this.CompleteChildTask));
			}
		}

		protected override void CompleteChildTask(AsyncTask childTask)
		{
			base.CompleteChildTask(childTask);
			if (Interlocked.Decrement(ref this.pendintTasksCount) == 0)
			{
				base.Complete();
			}
		}

		public override string ToString()
		{
			return "AsyncTaskParallel for " + base.Tasks.Count + " tasks";
		}

		private int pendintTasksCount;
	}
}
