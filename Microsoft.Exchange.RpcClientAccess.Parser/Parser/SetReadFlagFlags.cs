using System;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	[Flags]
	internal enum SetReadFlagFlags : byte
	{
		Read = 0,
		SuppressReceipt = 1,
		FolderMessageDialog = 2,
		ClearReadFlag = 4,
		DeferredErrors = 8,
		GenerateReceiptOnly = 16,
		ClearReadNotificationPending = 32,
		ClearNonReadNotificationPending = 64
	}
}
