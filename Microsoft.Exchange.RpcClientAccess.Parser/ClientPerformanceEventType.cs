using System;

namespace Microsoft.Exchange.RpcClientAccess
{
	internal enum ClientPerformanceEventType : byte
	{
		None,
		BackgroundRpcFailed,
		BackgroundRpcSucceeded,
		ForegroundRpcFailed,
		ForegroundRpcSucceeded,
		RpcAttempted,
		RpcFailed,
		RpcSucceeded
	}
}
