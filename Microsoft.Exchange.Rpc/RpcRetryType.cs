using System;

namespace Microsoft.Exchange.Rpc
{
	[Flags]
	internal enum RpcRetryType : uint
	{
		None = 0U,
		CallCancelled = 1U,
		ServerBusy = 2U,
		ServerUnavailable = 4U,
		AccessDenied = 8U
	}
}
