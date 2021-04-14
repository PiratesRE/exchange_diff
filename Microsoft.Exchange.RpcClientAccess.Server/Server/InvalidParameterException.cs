using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.RpcClientAccess.Server
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class InvalidParameterException : RpcServerException
	{
		internal InvalidParameterException(string message) : base(message, RpcErrorCode.InvalidParam)
		{
		}
	}
}
