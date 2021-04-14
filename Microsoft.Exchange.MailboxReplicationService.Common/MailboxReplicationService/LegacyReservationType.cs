using System;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal enum LegacyReservationType
	{
		Read = 1,
		Write,
		Release,
		Expire,
		ExpiredRead,
		ExpiredWrite
	}
}
