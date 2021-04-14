using System;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal enum ActionId
	{
		None,
		MarkAsRead,
		MarkAsUnRead,
		Move,
		Send,
		Delete,
		Flag,
		FlagClear,
		FlagComplete,
		CreateCalendarEvent,
		UpdateCalendarEvent,
		Max
	}
}
