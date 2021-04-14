using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.RpcClientAccess.Monitoring
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class RetryTask : BaseTask
	{
		public RetryTask(IContext context, BaseTask task, Predicate<BaseTask> isRetryablePredicate) : base(context, Strings.RetryTaskTitle, Strings.RetryTaskDescription, TaskType.Infrastructure, new ContextProperty[]
		{
			ContextPropertySchema.RetryCount.GetOnly(),
			ContextPropertySchema.RetryInterval.GetOnly(),
			ContextPropertySchema.Latency.GetOnly(),
			ContextPropertySchema.Exception.GetOnly(),
			ContextPropertySchema.InitialLatency.SetOnly(),
			ContextPropertySchema.InitialException.SetOnly()
		})
		{
			this.task = task;
			base.Result = TaskResult.Success;
			this.isRetryablePredicate = isRetryablePredicate;
		}

		protected override IEnumerator<ITask> Process()
		{
			Util.ThrowOnNullArgument(this.task, "task");
			int numRetries;
			if (!base.Properties.TryGet(ContextPropertySchema.RetryCount, out numRetries))
			{
				numRetries = 1;
			}
			TimeSpan retryInterval;
			if (!base.Properties.TryGet(ContextPropertySchema.RetryInterval, out retryInterval))
			{
				retryInterval = RetryTask.defaultRetryInterval;
			}
			BaseTask copyTask = this.task.Copy();
			for (int i = 0; i < numRetries + 1; i++)
			{
				yield return this.task;
				base.Result = this.task.Result;
				if (base.Result == TaskResult.Success || i >= numRetries || !this.isRetryablePredicate(this.task))
				{
					break;
				}
				if (i == 0)
				{
					TimeSpan value;
					if (this.task.Properties.TryGet(ContextPropertySchema.Latency, out value))
					{
						base.Set<TimeSpan>(ContextPropertySchema.InitialLatency, value);
					}
					Exception value2;
					if (this.task.Properties.TryGet(ContextPropertySchema.Exception, out value2))
					{
						base.Set<Exception>(ContextPropertySchema.InitialException, value2);
					}
				}
				yield return this.CreateSleepTask(retryInterval);
				this.task = copyTask.Copy();
			}
			yield break;
		}

		private ITask CreateSleepTask(TimeSpan sleepInterval)
		{
			Timer timer = null;
			return new AsyncTask(base.CreateDerivedContext(), delegate(AsyncCallback asyncCallback, object asyncState)
			{
				EasyAsyncResult easyAsyncResult = new EasyAsyncResult(asyncCallback, asyncState);
				timer = new Timer(delegate(object state)
				{
					EasyAsyncResult easyAsyncResult2 = (EasyAsyncResult)state;
					easyAsyncResult2.InvokeCallback();
				}, easyAsyncResult, sleepInterval, TimeSpan.FromMilliseconds(-1.0));
				return easyAsyncResult;
			}, delegate(IAsyncResult asyncResult)
			{
				timer.Dispose();
				return TaskResult.Success;
			});
		}

		private const int DefaultNumberOfRetries = 1;

		private static readonly TimeSpan defaultRetryInterval = TimeSpan.FromSeconds(1.0);

		private BaseTask task;

		private Predicate<BaseTask> isRetryablePredicate;
	}
}
