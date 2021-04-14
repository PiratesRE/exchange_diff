using System;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	[Flags]
	internal enum MessageFlags
	{
		None = 0,
		Read = 1,
		Unmodified = 2,
		Submit = 4,
		Unsent = 8,
		HasAttach = 16,
		FromMe = 32,
		Associated = 64,
		Resend = 128,
		RnPending = 256,
		NrnPending = 512,
		EverRead = 1024,
		Restricted = 2048,
		OriginX400 = 4096,
		OriginInternet = 8192,
		OriginMiscExt = 32768,
		OutlookNonEmsTransport = 65536
	}
}
