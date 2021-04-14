using System;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[Flags]
	internal enum CapabilityCheck
	{
		MRS = 1,
		OtherProvider = 2,
		BothMRSAndOtherProvider = 3
	}
}
