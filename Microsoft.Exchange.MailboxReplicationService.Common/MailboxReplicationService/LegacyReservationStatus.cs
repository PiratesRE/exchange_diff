using System;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal enum LegacyReservationStatus
	{
		Unknown,
		Success,
		CapacityExceeded,
		ResourceNotOwned,
		Error
	}
}
