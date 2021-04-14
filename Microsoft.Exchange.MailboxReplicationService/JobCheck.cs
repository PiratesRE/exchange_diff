using System;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal class JobCheck : UnthrottledWorkItem, IPeriodicWorkItem
	{
		public JobCheck(TimeSpan periodicInterval, Action callback) : base(TimeSpan.Zero, callback)
		{
			((IPeriodicWorkItem)this).PeriodicInterval = periodicInterval;
		}

		TimeSpan IPeriodicWorkItem.PeriodicInterval { get; set; }
	}
}
