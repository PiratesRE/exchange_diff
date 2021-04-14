using System;

namespace Microsoft.Exchange.MailboxReplicationService
{
	public enum EasMessageCategory
	{
		AddOrUpdate,
		Delete,
		ChangeToRead,
		ChangeToUnread
	}
}
