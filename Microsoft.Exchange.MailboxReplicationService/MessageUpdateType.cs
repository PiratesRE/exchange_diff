using System;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal enum MessageUpdateType
	{
		Delete = 1,
		SetRead,
		SetUnread
	}
}
