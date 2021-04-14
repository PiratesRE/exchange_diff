using System;

namespace Microsoft.Exchange.Data.Storage
{
	[Flags]
	internal enum RecipientFlags
	{
		UnSendable = 0,
		Sendable = 1,
		Organizer = 2,
		ExceptionalResponse = 16,
		ExceptionalDeleted = 32,
		Added = 64,
		AddedOnSend = 128,
		OriginalRecipient = 256,
		EvaluatedForRoom = 512
	}
}
