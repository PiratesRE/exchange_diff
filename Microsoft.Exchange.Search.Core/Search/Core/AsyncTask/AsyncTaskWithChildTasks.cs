using System;
using System.Collections.Generic;
using Microsoft.Exchange.Search.Core.Abstraction;
using Microsoft.Exchange.Search.Core.Common;

namespace Microsoft.Exchange.Search.Core.AsyncTask
{
	internal abstract class AsyncTaskWithChildTasks : AsyncTask
	{
		internal AsyncTaskWithChildTasks(IList<AsyncTask> tasks)
		{
			Util.ThrowOnNullOrEmptyArgument<AsyncTask>(tasks, "tasks");
			this.tasks = tasks;
			this.isCompletedTask = new bool[tasks.Count];
		}

		protected IList<AsyncTask> Tasks
		{
			get
			{
				return this.tasks;
			}
		}

		internal override void InternalExecute()
		{
			for (int i = 0; i < this.isCompletedTask.Length; i++)
			{
				this.isCompletedTask[i] = false;
			}
		}

		protected virtual void CompleteChildTask(AsyncTask childTask)
		{
			int num = this.Tasks.IndexOf(childTask);
			if (num < 0 || num >= this.Tasks.Count)
			{
				throw new InvalidOperationException("The completed task is not from any known child task.");
			}
			lock (this.isCompletedTask)
			{
				if (this.isCompletedTask[num])
				{
					throw new InvalidOperationException(string.Format("The task {0} has been completed before.", num));
				}
				this.isCompletedTask[num] = true;
			}
		}

		protected void Complete()
		{
			List<ComponentException> list = new List<ComponentException>();
			foreach (AsyncTask asyncTask in this.Tasks)
			{
				if (asyncTask.Exception != null)
				{
					list.Add(asyncTask.Exception);
				}
			}
			if (list.Count > 0)
			{
				this.Complete(new AggregateException(list.ToArray()));
				return;
			}
			this.Complete(null);
		}

		private new void Complete(ComponentException exception)
		{
			base.Complete(exception);
		}

		private readonly IList<AsyncTask> tasks;

		private bool[] isCompletedTask;
	}
}
