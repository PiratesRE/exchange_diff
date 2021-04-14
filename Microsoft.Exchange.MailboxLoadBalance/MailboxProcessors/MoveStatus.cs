using System;

namespace Microsoft.Exchange.MailboxLoadBalance.MailboxProcessors
{
	internal enum MoveStatus
	{
		MoveExistsNotInProgress,
		MoveExistsInProgress,
		MoveDoesNotExist
	}
}
