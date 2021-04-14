using System;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	[Flags]
	internal enum MessageStatusFlags : uint
	{
		None = 0U,
		Highlighted = 1U,
		Tagged = 2U,
		Hidden = 4U,
		DeleteMarked = 8U,
		Draft = 256U,
		Answered = 512U,
		InConflict = 2048U,
		RemoteDownload = 4096U,
		RemoteDelete = 8192U,
		MessageDeliveryNotificationSent = 16384U,
		MimeConversionFailed = 32768U
	}
}
