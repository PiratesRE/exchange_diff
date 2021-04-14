using System;
using Microsoft.Exchange.WorkloadManagement;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal class PeriodicWorkItem : WorkItem, IPeriodicWorkItem
	{
		public PeriodicWorkItem(TimeSpan periodicInterval, Action callback) : base(TimeSpan.Zero, callback, WorkloadType.Unknown)
		{
			((IPeriodicWorkItem)this).PeriodicInterval = periodicInterval;
		}

		TimeSpan IPeriodicWorkItem.PeriodicInterval { get; set; }
	}
}
