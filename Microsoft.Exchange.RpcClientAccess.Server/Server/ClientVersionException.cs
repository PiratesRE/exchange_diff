using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.RpcClientAccess.Server
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class ClientVersionException : RpcServerException
	{
		internal ClientVersionException(string message) : base(message, RpcErrorCode.ClientVerDisallowed)
		{
		}

		internal ClientVersionException(string message, Exception innerException) : base(message, RpcErrorCode.ClientVerDisallowed, innerException)
		{
		}
	}
}
