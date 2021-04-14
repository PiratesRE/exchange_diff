using System;

namespace Microsoft.Exchange.Server.Storage.LogicalDataModel
{
	[Flags]
	public enum MessageFlags
	{
		None = 0,
		Read = 1,
		Unmodified = 2,
		Submit = 4,
		Unsent = 8,
		HasAttachment = 16,
		FromMe = 32,
		Associated = 64,
		Resend = 128,
		ReadNotificationPending = 256,
		NonReadNotificationPending = 512,
		EverRead = 1024,
		Irm = 2048,
		OriginX400 = 4096,
		OriginInternet = 8192,
		OriginMiscExternal = 32768,
		NeedSpecialRecipientProcessing = 131072
	}
}
