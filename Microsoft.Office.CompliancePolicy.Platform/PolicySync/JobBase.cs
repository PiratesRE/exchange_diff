using System;
using System.Collections.Generic;

namespace Microsoft.Office.CompliancePolicy.PolicySync
{
	internal abstract class JobBase
	{
		public JobBase(IEnumerable<WorkItemBase> workItems, Action<JobBase> onJobCompleted, SyncAgentContext syncAgentContext)
		{
			ArgumentValidator.ThrowIfCollectionNullOrEmpty<WorkItemBase>("workItems", workItems);
			ArgumentValidator.ThrowIfNull("onJobCompleted", onJobCompleted);
			ArgumentValidator.ThrowIfNull("syncAgentContext", syncAgentContext);
			this.WorkItems = workItems;
			this.HostStateProvider = syncAgentContext.HostStateProvider;
			this.OnJobCompleted = onJobCompleted;
			this.LogProvider = syncAgentContext.LogProvider;
			this.SyncAgentContext = syncAgentContext;
		}

		internal HostStateProvider HostStateProvider { get; private set; }

		internal ExecutionLog LogProvider { get; private set; }

		internal SyncAgentContext SyncAgentContext { get; private set; }

		private protected IEnumerable<WorkItemBase> WorkItems { protected get; private set; }

		private protected Action<JobBase> OnJobCompleted { protected get; private set; }

		public abstract void Begin(object state);

		public IEnumerable<WorkItemBase> End()
		{
			return this.WorkItems;
		}
	}
}
