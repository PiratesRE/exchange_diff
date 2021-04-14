using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.InfoWorker.Common.Availability
{
	internal abstract class AsyncTaskWithChildTasks : AsyncTask
	{
		protected IList<AsyncTask> Tasks
		{
			get
			{
				return this.tasks;
			}
		}

		public AsyncTaskWithChildTasks(IList<AsyncTask> tasks)
		{
			if (tasks == null)
			{
				throw new ArgumentNullException("tasks");
			}
			this.tasks = tasks;
			this.isCompletedTask = new bool[tasks.Count];
		}

		protected virtual void CompleteChildTask(AsyncTask childTask)
		{
			int num = this.tasks.IndexOf(childTask);
			if (num < 0 || num >= this.tasks.Count)
			{
				throw new InvalidOperationException();
			}
			if (this.isCompletedTask[num])
			{
				throw new InvalidOperationException();
			}
			this.isCompletedTask[num] = true;
		}

		public override void Abort()
		{
			base.Abort();
			foreach (AsyncTask asyncTask in this.tasks)
			{
				asyncTask.Abort();
			}
		}

		private IList<AsyncTask> tasks;

		private bool[] isCompletedTask;
	}
}
