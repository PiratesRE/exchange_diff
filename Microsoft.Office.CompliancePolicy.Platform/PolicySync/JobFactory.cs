using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Office.CompliancePolicy.PolicySync
{
	internal sealed class JobFactory : IJobFactory
	{
		public JobBase CreateJob(IEnumerable<WorkItemBase> workItems, Action<JobBase> onJobCompleted, SyncAgentContext syncAgentContext)
		{
			ArgumentValidator.ThrowIfCollectionNullOrEmpty<WorkItemBase>("workItems", workItems);
			WorkItemBase workItemBase = workItems.First<WorkItemBase>();
			JobBase result;
			if (workItemBase is SyncWorkItem)
			{
				result = new SyncJob(workItems, onJobCompleted, syncAgentContext);
			}
			else
			{
				if (!(workItemBase is SyncStatusUpdateWorkitem))
				{
					throw new NotSupportedException(string.Format("This type of work item isn't supported: ", workItemBase.GetType()));
				}
				result = new StatusUpdatePublishJob(workItems.Cast<SyncStatusUpdateWorkitem>(), onJobCompleted, syncAgentContext);
			}
			return result;
		}
	}
}
