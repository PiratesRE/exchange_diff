using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Microsoft.Office.CompliancePolicy.PolicySync
{
	public sealed class SerializedJobDispatcher : JobDispatcherBase
	{
		public SerializedJobDispatcher(SyncAgentContext syncAgentContext, int maxWorkItemsPerJob) : base(syncAgentContext, maxWorkItemsPerJob)
		{
		}

		public override void Dispatch(object state)
		{
			ArgumentValidator.ThrowIfNull("WorkItemQueue", base.WorkItemQueue);
			while (!base.HostStateProvider.IsShuttingDown() && !base.WorkItemQueue.IsEmpty())
			{
				IEnumerable<WorkItemBase> enumerable = base.WorkItemQueue.Dequeue(base.MaxWorkItemsPerJob);
				if (enumerable == null)
				{
					Thread.Sleep(base.SyncAgentContext.SyncAgentConfig.JobDispatcherWaitIntervalWhenStarve);
				}
				else
				{
					IEnumerable<WorkItemBase> enumerable2 = JobDispatcherBase.TrimWorkItemsOverBatchSizeLimit(enumerable);
					if (enumerable2.Any<WorkItemBase>())
					{
						base.OnWorkItemsComplete(enumerable2);
					}
					JobBase jobBase = base.JobFactory.CreateJob(enumerable, new Action<JobBase>(this.OnJobCompleted), base.SyncAgentContext);
					jobBase.Begin(null);
				}
			}
			if (!base.HostStateProvider.IsShuttingDown())
			{
				base.WorkItemQueue.OnAllWorkItemDispatched();
			}
		}
	}
}
