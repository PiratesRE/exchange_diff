using System;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[Flags]
	public enum UserMailboxFlags
	{
		None = 0,
		RecoveryMDB = 1,
		Disconnected = 2,
		SoftDeleted = 4,
		MoveDestination = 8
	}
}
