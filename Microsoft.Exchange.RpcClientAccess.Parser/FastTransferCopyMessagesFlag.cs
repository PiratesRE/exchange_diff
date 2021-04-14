using System;

namespace Microsoft.Exchange.RpcClientAccess
{
	[Flags]
	internal enum FastTransferCopyMessagesFlag : byte
	{
		None = 0,
		Move = 1,
		BestBody = 16,
		SendEntryId = 32
	}
}
