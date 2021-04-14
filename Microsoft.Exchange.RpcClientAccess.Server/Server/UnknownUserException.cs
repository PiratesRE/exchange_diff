using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.RpcClientAccess.Server
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class UnknownUserException : RpcServerException
	{
		internal UnknownUserException(string message) : base(message, RpcErrorCode.UnknownUser)
		{
		}

		internal UnknownUserException(string message, Exception innerException) : base(message, RpcErrorCode.UnknownUser, innerException)
		{
		}
	}
}
