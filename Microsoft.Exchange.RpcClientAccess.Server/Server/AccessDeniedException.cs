using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.RpcClientAccess.Server
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class AccessDeniedException : RpcServerException
	{
		internal AccessDeniedException(string message) : base(message, RpcErrorCode.AccessDenied)
		{
		}

		internal AccessDeniedException(string message, Exception innerException) : base(message, RpcErrorCode.AccessDenied, innerException)
		{
		}
	}
}
