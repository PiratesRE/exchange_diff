using System;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[Flags]
	internal enum JobSortFlags
	{
		None = 0,
		IsInteractive = 1,
		HasReservations = 2
	}
}
