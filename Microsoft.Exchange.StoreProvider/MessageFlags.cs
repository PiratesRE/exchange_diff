using System;

namespace Microsoft.Mapi
{
	[Flags]
	internal enum MessageFlags : uint
	{
		None = 0U,
		Associated = 64U,
		FromMe = 32U,
		HasAttach = 16U,
		NrnPending = 512U,
		EverRead = 1024U,
		OriginInternet = 8192U,
		OriginMiscExt = 32768U,
		OriginX400 = 4096U,
		Read = 1U,
		Resend = 128U,
		RnPending = 256U,
		Submit = 4U,
		Unmodified = 2U,
		Unsent = 8U
	}
}
