using System;

namespace Microsoft.Exchange.Server.Storage.LogicalDataModel
{
	[Flags]
	public enum EventType
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
		Ics = 512,
		MailSubmitted = 1024,
		MailboxCreated = 2048,
		MailboxDeleted = 4096,
		MailboxDisconnected = 8192,
		MailboxReconnected = 16384,
		MailboxMoveStarted = 32768,
		MailboxMoveSucceeded = 65536,
		MailboxMoveFailed = 131072,
		CategRowAdded = 262144,
		CategRowModified = 524288,
		CategRowDeleted = 1048576,
		BeginLongOperation = 2097152,
		EndLongOperation = 4194304,
		DaclCreated = 8388608,
		MessageUnlinked = 16777216,
		MailboxModified = 33554432,
		MessagesLinked = 67108864,
		ReservedForMapi = 1073741824,
		Extended = -2147483648
	}
}
