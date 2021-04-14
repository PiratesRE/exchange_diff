using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.RpcClientAccess
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal abstract class RpcExecutionException : RpcServerException
	{
		protected RpcExecutionException(string message, RpcErrorCode storeError) : base(message, storeError)
		{
		}

		protected RpcExecutionException(string message, RpcErrorCode storeError, Exception innerException) : base(message, storeError, innerException)
		{
		}
	}
}
