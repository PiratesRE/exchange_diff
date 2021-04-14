using System;
using System.Collections.Generic;
using System.Threading;

namespace Microsoft.Exchange.Search.Core.AsyncTask
{
	internal sealed class AsyncTaskParallel : AsyncTaskWithChildTasks
	{
		internal AsyncTaskParallel(IList<AsyncTask> tasks) : base(tasks)
		{
		}

		public override string ToString()
		{
			return "AsyncTaskParallel for " + base.Tasks.Count + " tasks";
		}

		internal override void InternalExecute()
		{
			base.InternalExecute();
			this.pendingTasksCount = base.Tasks.Count;
			for (int i = 0; i < base.Tasks.Count; i++)
			{
				AsyncTask asyncTask = base.Tasks[i];
				asyncTask.Execute(new TaskCompleteCallback(this.CompleteChildTask));
			}
		}

		protected override void CompleteChildTask(AsyncTask childTask)
		{
			base.CompleteChildTask(childTask);
			if (Interlocked.Decrement(ref this.pendingTasksCount) == 0)
			{
				base.Complete();
			}
		}

		private int pendingTasksCount;
	}
}
