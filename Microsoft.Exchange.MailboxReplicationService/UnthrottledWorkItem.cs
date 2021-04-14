using System;
using Microsoft.Exchange.WorkloadManagement;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal class UnthrottledWorkItem : WorkItem
	{
		public UnthrottledWorkItem(Action callback) : this(TimeSpan.Zero, callback)
		{
		}

		public UnthrottledWorkItem(TimeSpan delay, Action callback) : base(delay, callback, WorkloadType.MailboxReplicationServiceHighPriority)
		{
		}

		private const WorkloadType UnthrottledWorkloadType = WorkloadType.MailboxReplicationServiceHighPriority;
	}
}
