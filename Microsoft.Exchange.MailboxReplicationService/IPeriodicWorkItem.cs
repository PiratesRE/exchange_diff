using System;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal interface IPeriodicWorkItem
	{
		TimeSpan PeriodicInterval { get; set; }
	}
}
