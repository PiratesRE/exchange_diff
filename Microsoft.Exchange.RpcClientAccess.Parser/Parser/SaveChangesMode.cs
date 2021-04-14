using System;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	[Flags]
	internal enum SaveChangesMode : byte
	{
		Close = 0,
		KeepOpenReadOnly = 1,
		KeepOpenReadWrite = 2,
		ForceSave = 4,
		DelayedCall = 8,
		SkipQuotaCheck = 16,
		TransportDelivery = 32,
		IMAPChange = 64,
		ForceNotificationPublish = 128
	}
}
