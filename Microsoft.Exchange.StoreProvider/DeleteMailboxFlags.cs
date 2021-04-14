using System;

namespace Microsoft.Mapi
{
	[Flags]
	internal enum DeleteMailboxFlags
	{
		DeleteDSCache = 0,
		MailboxMoved = 1,
		HardDelete = 2,
		MailboxMoveFailed = 4,
		SoftDelete = 8,
		RemoveSoftDeleted = 16
	}
}
