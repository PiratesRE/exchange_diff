using System;

namespace Microsoft.Mapi
{
	[Flags]
	internal enum MessageStatus
	{
		None = 0,
		Highlighted = 1,
		Tagged = 2,
		Hidden = 4,
		DelMarked = 8,
		Draft = 256,
		Answered = 512,
		InConflict = 2048,
		RemoteDownload = 4096,
		RemoteDelete = 8192,
		MessageDeliveryNotificationSent = 16384,
		MimeConversionFailed = 32768
	}
}
