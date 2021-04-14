using System;
using System.Collections.Generic;

namespace Microsoft.Office.CompliancePolicy.PolicySync
{
	public abstract class JobDispatcherBase
	{
		public JobDispatcherBase(SyncAgentContext syncAgentContext, int maxWorkItemsPerJob)
		{
			ArgumentValidator.ThrowIfNull("syncAgentContext", syncAgentContext);
			ArgumentValidator.ThrowIfZeroOrNegative("maxWorkItemsPerJob", maxWorkItemsPerJob);
			this.HostStateProvider = syncAgentContext.HostStateProvider;
			this.JobFactory = syncAgentContext.JobFactory;
			this.LogProvider = syncAgentContext.LogProvider;
			this.SyncAgentContext = syncAgentContext;
			this.MaxWorkItemsPerJob = maxWorkItemsPerJob;
		}

		public IWorkItemQueueProvider WorkItemQueue
		{
			protected get
			{
				return this.workItemQueue;
			}
			set
			{
				ArgumentValidator.ThrowIfNull("WorkItemQueue", value);
				this.workItemQueue = value;
			}
		}

		internal IJobFactory JobFactory { get; private set; }

		private protected HostStateProvider HostStateProvider { protected get; private set; }

		private protected ExecutionLog LogProvider { protected get; private set; }

		private protected SyncAgentContext SyncAgentContext { protected get; private set; }

		private protected int MaxWorkItemsPerJob { protected get; private set; }

		public abstract void Dispatch(object state);

		internal static IEnumerable<WorkItemBase> TrimWorkItemsOverBatchSizeLimit(IEnumerable<WorkItemBase> workItems)
		{
			List<WorkItemBase> list = new List<WorkItemBase>();
			foreach (WorkItemBase workItemBase in workItems)
			{
				WorkItemBase workItemBase2 = workItemBase.Split();
				if (workItemBase2 != null)
				{
					list.Add(workItemBase2);
				}
			}
			return list;
		}

		internal virtual void OnJobCompleted(JobBase job)
		{
			this.OnWorkItemsComplete(job.End());
		}

		protected void OnWorkItemsComplete(IEnumerable<WorkItemBase> workItems)
		{
			foreach (WorkItemBase item in workItems)
			{
				if (this.HostStateProvider.IsShuttingDown())
				{
					break;
				}
				this.workItemQueue.OnWorkItemCompleted(item);
			}
		}

		private IWorkItemQueueProvider workItemQueue;
	}
}
