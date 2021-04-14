using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Search.Core.AsyncTask
{
	internal sealed class AsyncTaskSequence : AsyncTaskWithChildTasks
	{
		internal AsyncTaskSequence(IList<AsyncTask> tasks) : base(tasks)
		{
		}

		public override string ToString()
		{
			return "AsyncTaskSequence for " + base.Tasks.Count + " tasks";
		}

		internal void Cancel(EventWaitHandle notifyHandle)
		{
			base.Cancel();
			this.notifyCancelled = notifyHandle;
			if (notifyHandle != null && !base.Running && Interlocked.CompareExchange<EventWaitHandle>(ref this.notifyCancelled, null, notifyHandle) == notifyHandle)
			{
				notifyHandle.Set();
			}
		}

		internal override void InternalExecute()
		{
			base.InternalExecute();
			this.currentTask = -1;
			this.ExecuteNextTask();
		}

		protected override void CompleteChildTask(AsyncTask childTask)
		{
			base.CompleteChildTask(childTask);
			if (childTask.Exception != null)
			{
				this.InternalComplete();
				return;
			}
			this.ExecuteNextTask();
		}

		private void ExecuteNextTask()
		{
			int num = Interlocked.Increment(ref this.currentTask);
			if (num < base.Tasks.Count)
			{
				if (!base.Cancelled)
				{
					AsyncTask asyncTask = base.Tasks[num];
					asyncTask.Execute(new TaskCompleteCallback(this.CompleteChildTask));
					return;
				}
				for (int i = num; i < base.Tasks.Count; i++)
				{
					base.Tasks[i].Cancel();
				}
			}
			this.InternalComplete();
		}

		private void InternalComplete()
		{
			try
			{
				base.Complete();
			}
			finally
			{
				EventWaitHandle eventWaitHandle = this.notifyCancelled;
				if (eventWaitHandle != null)
				{
					ExAssert.RetailAssert(base.Cancelled, "Task must be marked cancelled");
					if (Interlocked.CompareExchange<EventWaitHandle>(ref this.notifyCancelled, null, eventWaitHandle) == eventWaitHandle)
					{
						eventWaitHandle.Set();
					}
				}
			}
		}

		private int currentTask;

		private EventWaitHandle notifyCancelled;
	}
}
