using System;

namespace Microsoft.Exchange.MailboxTransport.StoreDriverCommon
{
	internal enum MessageFlags
	{
		Read = 1,
		UnModified,
		Submit = 4,
		UnSent = 8,
		HasAttach = 16,
		FromMe = 32,
		Associated = 64,
		Resend = 128,
		RnPending = 256,
		NrnPending = 512,
		OriginX400 = 4096,
		OriginInternet = 8192,
		OriginMiscExt = 32768
	}
}
