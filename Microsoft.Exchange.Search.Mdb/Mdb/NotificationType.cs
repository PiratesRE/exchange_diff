using System;

namespace Microsoft.Exchange.Search.Mdb
{
	internal enum NotificationType
	{
		Uninteresting,
		Insert,
		Update,
		Delete,
		Move,
		ReadFlagChange,
		DeleteMailbox
	}
}
