using System;

namespace Microsoft.Exchange.Management.Powershell.Support
{
	[Flags]
	public enum DatabaseEventNames
	{
		NewMail = 2,
		ObjectCreated = 4,
		ObjectDeleted = 8,
		ObjectModified = 16,
		ObjectMoved = 32,
		ObjectCopied = 16,
		MailSubmitted = 1024,
		MailboxCreated = 2048,
		MailboxDeleted = 4096,
		MailboxDisconnected = 8192,
		MailboxReconnected = 16384,
		MailboxMoveStarted = 32768,
		MailboxMoveSucceeded = 65536,
		MailboxMoveFailed = 131072
	}
}
