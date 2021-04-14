using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.RpcClientAccess
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class AbortRpcExecutionException : RpcExecutionException
	{
		public AbortRpcExecutionException(string message) : base(message, RpcErrorCode.RpcFailed)
		{
		}

		public AbortRpcExecutionException(string message, Exception innerException) : base(message, RpcErrorCode.RpcFailed, innerException)
		{
		}
	}
}
