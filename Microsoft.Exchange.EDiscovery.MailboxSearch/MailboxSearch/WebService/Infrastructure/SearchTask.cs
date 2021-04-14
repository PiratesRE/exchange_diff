using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.ResourceHealth;
using Microsoft.Exchange.Diagnostics.WorkloadManagement;
using Microsoft.Exchange.EDiscovery.MailboxSearch.WebService.Model;
using Microsoft.Exchange.WorkloadManagement;

namespace Microsoft.Exchange.EDiscovery.MailboxSearch.WebService.Infrastructure
{
	internal abstract class SearchTask<TIn> : ITask where TIn : class
	{
		public IExecutor Executor
		{
			get
			{
				return this.Context.Executor;
			}
		}

		public ISearchPolicy Policy
		{
			get
			{
				return this.Executor.Policy;
			}
		}

		protected SearchTaskContext Context
		{
			get
			{
				return (SearchTaskContext)((ITask)this).State;
			}
		}

		public virtual void Process(IList<TIn> items)
		{
			foreach (TIn item in items)
			{
				this.Process(item);
			}
		}

		public virtual void Execute(object item)
		{
			if (item is IList<object>)
			{
				this.Process(((IList<object>)item).Cast<TIn>().ToList<TIn>());
				return;
			}
			this.Process((TIn)((object)item));
		}

		public virtual void Process(TIn item)
		{
		}

		public virtual void Complete()
		{
		}

		public virtual void Cancel()
		{
		}

		public virtual ResourceKey[] GetResources()
		{
			return null;
		}

		string ITask.Description
		{
			get
			{
				return base.GetType().Name;
			}
			set
			{
			}
		}

		TimeSpan ITask.MaxExecutionTime
		{
			get
			{
				return this.Executor.Policy.ExecutionSettings.SearchTimeout;
			}
		}

		object ITask.State { get; set; }

		WorkloadSettings ITask.WorkloadSettings
		{
			get
			{
				return new WorkloadSettings(WorkloadType.Unknown, true);
			}
		}

		IBudget ITask.Budget
		{
			get
			{
				return this.Executor.Policy.Budget;
			}
		}

		TaskExecuteResult ITask.CancelStep(LocalizedException exception)
		{
			this.Cancel();
			return TaskExecuteResult.ProcessingComplete;
		}

		void ITask.Complete(TimeSpan queueAndDelayTime, TimeSpan totalTime)
		{
			this.Complete();
		}

		TaskExecuteResult ITask.Execute(TimeSpan queueAndDelayTime, TimeSpan totalTime)
		{
			this.Execute(this.Context.Item);
			return TaskExecuteResult.ProcessingComplete;
		}

		IActivityScope ITask.GetActivityScope()
		{
			return this.Executor.Policy.GetActivityScope();
		}

		void ITask.Timeout(TimeSpan queueAndDelayTime, TimeSpan totalTime)
		{
			this.Cancel();
		}
	}
}
