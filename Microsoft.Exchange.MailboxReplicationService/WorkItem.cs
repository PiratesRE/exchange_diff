using System;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.WorkloadManagement;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal class WorkItem
	{
		public WorkItem(TimeSpan delay, Action callback) : this(delay, callback, WorkloadType.Unknown)
		{
		}

		public WorkItem(TimeSpan delay, Action callback, WorkloadType workloadType)
		{
			this.Callback = callback;
			this.ScheduledRunTime = ((delay == TimeSpan.Zero) ? ExDateTime.MinValue : ExDateTime.UtcNow.Add(delay));
			this.WorkloadType = workloadType;
		}

		public Action Callback { get; private set; }

		public WorkloadType WorkloadType { get; private set; }

		public ExDateTime ScheduledRunTime { get; set; }
	}
}
