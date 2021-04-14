using System;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[Flags]
	internal enum ReservationFlags
	{
		None = 0,
		Read = 1,
		Write = 2,
		HighPriority = 4,
		Move = 16,
		Merge = 32,
		Archive = 64,
		PST = 128,
		Interactive = 256,
		InternalMaintenance = 512
	}
}
