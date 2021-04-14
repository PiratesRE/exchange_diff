using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.RpcClientAccess.Server
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class LoginFailureException : RpcServerException
	{
		internal LoginFailureException(string message) : base(message, RpcErrorCode.LoginFailure)
		{
		}

		internal LoginFailureException(string message, Exception innerException) : base(message, RpcErrorCode.LoginFailure, innerException)
		{
		}
	}
}
