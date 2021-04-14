using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.RpcClientAccess.Server
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class CallFailedException : RpcServerException
	{
		internal CallFailedException(string message) : base(message, RpcErrorCode.Error)
		{
		}

		internal CallFailedException(string message, Exception innerException) : base(message, RpcErrorCode.Error, innerException)
		{
		}
	}
}
