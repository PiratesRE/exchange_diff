using System;

namespace Microsoft.Exchange.Data.Storage
{
	[Flags]
	internal enum EventType
	{
		None = 0,
		NewMail = 1,
		ObjectCreated = 2,
		ObjectDeleted = 4,
		ObjectModified = 8,
		ObjectMoved = 16,
		ObjectCopied = 32,
		FreeBusyChanged = 64,
		CriticalError = 256,
		MailboxDeleted = 512,
		MailboxDisconnected = 1024,
		MailboxMoveFailed = 2048,
		MailboxMoveStarted = 4096,
		MailboxMoveSucceeded = 8192
	}
}
