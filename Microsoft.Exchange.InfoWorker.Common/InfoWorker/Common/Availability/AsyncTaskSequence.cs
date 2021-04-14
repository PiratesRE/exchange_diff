using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.InfoWorker.Common.Availability
{
	internal sealed class AsyncTaskSequence : AsyncTaskWithChildTasks
	{
		public AsyncTaskSequence(IList<AsyncTask> tasks) : base(tasks)
		{
			this.currentTask = -1;
		}

		public override void BeginInvoke(TaskCompleteCallback callback)
		{
			base.BeginInvoke(callback);
			this.BeginInvokeNextTask();
		}

		private void BeginInvokeNextTask()
		{
			this.currentTask++;
			if (this.currentTask < base.Tasks.Count && !base.Aborted)
			{
				AsyncTask asyncTask = base.Tasks[this.currentTask];
				asyncTask.BeginInvoke(new TaskCompleteCallback(this.CompleteChildTask));
				return;
			}
			base.Complete();
		}

		protected override void CompleteChildTask(AsyncTask childTask)
		{
			base.CompleteChildTask(childTask);
			this.BeginInvokeNextTask();
		}

		public override string ToString()
		{
			return "AsyncTaskSequence for " + base.Tasks.Count + " tasks";
		}

		private int currentTask;
	}
}
