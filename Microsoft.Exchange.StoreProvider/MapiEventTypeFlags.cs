using System;

namespace Microsoft.Mapi
{
	[Flags]
	internal enum MapiEventTypeFlags
	{
		CriticalError = 1,
		NewMail = 2,
		ObjectCreated = 4,
		ObjectDeleted = 8,
		ObjectModified = 16,
		ObjectMoved = 32,
		ObjectCopied = 64,
		SearchComplete = 128,
		TableModified = 256,
		StatusObjectModified = 512,
		MailSubmitted = 1024,
		Extended = -2147483648,
		MailboxCreated = 2048,
		MailboxDeleted = 4096,
		MailboxDisconnected = 8192,
		MailboxReconnected = 16384,
		MailboxMoveStarted = 32768,
		MailboxMoveSucceeded = 65536,
		MailboxMoveFailed = 131072
	}
}
