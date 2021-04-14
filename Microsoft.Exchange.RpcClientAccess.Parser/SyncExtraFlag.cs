using System;

namespace Microsoft.Exchange.RpcClientAccess
{
	[Flags]
	internal enum SyncExtraFlag : uint
	{
		None = 0U,
		Eid = 1U,
		MessageSize = 2U,
		Cn = 4U,
		OrderByDeliveryTime = 8U,
		NoChanges = 16U,
		ManifestMode = 32U,
		CatchUpFull = 64U,
		ReadCn = 128U
	}
}
