using System;

namespace Microsoft.Exchange.Data.Directory.Recipient
{
	[Flags]
	internal enum TransportSettingFlag
	{
		None = 0,
		MessageTrackingReadStatusDisabled = 4,
		InternalOnly = 8,
		OpenDomainRoutingDisabled = 16,
		QueryBaseDNRestrictionEnabled = 32,
		AllowArchiveAddressSync = 64,
		MessageCopyForSentAsEnabled = 128,
		MessageCopyForSendOnBehalfEnabled = 256,
		All = 504
	}
}
