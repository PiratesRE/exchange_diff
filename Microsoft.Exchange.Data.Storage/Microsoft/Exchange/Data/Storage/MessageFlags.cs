using System;

namespace Microsoft.Exchange.Data.Storage
{
	[Flags]
	internal enum MessageFlags
	{
		None = 0,
		IsRead = 1,
		IsUnmodified = 2,
		HasBeenSubmitted = 4,
		IsDraft = 8,
		IsFromMe = 32,
		IsAssociated = 64,
		IsResend = 128,
		IsReadReceiptPending = 256,
		IsNotReadReceiptPending = 512,
		WasEverRead = 1024,
		IsRestricted = 2048,
		NeedSpecialRecipientProcessing = 131072
	}
}
